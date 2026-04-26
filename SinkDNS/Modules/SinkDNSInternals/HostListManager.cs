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

namespace SinkDNS.Modules.SinkDNSInternals
{
    using SinkDNS.Modules.WindowsSystem;
    using SinkDNS.Properties;

    //This will manage the block lists for SinkDNS, including downloading, updating, and parsing them.
    //There will be a list of the block lists that are the most popular on this repo that SinkDNS references.
    //BlockListCompression as well, that will remove any # comments and blank lines from the block lists to reduce their size.
    public static class HostListManager
    {
        private static bool ProblemWhenDownloadingLists = false;
        public static void UpdateLists(Enums.ListType listType)
        {
            string listName = listType == Enums.ListType.Blocklist ? "Blocklists" : "Whitelists";
            NotificationManager.ShowNotification($"Updating {listName}", $"Downloading and updating {listName}...", Enums.StatusSeverityType.Information);
            GlobalNotifyIcon.Instance.SetIcon(Resources.DownloadingIcon);
            if (listType == Enums.ListType.Blocklist)
            {
                DownloadListsAsync(Settings.Default.BlocklistIniLocation, Settings.Default.BlocklistFolderLocation, Settings.Default.CombinedBlocklistFileLocation).GetAwaiter().GetResult();
            }
            else if (listType == Enums.ListType.Whitelist)
            {
                DownloadListsAsync(Settings.Default.WhitelistIniLocation, Settings.Default.WhitelistFolderLocation, Settings.Default.CombinedWhitelistFileLocation).GetAwaiter().GetResult();
            }
            else
            {
                TraceLogger.Log($"Invalid list type specified for update: {listType}", Enums.StatusSeverityType.Error);
                return;
            }
            GlobalNotifyIcon.Instance.SetIcon(Resources.UpdateAvailableIcon);
            if (Settings.Default.RestartDNSCryptAfterUpdatingLists)
            {
                bool RestartResult = LocalSystemManager.RestartDnsCrypt();
                TraceLogger.Log($"DNSCrypt restart result after updating {listName}: {RestartResult}", Enums.StatusSeverityType.Information);
                if (RestartResult & !ProblemWhenDownloadingLists)
                {
                    NotificationManager.ShowNotification($"{listName} Updated", $"{listName} have been updated and DNSCrypt restarted successfully.", Enums.StatusSeverityType.Information);
                }
                else if (!RestartResult & !ProblemWhenDownloadingLists)
                {
                    NotificationManager.ShowNotification($"{listName} Updated", $"{listName} have been updated, but DNSCrypt restart failed.", Enums.StatusSeverityType.Warning);
                }
                else if (RestartResult & ProblemWhenDownloadingLists)
                {
                    NotificationManager.ShowNotification($"{listName} Updated with Issues", $"{listName} update was attempted and DNSCrypt restarted successfully, but there were issues during the download process. Check logs for details.", Enums.StatusSeverityType.Warning);
                }
                else if (!RestartResult & ProblemWhenDownloadingLists)
                 {
                    NotificationManager.ShowNotification($"{listName} Updated with Issues", $"{listName} update was attempted, but DNSCrypt restart failed and there were issues during the download process. Check logs for details.", Enums.StatusSeverityType.Error);
                }
                else
                {
                    NotificationManager.ShowNotification($"{listName} Updated", $"{listName} have been updated and will be applied after a DNSCrypt service restart.", Enums.StatusSeverityType.Information);
                }
                GlobalNotifyIcon.Instance.SetIcon(Resources.SinkDNSIcon);
                TraceLogger.Log($"Finished {listName} update process. DNSCrypt restart attempted: {RestartResult}, Problem when downloading lists: {ProblemWhenDownloadingLists}", Enums.StatusSeverityType.Information);
                ProblemWhenDownloadingLists = false;
            }
        }

        private static async Task DownloadListsAsync(string IniLocation, string ListFolderLocation, string CombinedListLocation)
        {
            DateTime StartOfBlockList = DateTime.Now;
            if (!File.Exists(IniLocation))
            {
                TraceLogger.LogAndThrowMsgBox($"List configuration file not found: {IniLocation}", Enums.StatusSeverityType.Warning);
                return;
            }
            foreach (var file in Directory.GetFiles(ListFolderLocation))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    ProblemWhenDownloadingLists = true;
                    TraceLogger.Log($"Failed to delete old list file: {file}. Download halted. Exception: {ex}", Enums.StatusSeverityType.Error);
                    return;
                }
            }

            List<string> urls = ReadUrlsFromFile(IniLocation);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading list from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(ListFolderLocation, fileName);
                await DownloadManager.DownloadFileAsync(url, filePath).ConfigureAwait(false);
            }
            TraceLogger.Log("Finished downloading lists.");
            IOManager.MergeFiles(ListFolderLocation, CombinedListLocation);
            IOManager.RemoveDuplicates(CombinedListLocation);
            TraceLogger.Log("List update complete. Checking if all files have been updated recently");
            //Check if the files in the blocklist have a update date via using the StartOfBlockList, if the file has been modified before the StartOfBlockList, then it means the file was not updated during this download process, and we should log a warning about it.
            foreach (var file in Directory.GetFiles(ListFolderLocation))
            {
                DateTime lastWriteTime = File.GetLastWriteTime(file);
                if (lastWriteTime < StartOfBlockList)
                {
                    ProblemWhenDownloadingLists = true;
                    TraceLogger.Log($"Warning: List file {file} was not updated during this download process. Check logs if the download process failed on this file. Last write time: {lastWriteTime}", Enums.StatusSeverityType.Warning);
                }
            }
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
