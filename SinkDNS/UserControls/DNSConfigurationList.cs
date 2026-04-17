using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Properties;
using SinkDNS.Modules.DNSCrypt;
using System.Data;
using SinkDNS.Modules.WindowsSystem;

namespace SinkDNS.UserControls
{
    public partial class DNSConfigurationList : UserControl
    {
        public DNSConfigurationList()
        {
            InitializeComponent();
        }
        private string DNSCryptPath = LocalSystemManager.GetDNSCryptInstallationDirectory(); //Cache result.
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
                List<string> resolverNames = [.. PublicResolverManager.ParseResolverNamesFromMarkdown(markdownContent).OrderBy(name => name)];
                checkedListBox1.Items.AddRange([.. resolverNames]);
                string tomlPath = Path.Combine(DNSCryptPath, "dnscrypt-proxy.toml");
                List<string> selectedResolverNames = PublicResolverManager.GetConfiguredResolversFromToml(File.ReadAllText(tomlPath));
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

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            TraceLogger.Log("Attempting to apply selected DNS resolvers...");
            PublicResolverManager.WriteNewResolversToToml(Path.Combine(DNSCryptPath, "dnscrypt-proxy.toml"), checkedListBox1.CheckedItems.Cast<string>().ToList());
        }

        private void ApplyAndRestartBtn_Click(object sender, EventArgs e)
        {
            TraceLogger.Log("Attempting to apply selected DNS resolvers...");
            PublicResolverManager.WriteNewResolversToToml(Path.Combine(DNSCryptPath, "dnscrypt-proxy.toml"), checkedListBox1.CheckedItems.Cast<string>().ToList());
            TraceLogger.Log("Attempting to restart DNSCrypt service...");
            LocalSystemManager.RestartDnsCrypt();
        }

        private void AddCustomToListBtn_Click(object sender, EventArgs e)
        {
            //Check if text is customStaticTxt is valid (not empty, and not already in the list), if it is valid, add it to the checkedListBox1 and check it.
            if (!string.IsNullOrWhiteSpace(customStaticServerTxt.Text))
            {
                string newResolver = customStaticServerTxt.Text.Trim();
                if (!checkedListBox1.Items.Contains(newResolver))
                {
                    checkedListBox1.Items.Add(newResolver, true);
                    TraceLogger.Log($"Added custom resolver to list and checked it: {newResolver}");
                    MessageBox.Show($"Added custom resolver to list and checked it: {newResolver}", "Custom Resolver Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    TraceLogger.Log($"Custom resolver already exists in the list, not adding: {newResolver}");
                    MessageBox.Show($"Custom resolver already exists in the list, not adding: {newResolver}", "Duplicate Resolver", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
