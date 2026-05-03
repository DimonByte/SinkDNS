//MIT License

//Copyright (c) 2025 - 2026 Dimon

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

using System.Text.Json;
using System.Text.Json.Serialization;
using SinkDNS.Properties;

namespace SinkDNS.Modules.SinkDNSInternals
{
    public class HostListRoot
    {
        [JsonPropertyName("entries")]
        public List<HostListEntry> Entries { get; set; } = [];

        [JsonPropertyName("hostlistdirectoryinfo")]
        public HostListDirectoryInfo DirectoryInfo { get; set; } = new();
    }

    public class HostListEntry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = [];

        [JsonPropertyName("mainMirror")]
        public string MainMirror { get; set; } = string.Empty;

        [JsonPropertyName("alternativeMirrors")]
        public List<string> AlternativeMirrors { get; set; } = [];

        [JsonPropertyName("entries")]
        public int EntryCount { get; set; }

        [JsonPropertyName("formatType")]
        public string FormatType { get; set; } = string.Empty;

        [JsonPropertyName("license")]
        public string License { get; set; } = string.Empty;

        [JsonPropertyName("expiry")]
        public string Expiry { get; set; } = string.Empty;

        [JsonPropertyName("lastModified")]
        public string LastModified { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
    }

    public class HostListDirectoryInfo
    {
        [JsonPropertyName("lastUpdated")]
        public string LastUpdated { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("license")]
        public string License { get; set; } = string.Empty;

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; } = string.Empty;
    }

    public static class HostListDirectory
    {
        private const string JsonUrl = "https://raw.githubusercontent.com/DimonByte/HostlistDirectory/refs/heads/main/Directory.json";

        public static List<HostListEntry> HostListEntries { get; private set; } = [];

        public static HostListDirectoryInfo? DirectoryInfo { get; private set; }

        public static async Task InitializeHostListAsync()
        {
            string directoryPath = Settings.Default.HostListDirectory;
            string filePath = Path.Combine(directoryPath, Settings.Default.HostListDirectoryFile);
            bool needsDownload = false;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (!File.Exists(filePath))
            {
                TraceLogger.Log($"{Settings.Default.HostListDirectoryFile} not found, downloading...");
                needsDownload = true;
            }
            else
            {
                DateTime lastWriteTime = File.GetLastWriteTime(filePath);
                if ((DateTime.Now - lastWriteTime).TotalDays > Settings.Default.CacheExpirationDays)
                {
                    TraceLogger.Log($"{Settings.Default.HostListDirectoryFile} is expired, redownloading...");
                    needsDownload = true;
                }
                else
                {
                    TraceLogger.Log($"{Settings.Default.HostListDirectoryFile} is up to date, loading from disk.");
                }
            }
            if (needsDownload)
            {
                try
                {
                    await DownloadController.DownloadFileAsync(JsonUrl, filePath);
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Failed to download hostlist: {ex.Message}", Enums.StatusSeverityType.Error);
                }
            }
            // 4. Parse the JSON
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = await File.ReadAllTextAsync(filePath);
                    JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
                    var root = JsonSerializer.Deserialize<HostListRoot>(jsonContent, options);

                    if (root != null)
                    {
                        HostListEntries = root.Entries;
                        DirectoryInfo = root.DirectoryInfo;
                        TraceLogger.Log($"Successfully loaded {HostListEntries.Count} hostlist entries.");
                    }
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Error parsing hostlist JSON: {ex.Message}", Enums.StatusSeverityType.Error);
                }
            }
            else
            {
                TraceLogger.Log("Hostlist file could not be found after download attempt.", Enums.StatusSeverityType.Error);
            }
        }
    }
}
