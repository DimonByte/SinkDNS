//MIT License

//Copyright (c) 2025 Dimon

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.IO.Compression;
using System.Net.Http.Headers;

namespace SinkDNS.Modules.SinkDNSInternals
{
    //This will manage downloads for SinkDNS, DNSCrypt, and BlockLists, including starting, stopping, and monitoring download progress.
    internal class DownloadManager
    {
        private static readonly HttpClient httpClient = new();
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        static DownloadManager()
        {
            // Set default headers to mimic a browser
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SinkDNS", "1.0"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.Timeout = DefaultTimeout;
        }

        public static async Task<bool> DownloadFileAsync(string url, string localPath, CancellationToken cancellationToken = default)
        {
            try
            {
                TraceLogger.Log($"Download from {url} to {localPath} triggered. Running checks...");
                if (string.IsNullOrEmpty(url))
                {
                    TraceLogger.Log("URL is null or empty", Enums.StatusSeverityType.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(localPath))
                {
                    TraceLogger.Log("Local path is null or empty", Enums.StatusSeverityType.Error);
                    return false;
                }

                string? directory = Path.GetDirectoryName(localPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    TraceLogger.Log($"Directory created: {directory}");
                }

                TraceLogger.Log("About to check HTTPclient GetAsync...");
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromMinutes(5));
                HttpResponseMessage response = await httpClient.GetAsync(url, cts.Token).ConfigureAwait(false);
                TraceLogger.Log("HTTPclient GetAsync passed through.");

                if (response.IsSuccessStatusCode)
                {
                    TraceLogger.Log($"HTTP response received with status code: {response.StatusCode}");
                    TraceLogger.Log($"Downloading file from {url} to {localPath}");
                    byte[] contentBytes = await response.Content.ReadAsByteArrayAsync(cts.Token).ConfigureAwait(false);
                    bool isGzipped = response.Content.Headers.ContentEncoding?.Any(e => e.Contains("gzip")) ?? false;
                    if (isGzipped)
                    {
                        TraceLogger.Log("Content is gzipped, decompressing...");
                        using var compressedStream = new MemoryStream(contentBytes);
                        using var decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress);
                        using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
                        await decompressedStream.CopyToAsync(fileStream, cts.Token).ConfigureAwait(false);
                    }
                    else
                    {
                        TraceLogger.Log("Content is not gzipped, writing directly to file...");
                        using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
                        await fileStream.WriteAsync(contentBytes, cts.Token).ConfigureAwait(false);
                    }

                    TraceLogger.Log("Download completed successfully");
                    return true;
                }
                else
                {
                    TraceLogger.Log($"Download failed with status code: {response.StatusCode}", Enums.StatusSeverityType.Error);
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                TraceLogger.Log("Download was cancelled or timed out", Enums.StatusSeverityType.Error);
                return false;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error downloading file: {ex.Message}", Enums.StatusSeverityType.Error);
                TraceLogger.Log($"Exception details: {ex}", Enums.StatusSeverityType.Error);
                return false;
            }
        }

        public static async Task<string?> DownloadStringAsync(string url)
        {
            try
            {
                TraceLogger.Log($"Downloading string from {url}");
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    TraceLogger.Log($"Download failed with status code: {response.StatusCode}", Enums.StatusSeverityType.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error downloading string: {ex.Message}", Enums.StatusSeverityType.Error);
                return null;
            }
        }

        public static async Task<Dictionary<string, string>?> GetHttpHeadersAsync(string url)
        {
            try
            {
                TraceLogger.Log($"Getting HTTP headers from {url}");
                var request = new HttpRequestMessage(HttpMethod.Head, url);
                HttpResponseMessage response = await httpClient.SendAsync(request);

                var headers = new Dictionary<string, string>();
                foreach (var header in response.Content.Headers)
                {
                    headers[header.Key] = string.Join(",", header.Value);
                }

                return headers;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error getting HTTP headers: {ex.Message}", Enums.StatusSeverityType.Error);
                return null;
            }
        }

        public static async Task<bool> IsUrlAccessibleAsync(string url)
        {
            try
            {
                TraceLogger.Log($"Checking accessibility of URL: {url}");
                HttpResponseMessage response = await httpClient.GetAsync(url);
                TraceLogger.Log($"URL accessibility check returned status code: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                TraceLogger.Log($"URL is not accessible: {url}", Enums.StatusSeverityType.Warning);
                return false;
            }
        }
    }
}
