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
    //This will manage the block lists for SinkDNS, including downloading, updating, and parsing them.
    //There will be a list of the block lists that are the most popular on this repo that SinkDNS references.
    //BlockListCompression as well, that will remove any # comments and blank lines from the block lists to reduce their size.
    public static class BlocklistManager
    {
        private static readonly string BlocklistFolder = "hostfiles/blocklist";
        private static readonly string WhitelistFolder = "hostfiles/whitelist";
        private static readonly string BlocklistIni = "config/blocklist.ini";
        private static readonly string WhitelistIni = "config/whitelist.ini";
        private static readonly string UserBlocklistIni = "config/userblocklist.ini";
        private static readonly string UserWhitelistIni = "config/userwhitelist.ini";
        private static readonly string CombinedBlocklistFile = "hostfiles/blocklist/combined-blocklist.txt";
        private static readonly string CombinedWhitelistFile = "hostfiles/whitelist/combined-whitelist.txt";

        public static async Task DownloadBlocklistsAsync()
        {
            if (!File.Exists(BlocklistIni))
            {
                TraceLogger.Log($"Blocklist configuration file not found: {BlocklistIni}", Enums.StatusSeverityType.Warning);
                return;
            }

            var urls = ReadUrlsFromFile(BlocklistIni);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading blocklist from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(BlocklistFolder, fileName);
                await DownloadManager.DownloadFileAsync(url, filePath).ConfigureAwait(false);
            }
            TraceLogger.Log("Finished downloading blocklists.");
        }
        public static async Task DownloadWhitelistsAsync()
        {
            if (!File.Exists(WhitelistIni))
            {
                TraceLogger.Log($"Whitelist configuration file not found: {WhitelistIni}", Enums.StatusSeverityType.Warning);
                return;
            }

            var urls = ReadUrlsFromFile(WhitelistIni);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading whitelist from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(WhitelistFolder, fileName);
                await DownloadManager.DownloadFileAsync(url, filePath);
            }
            TraceLogger.Log("Finished downloading whitelists.");
        }

        public static void AddToUserBlocklist(string domain)
        {
            TraceLogger.Log($"Adding domain to user blocklist: {domain}");
            AddToIniFile(UserBlocklistIni, domain);
        }

        public static void AddToUserWhitelist(string domain)
        {
            TraceLogger.Log($"Adding domain to user whitelist: {domain}");
            AddToIniFile(UserWhitelistIni, domain);
        }

        public static void MergeBlocklists()
        {
            TraceLogger.Log("Merging blocklist files...");
            MergeFiles(BlocklistFolder, CombinedBlocklistFile);
        }

        public static void MergeWhitelists()
        {
            TraceLogger.Log("Merging whitelist files...");
            MergeFiles(WhitelistFolder, CombinedWhitelistFile);
        }
        public static void ClearBlocklists()
        {
            TraceLogger.Log("Clearing blocklist files...");
            ClearFiles(BlocklistFolder);
        }

        public static void ClearWhitelists()
        {
            TraceLogger.Log("Clearing whitelist files...");
            ClearFiles(WhitelistFolder);
        }

        public static bool IsBlocked(string domain)
        {
            if (!File.Exists(CombinedBlocklistFile))
                return false;

            var lines = File.ReadAllLines(CombinedBlocklistFile);
            return lines.Any(line =>
                !string.IsNullOrWhiteSpace(line) &&
                !line.StartsWith("#") &&
                line.Contains(domain));
        }

        public static bool IsWhitelisted(string domain)
        {
            if (!File.Exists(CombinedWhitelistFile))
                return false;

            var lines = File.ReadAllLines(CombinedWhitelistFile);
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

        private static void AddToIniFile(string iniFilePath, string domain)
        {
            var directory = Path.GetDirectoryName(iniFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            File.AppendAllText(iniFilePath, $"{domain}{Environment.NewLine}");
        }

        private static void MergeFiles(string sourceFolder, string outputFile)
        {
            // Delete existing combined file, we don't want that mess to happen...
            TraceLogger.Log($"Creating combined file: {outputFile}");
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            var files = Directory.GetFiles(sourceFolder, "*.txt");
            if (files.Length == 0)
            {
                TraceLogger.Log($"No files found to merge in {sourceFolder}", Enums.StatusSeverityType.Warning);
                return;
            }

            using var writer = new StreamWriter(outputFile);
            foreach (var file in files)
            {
                TraceLogger.Log($"Merging file: {file}");
                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            TraceLogger.Log($"Finished merging files into: {outputFile}");
        }

        private static void ClearFiles(string folder)
        {
            var files = Directory.GetFiles(folder, "*.txt");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Error deleting file {file}: {ex.Message}", Enums.StatusSeverityType.Error);
                }
            }
            TraceLogger.Log($"Cleared all files in folder: {folder}");
        }
    }
}