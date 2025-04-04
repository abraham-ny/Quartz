using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Quartz.Vostro.Compiler;
using Quartz.Vostro.Runtime;

namespace Quartz.Vostro
{
    public sealed class VostroEngine : IDisposable
    {
        private readonly V8Isolate _isolate;
        private readonly V8Context _context;
        private readonly ConcurrentQueue<ScriptTask> _taskQueue = new();
        private readonly JITCompiler _compiler;
        private bool _isRunning;

        public VostroEngine()
        {
            _isolate = new V8Isolate();
            _context = _isolate.CreateContext();
            _compiler = new JITCompiler();
        }

        public void Initialize()
        {
            _isRunning = true;
            Task.Run(ExecutionLoopAsync);
        }

        public void ExecuteScript(string script, string sourceUrl = "")
        {
            var compiled = _compiler.Compile(script, sourceUrl);
            _taskQueue.Enqueue(new ScriptTask(compiled));
        }

        private async Task ExecutionLoopAsync()
        {
            while (_isRunning)
            {
                if (_taskQueue.TryDequeue(out var task))
                {
                    try
                    {
                        var result = _context.Execute(task.CompiledCode);
                        task.CompletionSource.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        task.CompletionSource.SetException(ex);
                    }
                }
                await Task.Delay(1);
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            _context.Dispose();
            _isolate.Dispose();
        }
    }

    internal class ScriptTask
    {
        public CompiledScript CompiledCode { get; }
        public TaskCompletionSource<JsValue> CompletionSource { get; }

        public ScriptTask(CompiledScript code)
        {
            CompiledCode = code;
            CompletionSource = new TaskCompletionSource<JsValue>();
        }
    }
}
