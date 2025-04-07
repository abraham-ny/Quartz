using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace Origin.Networking
{
    public sealed class NetworkManager : IDisposable
    {
        private readonly ConcurrentQueue<HttpRequest> _requestQueue = new();
        private readonly HttpClient _httpClient;
        private readonly UserAgent _userAgent;

        public NetworkManager(UserAgent userAgent)
        {
            _userAgent = userAgent;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent.AgentString);
        }
        private bool _isRunning;

        public void Initialize()
        {
            _isRunning = true;
            // Start processing requests in background
            Task.Run(ProcessRequestsAsync);
        }

        public void QueueRequest(HttpRequest request)
        {
            _requestQueue.Enqueue(request);
        }

        private async Task ProcessRequestsAsync()
        {
            while (_isRunning)
            {
                if (_requestQueue.TryDequeue(out var request))
                {
                    try
                    {
                        var response = await _httpClient.SendAsync(request);
                        request.Callback?.Invoke(response);
                    }
                    catch (Exception ex)
                    {
                        request.Callback?.Invoke(new HttpResponse
                        {
                            StatusCode = HttpStatusCode.InternalServerError,
                            Error = ex.Message
                        });
                    }
                }
                else
                {
                    await Task.Delay(10); // Small delay when queue is empty
                }
            }
        }

        public void ProcessRequests()
        {
            // Synchronous processing for the main thread
            while (_requestQueue.TryDequeue(out var request))
            {
                try
                {
                    var response = _httpClient.Send(request);
                    request.Callback?.Invoke(response);
                }
                catch (Exception ex)
                {
                    request.Callback?.Invoke(new HttpResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        Error = ex.Message
                    });
                }
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            _httpClient.Dispose();
        }
    }

    public class HttpRequest
    {
        public required Uri Url { get; init; }
        public Action<HttpResponse>? Callback { get; init; }
    }

    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; init; }
        public string? Content { get; init; }
        public string? Error { get; init; }
    }
}
