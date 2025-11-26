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

using System.Net.Http.Headers;

namespace SinkDNS.Modules.SinkDNSInternals
{
    //This will manage downloads for SinkDNS, DNSCrypt, and BlockLists, including starting, stopping, and monitoring download progress.
    //I'll be honest and say this might not work. This needs testing.
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

        public static async Task<bool> DownloadFileAsync(string url, string localPath)
        {
            try
            {
                if (string.IsNullOrEmpty(localPath))
                {
                    TraceLogger.Log("Local path is null or empty", Enums.StatusSeverityType.Error);
                    return false;
                }

                string? directory = Path.GetDirectoryName(localPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fileStream);
                    return true;
                }
                else
                {
                    TraceLogger.Log($"Download failed with status code: {response.StatusCode}", Enums.StatusSeverityType.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error downloading file: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }

        public static async Task<string?> DownloadStringAsync(string url)
        {
            try
            {
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
                HttpResponseMessage response = await httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
