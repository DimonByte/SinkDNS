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

using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.WindowsSystem;
using SinkDNS.Properties;
using System.Text.RegularExpressions;

namespace SinkDNS.Modules.DNSCrypt.Data
{
    // This will load the public resolvers from DNSCrypt and return a list of them.
    internal class ResolverManager
    {

        public static async Task<string> DownloadPublicResolversAsync()
        {
            string? markdownContent = null;
            if (!File.Exists(Path.Combine(Settings.Default.ResolversFolderLocation, Settings.Default.PublicResolversFile)))
            {
                TraceLogger.Log($"{Settings.Default.PublicResolversFile} not found on disk in {Settings.Default.ResolversFolderLocation}, downloading...");
                await DownloadController.DownloadFileAsync("https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md", Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
                markdownContent = File.ReadAllText(Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
            }
            else
            {
                DateTime creationDate = File.GetLastWriteTime(Path.Combine(Settings.Default.ResolversFolderLocation, Settings.Default.PublicResolversFile));
                if ((DateTime.Now - creationDate).TotalDays > Settings.Default.CacheExpirationDays)
                {
                    TraceLogger.Log($"{Settings.Default.PublicResolversFile} is older than {Settings.Default.CacheExpirationDays} days, redownloading...");
                    await DownloadController.DownloadFileAsync("https://raw.githubusercontent.com/DNSCrypt/dnscrypt-resolvers/master/v3/public-resolvers.md", Path.Combine(Settings.Default.ResolversFolderLocation, "public-resolvers.md"));
                    markdownContent = File.ReadAllText(Path.Combine(Settings.Default.ResolversFolderLocation, Settings.Default.PublicResolversFile));
                }
                else
                {
                    TraceLogger.Log($"{Settings.Default.PublicResolversFile} is up to date (not older than 7 days), loading from disk...");
                    markdownContent = File.ReadAllText(Path.Combine(Settings.Default.ResolversFolderLocation, Settings.Default.PublicResolversFile));
                }
            }
            if (markdownContent == null)
            {
                TraceLogger.Log("Failed to load public resolver list: Content is null", Modules.Enums.StatusSeverityType.Error);
                return null;
            }
            markdownContent = markdownContent.Replace("\r\n", "\n").Replace("\r", "\n"); // Normalize line endings
            return markdownContent;
        }

        public static List<string> ParseResolverNamesFromMarkdown(string? markdownContent)
        {
            //Format of the markdown is:
            //## TEST DNS

            //Non-filtering, No-logging, DNSSEC DoH operated by someone.
            //Homepage: https://TEST.COm

            //sdns://REMOVED
            //sdns://REMOVED
            List<string>? resolverNames = null;
            foreach (string line in markdownContent?.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries) ?? [])
            {
                if (line.StartsWith("## "))
                {
                    string resolverName = line[3..].Trim();
                    resolverNames ??= [];
                    resolverNames.Add(resolverName);
                }
            }
            TraceLogger.Log($"Total resolver names parsed from markdown: {resolverNames?.Count ?? 0}");
            return resolverNames ?? [];
        }
        public static List<string> GetConfiguredResolversFromToml(string tomlContent)
        {
            List<string> configuredResolvers = [];
            foreach (string line in tomlContent.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
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
                        string resolversList = line[startIndex..endIndex];
                        string[] resolvers = resolversList.Split([','], StringSplitOptions.RemoveEmptyEntries);
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
            configuredResolvers = [.. configuredResolvers.Select(r => r.Trim().Trim('"', '\'')).Where(r => !string.IsNullOrEmpty(r))];
            TraceLogger.Log("Resolver List:" + string.Join(", ", configuredResolvers));
            return configuredResolvers;
        }

        public static void WriteNewResolversToToml(string tomlFilePath, List<string> selectedResolvers)
        {
            TraceLogger.Log("Writing new resolvers to toml file: " + tomlFilePath);
            TraceLogger.Log("Selected resolvers: " + string.Join(", ", selectedResolvers));
            string tomlContent = File.ReadAllText(tomlFilePath);
            string newServerNamesLine = $"server_names = [{string.Join(", ", selectedResolvers.Select(r => $"\"{r}\""))}]";
            // Ensure the regex only matches the server_names line and not disabled_server_names.
            // We use a negative lookahead at the start of the line to skip any line that contains "disabled_server_names".
            TraceLogger.Log("New server_names line: " + newServerNamesLine);
            string pattern = @"^(?<indent>\s*)(?!.*disabled_server_names)server_names\s*=\s*\[.*?\].*$";
            string replacement = "${indent}" + newServerNamesLine;
            string updatedTomlContent = Regex.Replace(tomlContent, pattern, replacement, RegexOptions.Multiline);
            //File.WriteAllText(tomlFilePath, updatedTomlContent);
            if (string.IsNullOrEmpty(updatedTomlContent))
                {
                TraceLogger.Log("An error has occurred when processing new public resolvers to write to the DNSCrypt config file: Updated TOML content is empty. Aborting write operation.", Enums.StatusSeverityType.Error);
                return;
            }
            if (CommandRunner.IsRunAsAdmin())
            {
                TraceLogger.Log("Running with admin privileges, writing directly to file.");
                File.WriteAllText(tomlFilePath, updatedTomlContent);
            }
            else
            {
                TraceLogger.Log("Not running with admin privileges, attempting to write with elevation.");
                bool success = CommandRunner.SaveToFileWithElevation(tomlFilePath, updatedTomlContent);
                if (!success)
                {
                    TraceLogger.Log("Failed to write new resolvers to toml file with elevation.", Enums.StatusSeverityType.Error);
                }
                else
                {
                    TraceLogger.Log("Successfully wrote new resolvers to toml file with elevation.");
                }
            }
        }
    }
}
