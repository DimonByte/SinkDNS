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
    using SinkDNS.Modules.DNSCrypt;
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
            string listName = listType switch
            {
                Enums.ListType.Blocklist => "Blocklists",
                Enums.ListType.Whitelist => "Whitelists",
                Enums.ListType.Both => "Both", // <-- Added the case for 'Both'
                _ => throw new ArgumentOutOfRangeException(nameof(listType), "Unknown list type provided.") // Safety fallback
            };
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
            else if (listType == Enums.ListType.Both)
            {
                DownloadListsAsync(Settings.Default.BlocklistIniLocation, Settings.Default.BlocklistFolderLocation, Settings.Default.CombinedBlocklistFileLocation).GetAwaiter().GetResult();
                DownloadListsAsync(Settings.Default.WhitelistIniLocation, Settings.Default.WhitelistFolderLocation, Settings.Default.CombinedWhitelistFileLocation).GetAwaiter().GetResult();
            }
            else
            {
                TraceLogger.Log($"Invalid list type specified for update: {listType}", Enums.StatusSeverityType.Error);
                return;
            }
            // Read the file as lines (creates an array of strings)
            string[] UserWebsiteWhitelistLines = File.ReadAllLines(Settings.Default.UserWhitelistIniLocation);
            File.AppendAllLines(Settings.Default.CombinedWhitelistFileLocation, UserWebsiteWhitelistLines);
            // Read the file as lines (creates an array of strings)
            string[] UserWebsiteBlocklistLines = File.ReadAllLines(Settings.Default.UserBlocklistIniLocation);
            File.AppendAllLines(Settings.Default.CombinedBlocklistFileLocation, UserWebsiteBlocklistLines);
            if (Settings.Default.RestartDNSCryptAfterUpdatingLists)
            {
                bool RestartResult = DnsCryptServiceManager.RestartDnsCrypt();
                TraceLogger.Log($"DNSCrypt restart result after updating {listName}: {RestartResult}", Enums.StatusSeverityType.Information);
                if (RestartResult & !ProblemWhenDownloadingLists)
                {
                    NotificationManager.ShowNotification($"{listName} Updated", $"{listName} have been updated with {File.ReadAllLines(listType == Enums.ListType.Blocklist ? Settings.Default.CombinedBlocklistFileLocation : Settings.Default.CombinedWhitelistFileLocation).Length} unique entries.", Enums.StatusSeverityType.Information);
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
                    NotificationManager.ShowNotification($"{listName} Updated", $"{listName} have been updated with {File.ReadAllLines(listType == Enums.ListType.Blocklist ? Settings.Default.CombinedBlocklistFileLocation : Settings.Default.CombinedWhitelistFileLocation).Length} unique entries and will be applied after a DNSCrypt service restart.", Enums.StatusSeverityType.Information);
                }
                TraceLogger.Log($"Finished {listName} update process. DNSCrypt restart attempted: {RestartResult}, Problem when downloading lists: {ProblemWhenDownloadingLists}", Enums.StatusSeverityType.Information);
                ProblemWhenDownloadingLists = false;
            }
        }

        private static async Task DownloadListsAsync(string IniLocation, string ListFolderLocation, string CombinedListLocation)
        {
            TraceLogger.Log($"Starting download async for INI {IniLocation} | ListFolderLocation: {ListFolderLocation} | CombinedListLocation: {CombinedListLocation}");
            DateTime StartOfBlockList = DateTime.Now;
            if (!File.Exists(IniLocation))
            {
                TraceLogger.LogAndThrowMsgBox($"List configuration file not found: {IniLocation}", Enums.StatusSeverityType.Warning);
                return;
            }
            
            List<string> urls = ReadUrlsFromFile(IniLocation);
            foreach (var url in urls)
            {
                TraceLogger.Log($"Downloading list from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(ListFolderLocation, fileName);
                await DownloadController.DownloadFileAsync(url, filePath).ConfigureAwait(false);
            }
            TraceLogger.Log("Finished downloading lists.");
            IOManager.MergeFiles(ListFolderLocation, CombinedListLocation);
            IOManager.RemoveDuplicates(CombinedListLocation);
            TraceLogger.Log("List update complete. Checking if all files have been updated recently");

            //Possible fix for "This causes IO exception when IOManager attempts to merge the files. Being used by another process."
            //We will check after downloads are complete to delete the files since then we are no longer locked.
            foreach (var file in Directory.GetFiles(ListFolderLocation))
            {
                DateTime lastWriteTime = File.GetLastWriteTime(file);
                if (lastWriteTime < StartOfBlockList)
                {
                    TraceLogger.Log($"Deleting {file} since it was not written to during downloadlistasync. (LastWriteTime is less than StartOfBlockListTime)");
                    File.Delete(file);
                }
                else
                {
                    TraceLogger.Log($"List file {file} was updated successfully. Last write time: {lastWriteTime}");
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
