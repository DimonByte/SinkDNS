//MIT License

//Copyright (c) 2026 Dimon

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
    public static class HostListManager
    {
        public static async Task DownloadBlocklistsAsync()
        {
            DateTime StartOfBlockList = DateTime.Now;
            if (!File.Exists(Settings.Default.BlocklistIniLocation))
            {
                TraceLogger.LogAndThrowMsgBox($"Blocklist configuration file not found: {Settings.Default.BlocklistIniLocation}", Enums.StatusSeverityType.Warning);
                return;
            }
            //Delete all blocklist files in the blocklist folder before downloading new ones to ensure we don't have any old blocklists lying around.
            foreach (var file in Directory.GetFiles(Settings.Default.BlocklistFolderLocation))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    //We can't continue if we can't delete blocklists, since it failed here it might be permissions. And we don't want to merge old blocklists with new ones.
                    TraceLogger.Log($"Failed to delete old blocklist file: {file}. Download halted. Exception: {ex.ToString()}", Enums.StatusSeverityType.Error);
                    return;
                }
            }

            var urls = ReadUrlsFromFile(Settings.Default.BlocklistIniLocation);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading blocklist from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(Settings.Default.BlocklistFolderLocation, fileName);
                await DownloadManager.DownloadFileAsync(url, filePath).ConfigureAwait(false);
            }
            TraceLogger.Log("Finished downloading blocklists.");
            IOManager.MergeFiles(Settings.Default.BlocklistFolderLocation, Settings.Default.CombinedBlocklistFileLocation);
            IOManager.RemoveDuplicates(Settings.Default.CombinedBlocklistFileLocation);
            TraceLogger.Log("Blocklist update complete. Checking if all files have been updated recently");
            //Check if the files in the blocklist have a update date via using the StartOfBlockList, if the file has been modified before the StartOfBlockList, then it means the file was not updated during this download process, and we should log a warning about it.
            foreach (var file in Directory.GetFiles(Settings.Default.BlocklistFolderLocation))
            {
                var lastWriteTime = File.GetLastWriteTime(file);
                if (lastWriteTime < StartOfBlockList)
                {
                    TraceLogger.Log($"Warning: Blocklist file {file} was not updated during this download process. Check logs if the download process failed on this file. Last write time: {lastWriteTime}", Enums.StatusSeverityType.Warning);
                }
            }
        }

        public static async Task DownloadWhitelistsAsync()
        {
            if (!File.Exists(Settings.Default.WhitelistIniLocation))
            {
                TraceLogger.LogAndThrowMsgBox($"Whitelist configuration file not found: {Settings.Default.WhitelistIniLocation}", Enums.StatusSeverityType.Warning);
                return;
            }

            var urls = ReadUrlsFromFile(Settings.Default.WhitelistIniLocation);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading whitelist from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(Settings.Default.WhitelistFolderLocation, fileName);
                await DownloadManager.DownloadFileAsync(url, filePath);
            }
            TraceLogger.Log("Finished downloading whitelists.");
        }

        public static void AddToUserBlocklist(string domain)
        {
            TraceLogger.Log($"Adding domain to user blocklist: {domain}");
            IOManager.AddToIniFile(Settings.Default.UserBlocklistIniLocation, domain);
        }

        public static void AddToUserWhitelist(string domain)
        {
            TraceLogger.Log($"Adding domain to user whitelist: {domain}");
            IOManager.AddToIniFile(Settings.Default.UserWhitelistIniLocation, domain);
        }

        public static void MergeBlocklists()
        {
            TraceLogger.Log("Merging blocklist files...");
            IOManager.MergeFiles(Settings.Default.BlocklistFolderLocation, Settings.Default.CombinedBlocklistFileLocation);
        }

        public static void MergeWhitelists()
        {
            TraceLogger.Log("Merging whitelist files...");
            IOManager.MergeFiles(Settings.Default.WhitelistFolderLocation, Settings.Default.CombinedWhitelistFileLocation);
        }

        public static void ClearBlocklists()
        {
            TraceLogger.Log("Clearing blocklist files...");
            IOManager.ClearFiles(Settings.Default.BlocklistFolderLocation);
        }

        public static void ClearWhitelists()
        {
            TraceLogger.Log("Clearing whitelist files...");
            IOManager.ClearFiles(Settings.Default.WhitelistFolderLocation);
        }

        public static bool IsBlocked(string domain)
        {
            if (!File.Exists(Settings.Default.CombinedBlocklistFileLocation))
                return false;

            var lines = File.ReadAllLines(Settings.Default.CombinedBlocklistFileLocation);
            return lines.Any(line =>
                !string.IsNullOrWhiteSpace(line) &&
                !line.StartsWith('#') &&
                line.Contains(domain));
        }

        public static bool IsWhitelisted(string domain)
        {
            if (!File.Exists(Settings.Default.CombinedWhitelistFileLocation))
                return false;

            var lines = File.ReadAllLines(Settings.Default.CombinedWhitelistFileLocation);
            return lines.Any(line =>
                !string.IsNullOrWhiteSpace(line) &&
                !line.StartsWith('#') &&
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
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                    continue;

                urls.Add(line.Trim());
            }

            return urls;
        }
    }
}
