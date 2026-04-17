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

using SinkDNS.Modules.WindowsSystem;
using System.Net;
using System.Text.RegularExpressions;

namespace SinkDNS.Modules.SinkDNSInternals
{
    // This will manage updates for SinkDNS, and DNSCrypt, including checking for new versions and downloading them via the DownloadManager.
    internal class UpdateManager
    {
        //Static IsBlockListUpdateAvailable(BlocklistName As String) As Boolean
        //If a new version of the blocklist is available, return true, else return false.
        //Use DownloadManager to download the new blocklist if available.
        //If Local Blocklist doesn't match Remote Blocklist version, return true.
        //If Local blocklist doesn't exist, return true to force download.
        //InternetManager.LoadRemoteBlocklistVersion(BlocklistName) //Get the remote version of the blocklist.
        //LocalBlocklistVersion = FileManager.GetLocalBlocklistVersion(BlocklistName) //Get the local version of the blocklist.

        //Static IsSinkDNSUpdateAvailable() As Boolean
        //If a new version of SinkDNS is available, return true, else return false.

        //Static IsDNSCryptProxyUpdateAvailable() As Boolean
        //If a new version of DNSCrypt is available, return true, else return false.
        //How this will work:
        //Get where the DNSCryptProxy executable is located currently via the ServiceManager.GetDNSCryptInstallationDirectory(true) method.
        //Connect via HTTPS to the github releases page for DNSCryptProxy and check the latest release version.
        //Run --version on the local DNSCryptProxy executable to get the local version. So it would be the ServiceManager.GetDNSCryptInstallationDirectory(true) + "dnscrypt-proxy.exe --version" command.
        //Compare the two versions, if the remote version is newer than the local version, return true, else return false.

        public static bool IsDNSCryptProxyUpdateAvailable()
        {
            //Step 1: Get the installation directory of DNSCryptProxy, including the executable path, using the ServiceManager.GetDNSCryptInstallationDirectory(true) method.
            string dnsCryptInstallationDirectory = LocalSystemManager.GetDNSCryptInstallationDirectory(true);
            if (string.IsNullOrEmpty(dnsCryptInstallationDirectory))
            {
                TraceLogger.Log("Unable to get installation directory of DNSCryptProxy. Update check failure.", Enums.StatusSeverityType.Error);
                return false;
            }
            if (!dnsCryptInstallationDirectory.Contains(".exe"))
            {
                TraceLogger.Log("Unable to get executable of DNSCryptProxy. Update check failure.", Enums.StatusSeverityType.Error);
                return false;
            }
            //Step 2: Now the fun part, get latest version number from github releases for https://github.com/DNSCrypt/dnscrypt-proxy/releases/tag/

            string latestTag = GetLatestReleaseTag("DNSCrypt/dnscrypt-proxy");
            if (string.IsNullOrEmpty(latestTag))
            {
                TraceLogger.Log("Failed to determine latest dnscrypt-proxy release.", Enums.StatusSeverityType.Warning);
                return false;
            }
            string latestVersion = latestTag.StartsWith("v", StringComparison.OrdinalIgnoreCase) ? latestTag[1..] : latestTag;

            TraceLogger.Log($"Latest dnscrypt-proxy release tag: {latestTag} (normalized: {latestVersion})");

            // Step 3: Run --version on the local DNSCryptProxy executable to get the local version. ServiceManager.GetDNSCryptInstallationDirectory(true) + "dnscrypt-proxy.exe --version" command.
            string localDNSCryptVer = CommandRunner.RunCommand($"\"{dnsCryptInstallationDirectory}\" --version");
            TraceLogger.Log($"Local dnscrypt-proxy version output: {localDNSCryptVer}");
            //Compare! If the remote version is newer than the local version, return true, else return false. Simples!
            if (Version.TryParse(latestVersion, out var latestVer) && Version.TryParse(localDNSCryptVer, out var localVer))
            {
                if (latestVer > localVer)
                {
                    TraceLogger.Log($"A newer version of dnscrypt-proxy is available: {latestVer} (local: {localVer})");
                    return true;
                }
                else
                {
                    TraceLogger.Log($"dnscrypt-proxy is up to date. Latest: {latestVer}, Local: {localVer}");
                    return false;
                }
            }
            else
            {
                TraceLogger.Log($"Failed to parse version numbers. Latest tag: '{latestTag}' (normalized: '{latestVersion}'), Local version output: '{localDNSCryptVer}'", Enums.StatusSeverityType.Warning);
                return false;
            }
        }

        // Fetches the latest release tag for a GitHub repo using the "releases/latest" redirect.
        // ownerRepo should be "Owner/Repo", e.g. "DNSCrypt/dnscrypt-proxy".
        private static string GetLatestReleaseTag(string ownerRepo)
        {
            try
            {
                TraceLogger.Log($"Checking latest release for {ownerRepo}...");
                using HttpClient http = new(new HttpClientHandler { AllowAutoRedirect = false });
                http.DefaultRequestHeaders.UserAgent.ParseAdd("SinkDNS-Updater/1.0 (+https://example/)");

                var url = $"https://github.com/{ownerRepo}/releases/latest";
                var resp = http.GetAsync(url).GetAwaiter().GetResult();
                TraceLogger.Log($"HTTP response for {url}: {(int)resp.StatusCode} {resp.StatusCode}");
                // Check for redirect location (preferred, reliable)
                if (resp.StatusCode == HttpStatusCode.Found ||
                    resp.StatusCode == HttpStatusCode.Redirect ||
                    resp.StatusCode == HttpStatusCode.MovedPermanently ||
                    resp.StatusCode == HttpStatusCode.SeeOther ||
                    resp.StatusCode == HttpStatusCode.TemporaryRedirect ||
                    resp.StatusCode == HttpStatusCode.PermanentRedirect)
                {
                    var loc = resp.Headers.Location;
                    if (loc != null)
                    {
                        TraceLogger.Log($"Redirect location: {loc}");
                        // The redirect typically ends with "/releases/tag/vX.Y.Z" -> we take the last segment
                        var segments = loc.Segments.Select(s => s.Trim('/')).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                        if (segments.Length > 0)
                        {
                            var tag = segments.Last();
                            TraceLogger.Log($"Extracted tag from redirect: {tag}");
                            return tag;
                        }
                    }
                }
                TraceLogger.Log($"No redirect found for {url}, status code: {(int)resp.StatusCode} {resp.StatusCode}. Attempting HTML parsing fallback.");
                // Fallback: some servers may not redirect; parse the HTML for /releases/tag/<tag>
                var html = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var m = Regex.Match(html, @"/releases/tag/([^""'\s<>]+)");
                if (m.Success && m.Groups.Count > 1)
                {
                    TraceLogger.Log($"Extracted tag from HTML: {m.Groups[1].Value}");
                    return m.Groups[1].Value.Trim('/');
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"GetLatestReleaseTag error for {ownerRepo}: {ex}", Enums.StatusSeverityType.Error);
            }
            TraceLogger.Log($"Failed to get latest release tag for {ownerRepo}.", Enums.StatusSeverityType.Error);
            return "";
        }
    }
}
