using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Origin.Networking;
using Origin.Rendering;
using Origin.Vostro;

namespace Origin.Core
{
    public class EngineInitializationException : Exception
    {
        public EngineInitializationException(string message) : base(message) { }
        public EngineInitializationException(string message, Exception inner) : base(message, inner) { }
    }

    internal class TaskScheduler : IDisposable
    {
        private readonly PriorityQueue<Action, int> _taskQueue = new();
        private readonly object _queueLock = new();
        private readonly Thread[] _workerThreads;
        private bool _isRunning;

        public TaskScheduler(int workerCount = 4)
        {
            _workerThreads = new Thread[workerCount];
            for (int i = 0; i < workerCount; i++)
            {
                _workerThreads[i] = new Thread(WorkerLoop) { IsBackground = true };
                _workerThreads[i].Start();
            }
            _isRunning = true;
        }

        public void Run(Action task, int priority = 0)
        {
            lock (_queueLock)
            {
                _taskQueue.Enqueue(task, priority);
                Monitor.Pulse(_queueLock);
            }
        }

        private void WorkerLoop()
        {
            while (_isRunning)
            {
                Action? task = null;
                lock (_queueLock)
                {
                    while (_taskQueue.Count == 0 && _isRunning)
                    {
                        Monitor.Wait(_queueLock);
                    }

                    if (_taskQueue.Count > 0)
                    {
                        _taskQueue.TryDequeue(out task, out _);
                    }
                }

                task?.Invoke();
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            lock (_queueLock)
            {
                Monitor.PulseAll(_queueLock);
            }

            foreach (var thread in _workerThreads)
            {
                thread.Join();
            }
        }
    }

    public sealed class OriginEngine : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly RenderPipeline _renderPipeline;
        private readonly VostroEngine _vostroEngine;
        private readonly Stopwatch _frameTimer = new();
        private readonly FrameRateController _frameRateController = new();
        private readonly TaskScheduler _taskScheduler = new();
        private bool _isRunning;

        public OriginEngine()
        {
            _networkManager = new NetworkManager();
            _renderPipeline = new RenderPipeline();
            _vostroEngine = new VostroEngine();
        }

        public void Initialize()
        {
            // Initialize components with proper error handling
            try
            {
                Parallel.Invoke(
                    () => _taskScheduler.Run(() => _networkManager.Initialize()),
                    () => _taskScheduler.Run(() => _renderPipeline.Initialize()),
                    () => _taskScheduler.Run(() => _vostroEngine.Initialize())
                );
            }
            catch (AggregateException ex)
            {
                throw new EngineInitializationException("Failed to initialize engine components", ex);
            }
        }

        public void Run(int targetFps = 60)
        {
            _isRunning = true;
            _frameRateController.TargetFps = targetFps;
            _frameTimer.Start();

            while (_isRunning)
            {
                _frameRateController.BeginFrame();
                
                try
                {
                    // Process tasks in priority order
                    _taskScheduler.Run(() => _networkManager.ProcessRequests());
                    _taskScheduler.Run(() => _vostroEngine.ExecutePendingTasks());
                    
                    if (_frameRateController.ShouldRender)
                    {
                        _renderPipeline.RenderFrame();
                        _frameRateController.RenderedFrame();
                    }

                    // Sleep if we're ahead of schedule
                    _frameRateController.ThrottleFrame();
                }
                catch (Exception ex)
                {
                    // Log error but keep running
                    Debug.WriteLine($"Engine error: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Dispose()
        {
            Stop();
            _frameTimer.Stop();
            _networkManager.Dispose();
            _renderPipeline.Dispose();
            _vostroEngine.Dispose();
            _taskScheduler.Dispose();
        }

        public double CurrentFps => _frameRateController.CurrentFps;
        public double FrameTime => _frameRateController.FrameTime;
    }

    internal class FrameRateController
    {
        private readonly Stopwatch _frameTimer = new();
        private readonly RollingAverage _fpsAverage = new(60);
        private double _frameBudgetMs;
        private double _lastFrameTime;

        public int TargetFps { get; set; } = 60;
        public bool ShouldRender { get; private set; } = true;
        public double CurrentFps => _fpsAverage.Average;
        public double FrameTime => _lastFrameTime;

        public void BeginFrame()
        {
            _frameTimer.Restart();
            _frameBudgetMs = 1000.0 / TargetFps;
            ShouldRender = true;
        }

        public void RenderedFrame()
        {
            _lastFrameTime = _frameTimer.Elapsed.TotalMilliseconds;
            _fpsAverage.AddSample(1000.0 / _lastFrameTime);
        }

        public void ThrottleFrame()
        {
            var elapsed = _frameTimer.Elapsed.TotalMilliseconds;
            var remaining = _frameBudgetMs - elapsed;
            
            if (remaining > 1)
            {
                Thread.Sleep((int)remaining);
            }
        }
    }

    internal class RollingAverage
    {
        private readonly double[] _samples;
        private int _index;
        private double _sum;

        public RollingAverage(int sampleCount)
        {
            _samples = new double[sampleCount];
        }

        public void AddSample(double value)
        {
            _sum -= _samples[_index];
            _samples[_index] = value;
            _sum += value;
            _index = (_index + 1) % _samples.Length;
        }

        public double Average => _sum / _samples.Length;
    }
}
