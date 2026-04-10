using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.System;
using SinkDNS.Properties;
using System.Data;

namespace SinkDNS.UserControls
{
    public partial class DNSConfigurationList : UserControl
    {
        public DNSConfigurationList()
        {
            InitializeComponent();
        }

        private async void DNSConfigurationList_Load(object sender, EventArgs e)
        {
            TraceLogger.Log("Loading DNS Configuration List...");
            string? markdownContent = null;
            try
            {
                //string? markdownContent = await DownloadManager.DownloadStringAsync("https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md");
                //Check if the file "public-resolvers.md" exists in the Settings.Default.ResolversFolderLocation, if not download it with DownloadFileAsync.
                if (!File.Exists(Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md")))
                {
                    TraceLogger.Log($"public-resolvers.md not found on disk in {Settings.Default.ResolversFolderLocation}, downloading...");
                    await DownloadManager.DownloadFileAsync("https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md", Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
                    markdownContent = File.ReadAllText(Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
                }
                else
                {
                    DateTime creationDate = File.GetCreationTime(Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
                    if ((DateTime.Now - creationDate).TotalDays > 7)
                    {
                        TraceLogger.Log($"public-resolvers.md is older than 7 days, redownloading...");
                        await DownloadManager.DownloadFileAsync("https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md", Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
                    }
                    else
                    {
                        TraceLogger.Log($"public-resolvers.md is up to date (not older than 7 days), loading from disk...");
                        markdownContent = File.ReadAllText(Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
                    }
                }
                if (markdownContent == null)
                {
                    TraceLogger.Log("Failed to load resolver list: Content is null");
                    return;
                }
                markdownContent = markdownContent.Replace("\r\n", "\n").Replace("\r", "\n"); // Normalize line endings
                List<string> resolverNames = ParseResolverNamesFromMarkdown(markdownContent).OrderBy(name => name).ToList();
                checkedListBox1.Items.AddRange(resolverNames.ToArray());
                string tomlPath = Path.Combine(LocalSystemManager.GetDNSCryptInstallationDirectory(), "dnscrypt-proxy.toml");
                List<string> selectedResolverNames = GetConfiguredResolversFromToml(File.ReadAllText(tomlPath));
                SetCheckBoxesOnListbox(selectedResolverNames);
            }
            catch (Exception ex)
            {
                TraceLogger.Log("Exception occurred while loading DNS resolver list: " + ex.ToString(), Modules.Enums.StatusSeverityType.Error);
            }
        }

        private void SetCheckBoxesOnListbox(List<string> selectedResolverNames)
        {
            TraceLogger.Log("Setting checkboxes on listbox based on selected resolver names...");
            TraceLogger.Log($"Number of selected resolver names: {selectedResolverNames.Count}");
            TraceLogger.Log($"Number of items in checkedListBox1: {checkedListBox1.Items.Count}");
            //The checkedListbox1 now has all resolvers, look at all of the items on the checkedListBox1, and if there is an item with the same text as one of the selectedResolverNames, then check that item in the checkedListBox1.
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                string item = checkedListBox1.Items[i].ToString() ?? "";
                if (selectedResolverNames.Contains(item))
                {
                    checkedListBox1.SetItemChecked(i, true);
                    TraceLogger.Log($"Checked resolver in list: {item}");
                }
            }

            TraceLogger.Log("Finished setting checkboxes on listbox.");
        }

        private static List<string> ParseResolverNamesFromMarkdown(string? markdownContent)
        {
            //Format of the markdown is:
            //## TEST DNS

            //Non-filtering, No-logging, DNSSEC DoH operated by someone.
            //Homepage: https://TEST.COm

            //sdns://REMOVED
            //sdns://REMOVED
            List<string>? resolverNames = null;
            foreach (string line in markdownContent?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
            {
                if (line.StartsWith("## "))
                {
                    string resolverName = line.Substring(3).Trim();
                    resolverNames ??= new List<string>();
                    resolverNames.Add(resolverName);
                }
            }
            TraceLogger.Log($"Total resolver names parsed from markdown: {resolverNames?.Count ?? 0}");
            return resolverNames ?? new List<string>();
        }
        private static List<string> GetConfiguredResolversFromToml(string tomlContent)
        {
            List<string> configuredResolvers = new List<string>();
            foreach (string line in tomlContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                //TraceLogger.Log("Checking line in toml: " + line);
                if (line.TrimStart().StartsWith("server_names = ["))
                {
                    TraceLogger.Log("Found server_names line: " + line);
                    int startIndex = line.IndexOf('[') + 1;
                    int endIndex = line.IndexOf(']');
                    if (startIndex > 0 && endIndex > startIndex)
                    {
                        TraceLogger.Log("Extracting resolver names from server_names line.");
                        string resolversList = line.Substring(startIndex, endIndex - startIndex);
                        string[] resolvers = resolversList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string resolver in resolvers)
                        {
                            TraceLogger.Log("Processing resolver entry: " + resolver);
                            string cleanedResolver = resolver.Trim().Trim('"');
                            if (!string.IsNullOrEmpty(cleanedResolver))
                            {
                                TraceLogger.Log("Adding resolver to configured list: " + cleanedResolver);
                                configuredResolvers.Add(cleanedResolver);
                            }
                        }
                    }
                }
            }
            TraceLogger.Log("Total configured resolvers found: " + configuredResolvers.Count);
            configuredResolvers = configuredResolvers.Select(r => r.Trim().Trim('"', '\'')).Where(r => !string.IsNullOrEmpty(r)).ToList();
            TraceLogger.Log("Resolver List:" + string.Join(", ", configuredResolvers));
            return configuredResolvers;
        }
    }
}
