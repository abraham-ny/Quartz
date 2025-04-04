using System;
using System.Threading.Tasks;
using Quartz.Networking;
using Quartz.Rendering;
using Quartz.Vostro;

namespace Quartz.Core
{
    public sealed class QuartzEngine : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly RenderPipeline _renderPipeline;
        private readonly VostroEngine _vostroEngine;

        public QuartzEngine()
        {
            _networkManager = new NetworkManager();
            _renderPipeline = new RenderPipeline();
            _vostroEngine = new VostroEngine();
        }

        public void Initialize()
        {
            // Initialize components in parallel
            Parallel.Invoke(
                () => _networkManager.Initialize(),
                () => _renderPipeline.Initialize(),
                () => _vostroEngine.Initialize()
            );
        }

        public void Run()
        {
            // Main engine loop
            while (true)
            {
                // Process network requests
                _networkManager.ProcessRequests();

                // Execute JavaScript
                _vostroEngine.ExecutePendingTasks();

                // Render frame
                _renderPipeline.RenderFrame();
            }
        }

        public void Dispose()
        {
            _networkManager.Dispose();
            _renderPipeline.Dispose();
            _vostroEngine.Dispose();
        }
    }
}
