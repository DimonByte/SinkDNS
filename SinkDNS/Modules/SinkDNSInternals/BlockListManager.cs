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

namespace SinkDNS.Modules.SinkDNSInternals
{
    using SinkDNS.Modules.System;
    using SinkDNS.Properties;

    //This will manage the block lists for SinkDNS, including downloading, updating, and parsing them.
    //There will be a list of the block lists that are the most popular on this repo that SinkDNS references.
    //BlockListCompression as well, that will remove any # comments and blank lines from the block lists to reduce their size.
    public static class BlocklistManager
    {
        public static async Task DownloadBlocklistsAsync()
        {
            if (!File.Exists(Settings.Default.BlocklistIni))
            {
                TraceLogger.Log($"Blocklist configuration file not found: {Settings.Default.BlocklistIni}", Enums.StatusSeverityType.Warning);
                return;
            }

            var urls = ReadUrlsFromFile(Settings.Default.BlocklistIni);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading blocklist from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(Settings.Default.BlocklistFolder, fileName);
                await DownloadManager.DownloadFileAsync(url, filePath).ConfigureAwait(false);
            }
            TraceLogger.Log("Finished downloading blocklists.");
            IOManager.MergeFiles(Settings.Default.BlocklistFolder, Settings.Default.CombinedBlocklistFile);
            IOManager.RemoveDuplicates(Settings.Default.CombinedBlocklistFile);
            TraceLogger.Log("Blocklist update complete.");
        }

        public static async Task DownloadWhitelistsAsync()
        {
            if (!File.Exists(Settings.Default.WhitelistIni))
            {
                TraceLogger.Log($"Whitelist configuration file not found: {Settings.Default.WhitelistIni}", Enums.StatusSeverityType.Warning);
                return;
            }

            var urls = ReadUrlsFromFile(Settings.Default.WhitelistIni);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading whitelist from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(Settings.Default.WhitelistFolder, fileName);
                await DownloadManager.DownloadFileAsync(url, filePath);
            }
            TraceLogger.Log("Finished downloading whitelists.");
        }

        public static void AddToUserBlocklist(string domain)
        {
            TraceLogger.Log($"Adding domain to user blocklist: {domain}");
            IOManager.AddToIniFile(Settings.Default.UserBlocklistIni, domain);
        }

        public static void AddToUserWhitelist(string domain)
        {
            TraceLogger.Log($"Adding domain to user whitelist: {domain}");
            IOManager.AddToIniFile(Settings.Default.UserWhitelistIni, domain);
        }

        public static void MergeBlocklists()
        {
            TraceLogger.Log("Merging blocklist files...");
            IOManager.MergeFiles(Settings.Default.BlocklistFolder, Settings.Default.CombinedBlocklistFile);
        }

        public static void MergeWhitelists()
        {
            TraceLogger.Log("Merging whitelist files...");
            IOManager.MergeFiles(Settings.Default.WhitelistFolder, Settings.Default.CombinedWhitelistFile);
        }

        public static void ClearBlocklists()
        {
            TraceLogger.Log("Clearing blocklist files...");
            IOManager.ClearFiles(Settings.Default.BlocklistFolder);
        }

        public static void ClearWhitelists()
        {
            TraceLogger.Log("Clearing whitelist files...");
            IOManager.ClearFiles(Settings.Default.WhitelistFolder);
        }

        public static bool IsBlocked(string domain)
        {
            if (!File.Exists(Settings.Default.CombinedBlocklistFile))
                return false;

            var lines = File.ReadAllLines(Settings.Default.CombinedBlocklistFile);
            return lines.Any(line =>
                !string.IsNullOrWhiteSpace(line) &&
                !line.StartsWith("#") &&
                line.Contains(domain));
        }

        public static bool IsWhitelisted(string domain)
        {
            if (!File.Exists(Settings.Default.CombinedWhitelistFile))
                return false;

            var lines = File.ReadAllLines(Settings.Default.CombinedWhitelistFile);
            return lines.Any(line =>
                !string.IsNullOrWhiteSpace(line) &&
                !line.StartsWith("#") &&
                line.Contains(domain));
        }

        private static List<string> ReadUrlsFromFile(string filePath)
        {
            var urls = new List<string>();
            if (!File.Exists(filePath))
                return urls;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                urls.Add(line.Trim());
            }

            return urls;
        }
    }
}
