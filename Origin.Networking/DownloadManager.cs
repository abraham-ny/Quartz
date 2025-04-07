using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Origin.Networking
{
    public class DownloadManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, DownloadTask> _activeDownloads = new();
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _concurrencySemaphore;
        private bool _isRunning;

        public DownloadManager(int maxConcurrentDownloads = 4)
        {
            _httpClient = new HttpClient();
            _concurrencySemaphore = new SemaphoreSlim(maxConcurrentDownloads);
        }

        public async Task<DownloadTask> StartDownloadAsync(
            string url, 
            string destinationPath,
            IProgress<DownloadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            var download = new DownloadTask(url, destinationPath);
            _activeDownloads.TryAdd(download.Id, download);

            await _concurrencySemaphore.WaitAsync(cancellationToken);
            try
            {
                await download.StartAsync(_httpClient, progress, cancellationToken);
            }
            finally
            {
                _concurrencySemaphore.Release();
                _activeDownloads.TryRemove(download.Id, out _);
            }

            return download;
        }

        public bool TryPauseDownload(string downloadId)
        {
            if (_activeDownloads.TryGetValue(downloadId, out var download))
            {
                return download.Pause();
            }
            return false;
        }

        public bool TryResumeDownload(string downloadId)
        {
            if (_activeDownloads.TryGetValue(downloadId, out var download))
            {
                return download.Resume();
            }
            return false;
        }

        public bool TryCancelDownload(string downloadId)
        {
            if (_activeDownloads.TryGetValue(downloadId, out var download))
            {
                download.Cancel();
                return true;
            }
            return false;
        }

        public IEnumerable<DownloadTask> GetActiveDownloads()
        {
            return _activeDownloads.Values;
        }

        public void Dispose()
        {
            _isRunning = false;
            _httpClient.Dispose();
            _concurrencySemaphore.Dispose();
        }
    }

    public class DownloadTask
    {
        public string Id { get; }
        public string Url { get; }
        public string DestinationPath { get; }
        public DownloadState State { get; private set; }
        public long BytesDownloaded { get; private set; }
        public long? TotalBytes { get; private set; }

        private CancellationTokenSource _cancellationTokenSource;

        public DownloadTask(string url, string destinationPath)
        {
            Id = Guid.NewGuid().ToString();
            Url = url;
            DestinationPath = destinationPath;
            State = DownloadState.Pending;
        }

        public async Task StartAsync(
            HttpClient httpClient,
            IProgress<DownloadProgress> progress = null,
            CancellationToken externalToken = default)
        {
            State = DownloadState.Downloading;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(externalToken);

            try
            {
                using var response = await httpClient.GetAsync(
                    Url, 
                    HttpCompletionOption.ResponseHeadersRead, 
                    _cancellationTokenSource.Token);

                response.EnsureSuccessStatusCode();
                TotalBytes = response.Content.Headers.ContentLength;

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(
                    DestinationPath, 
                    FileMode.Create, 
                    FileAccess.Write, 
                    FileShare.None);

                var buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(
                    buffer, 
                    0, 
                    buffer.Length, 
                    _cancellationTokenSource.Token)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    BytesDownloaded += bytesRead;

                    progress?.Report(new DownloadProgress(
                        BytesDownloaded, 
                        TotalBytes,
                        State));
                }

                State = DownloadState.Completed;
            }
            catch (OperationCanceledException)
            {
                State = _cancellationTokenSource.IsCancellationRequested 
                    ? DownloadState.Cancelled 
                    : DownloadState.Paused;
            }
            catch
            {
                State = DownloadState.Failed;
                throw;
            }
        }

        public bool Pause()
        {
            if (State == DownloadState.Downloading)
            {
                _cancellationTokenSource?.Cancel();
                State = DownloadState.Paused;
                return true;
            }
            return false;
        }

        public bool Resume()
        {
            if (State == DownloadState.Paused)
            {
                State = DownloadState.Pending;
                return true;
            }
            return false;
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
            State = DownloadState.Cancelled;
        }
    }

    public enum DownloadState
    {
        Pending,
        Downloading,
        Paused,
        Completed,
        Cancelled,
        Failed
    }

    public class DownloadProgress
    {
        public long BytesDownloaded { get; }
        public long? TotalBytes { get; }
        public DownloadState State { get; }

        public DownloadProgress(long bytesDownloaded, long? totalBytes, DownloadState state)
        {
            BytesDownloaded = bytesDownloaded;
            TotalBytes = totalBytes;
            State = state;
        }
    }
}
