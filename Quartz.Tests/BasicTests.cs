using Quartz.Core;
using Quartz.Networking;
using Quartz.Rendering;
using Quartz.Vostro;
using Xunit;

namespace Quartz.Tests
{
    public class BasicTests
    {
        [Fact]
        public void FixedPointMath_ConvertsCorrectly()
        {
            float testValue = 1.5f;
            int fixedValue = FixedPointMath.ToFixed(testValue);
            float convertedBack = FixedPointMath.ToFloat(fixedValue);
            Assert.Equal(testValue, convertedBack, 3);
        }

        [Fact]
        public void Engine_InitializesComponents()
        {
            using var engine = new QuartzEngine();
            engine.Initialize();
            // If we got here without exceptions, initialization succeeded
            Assert.True(true);
        }

        [Fact]
        public void NetworkManager_ProcessesRequests()
        {
            using var manager = new NetworkManager();
            manager.Initialize();
            
            var testRequest = new HttpRequest
            {
                Url = new Uri("http://example.com"),
                Callback = response => Assert.Equal(HttpStatusCode.OK, response.StatusCode)
            };
            
            manager.QueueRequest(testRequest);
        }

        [Fact]
        public void RenderPipeline_CreatesFrameBuffer()
        {
            using var pipeline = new RenderPipeline();
            pipeline.Initialize();
            var frame = pipeline.GetCurrentFrame();
            Assert.NotNull(frame);
        }

        [Fact]
        public void VostroEngine_ProcessesScripts()
        {
            using var engine = new VostroEngine();
            engine.Initialize();
            engine.ExecuteScript("console.log('test')");
            engine.ExecutePendingTasks();
            // If we got here without exceptions, execution succeeded
            Assert.True(true);
        }
    }
}
