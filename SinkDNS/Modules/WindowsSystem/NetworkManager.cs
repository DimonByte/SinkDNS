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
using SinkDNS.Properties;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SinkDNS.Modules.WindowsSystem
{
    internal class NetworkManager
    {
        public static void SaveSelectedNetworkAdapterToSettings(string adapterName)
        {
            TraceLogger.Log($"Saving selected network adapter: {adapterName} to PrimaryNetworkAdapter Settings.");
            //This is the adaptor that will have DNS changed to DNSCRYPT.
            if (!string.IsNullOrEmpty(adapterName))
            {
                //Now test that this adapter is active and has an IPv4 address. If it does not have an IPv4 address, then we should not save it as the primary adapter, because we will not be able to change the DNS settings on it.
                if (IsAdapterActive(adapterName) == OperationalStatus.Up)
                {
                    Settings.Default.PrimaryNetworkAdapter = adapterName;
                    Settings.Default.Save();
                    TraceLogger.Log($"Saved primary network adapter: {adapterName}");
                    if (!BackupDNSConfigOfNetworkAdapter(adapterName))
                    {
                        TraceLogger.LogAndThrowMsgBox($"Failed to backup DNS configuration of adapter {adapterName}.", Enums.StatusSeverityType.Error);
                    }
                }
                else
                {
                    TraceLogger.Log($"Adapter {adapterName} is not active. Cannot save as primary network adapter.", Enums.StatusSeverityType.Error);
                }
            }
        }

        private static OperationalStatus IsAdapterActive(string adapterName)
        {
            TraceLogger.Log($"Checking if adapter {adapterName} is active...");
            if (adapterName != null && !string.IsNullOrEmpty(adapterName))
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.Name == adapterName)
                    {
                        TraceLogger.Log($"Adapter {adapterName} found. Operational status: {networkInterface.OperationalStatus}");
                        return networkInterface.OperationalStatus;
                    }
                }
            }
            TraceLogger.Log($"Adapter {adapterName} not found among network interfaces.", Enums.StatusSeverityType.Error);
            return OperationalStatus.Unknown;
        }

        public static NetworkInterface? GetAdapterInterfaceByName(string adapterName)
        {
            TraceLogger.Log($"Getting adapter by name: {adapterName}");
            if (adapterName != null && !string.IsNullOrEmpty(adapterName))
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.Name == adapterName)
                    {
                        TraceLogger.Log($"Adapter {adapterName} found.");
                        return networkInterface;
                    }
                }
            }
            TraceLogger.Log($"Adapter {adapterName} not found among network interfaces.", Enums.StatusSeverityType.Error);
            return null;
        }

        public static void SetDNSOnSelectedAdapter(
            NetworkInterface adapter,
            bool setDNSToAllAdapters,
            IPAddress primaryIpv4,
            IPAddress secondaryIpv4,
            IPAddress primaryIpv6,
            IPAddress secondaryIpv6)
        {
            //Check if ips are null.
            if (primaryIpv4 == null && primaryIpv6 == null)
            {
                TraceLogger.Log("Both primary IPv4 and primary IPv6 addresses are null. Cannot set DNS.", Enums.StatusSeverityType.Error);
                return;
            }
            TraceLogger.Log($"Setting DNS. Adapter: {adapter?.Name ?? "All"}, AllAdapters: {setDNSToAllAdapters}");

            void BuildCommandsForAdapter(NetworkInterface ni, List<string> commands)
            {
                bool hasIPv6 = IsIPv6EnabledOnAdapter(ni);

                // IPv4
                commands.Add($"netsh interface ipv4 set dnsservers \"{ni.Name}\" static {primaryIpv4}");
                if (secondaryIpv4 != null)
                {
                    commands.Add($"netsh interface ipv4 add dnsservers \"{ni.Name}\" {secondaryIpv4} index=2");
                }

                // IPv6
                if (hasIPv6 && primaryIpv6 != null)
                {
                    commands.Add($"netsh interface ipv6 set dnsservers \"{ni.Name}\" static {primaryIpv6}");
                    if (secondaryIpv6 != null)
                    {
                        commands.Add($"netsh interface ipv6 add dnsservers \"{ni.Name}\" {secondaryIpv6} index=2");
                    }
                }
            }

            if (adapter != null && !setDNSToAllAdapters)
            {
                var commands = new List<string>();

                TraceLogger.Log($"Setting DNS for adapter: {adapter.Name}");
                BuildCommandsForAdapter(adapter, commands);

                commands.Add("ipconfig /flushdns");

                CommandRunner.RunElevatedCommands([.. commands]);
            }
            else if (adapter == null || setDNSToAllAdapters)
            {
                TraceLogger.Log("Setting DNS for all adapters.");

                var commands = new List<string>();

                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    BuildCommandsForAdapter(ni, commands);
                }

                commands.Add("ipconfig /flushdns");

                TraceLogger.Log($"Commands: {string.Join(" | ", commands)}");

                if (CommandRunner.RunElevatedCommands([.. commands]))
                {
                    TraceLogger.Log("Successfully set DNS for all adapters.");
                }
                else
                {
                    TraceLogger.Log("Failed to set DNS for all adapters.", Enums.StatusSeverityType.Error);
                }
            }
            else
            {
                TraceLogger.Log("Adapter was null and SetDNSToAllAdapters is false. Aborting.", Enums.StatusSeverityType.Error);
            }
        }

        public static List<string> GetNetworkAdapterNames()
        {
            List<string> adapterNames = [];
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                adapterNames.Add(networkInterface.Name);
            }
            return adapterNames;
        }

        public static bool RevertToBackupDNS(NetworkInterface adapter, bool SetDNSToAllAdapters)
        {
            //Step1: Get the network adapter.
            //Step2: Get the backup DNS config for that adapter from the backup folder. following file name of "{adapter.Name}_dns_backup.txt"
            //Step2.5: if backup file does not exist, log error and revert to cloudflares and return. And tell user about via msgbox.
            //Step3: read the DNS addresses from that file and set them back to the adapter using netsh commands. 
            //The formatting is:
            //IPv4 Primary: 127.0.0.1
            //IPv4 Secondary: N / A
            //IPv6 Primary: ::1
            //IPv6 Secondary: N / A
            //Step4: If SetDNSToAllAdapters is true, then set the DNS for all adapters to the backup config.
            //This is a bit more complex, because we have to check if each adapter has IPv6 enabled and set the DNS accordingly.

            if (adapter == null && SetDNSToAllAdapters)
            {
                bool allSuccessful = true;
                //Go through each adapter and try to find a backup 
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (Path.Exists(Path.Combine(Settings.Default.BackupFolderLocation, $"{ni.Name}_dns_backup.txt")))
                    {
                        var (ipv4Primary, ipv4Secondary, ipv6Primary, ipv6Secondary) = IOManager.ReadDNSConfigFromBackup(Path.Combine(Settings.Default.BackupFolderLocation, $"{ni.Name}_dns_backup.txt"));
                        TraceLogger.Log($"Read backup DNS config for adapter {ni.Name}: IPv4 Primary: {ipv4Primary}, IPv4 Secondary: {ipv4Secondary}, IPv6 Primary: {ipv6Primary}, IPv6 Secondary: {ipv6Secondary}");
                        //SetDNSOnSelectedAdapter(ni, false, IPAddress.Parse(ipv4Primary), IPAddress.Parse(ipv6Primary));
                        SetDNSOnSelectedAdapter(ni, false,
                            ipv4Primary != "N/A" ? IPAddress.Parse(ipv4Primary) : null,
                            ipv4Secondary != "N/A" ? IPAddress.Parse(ipv4Secondary) : null,
                            ipv6Primary != "N/A" ? IPAddress.Parse(ipv6Primary) : null,
                            ipv6Secondary != "N/A" ? IPAddress.Parse(ipv6Secondary) : null);
                    }
                    else
                    {
                        allSuccessful = false;
                        TraceLogger.Log($"Backup DNS configuration file for adapter {ni.Name} not found. SinkDNS has set your {ni.Name}'s DNS to Cloudflare's DNS as a fallback.", Enums.StatusSeverityType.Warning);
                        SetDNSOnSelectedAdapter(ni, false, IPAddress.Parse("1.1.1.1"), IPAddress.Parse("1.0.0.1"), IPAddress.Parse("2606:4700:4700::1111"), IPAddress.Parse("2606:4700:4700::1001"));
                    }
                }
                if (allSuccessful)
                {
                    return true;
                }
                else
                {
                    TraceLogger.Log("One or more adapters did not have backup DNS configuration files. SinkDNS has set those adapters' DNS to Cloudflare's DNS as a fallback.", Enums.StatusSeverityType.Warning);
                    MessageBox.Show($"One or more adapters did not have backup DNS configuration files. SinkDNS has set those adapters' DNS to Cloudflare's DNS as a fallback.", "Backup Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else if (adapter == null && !SetDNSToAllAdapters)
            {
                TraceLogger.Log("No adapter specified and SetDNSToAllAdapters is false. Cannot revert to backup DNS.", Enums.StatusSeverityType.Error);
                return false;
            }
            else if (adapter != null && !SetDNSToAllAdapters)
            {
                if (Path.Exists(Path.Combine(Settings.Default.BackupFolderLocation, $"{adapter.Name}_dns_backup.txt")))
                {
                    var (ipv4Primary, ipv4Secondary, ipv6Primary, ipv6Secondary) = IOManager.ReadDNSConfigFromBackup(Path.Combine(Settings.Default.BackupFolderLocation, $"{adapter.Name}_dns_backup.txt"));
                    TraceLogger.Log($"Read backup DNS config for adapter {adapter.Name}: IPv4 Primary: {ipv4Primary}, IPv4 Secondary: {ipv4Secondary}, IPv6 Primary: {ipv6Primary}, IPv6 Secondary: {ipv6Secondary}");
                    //SetDNSOnSelectedAdapter(adapter, false, IPAddress.Parse(ipv4Primary), IPAddress.Parse(ipv6Primary));
                    try
                    {
                        SetDNSOnSelectedAdapter(adapter, false,
                            ipv4Primary != "N/A" ? IPAddress.Parse(ipv4Primary) : null,
                            ipv4Secondary != "N/A" ? IPAddress.Parse(ipv4Secondary) : null,
                            ipv6Primary != "N/A" ? IPAddress.Parse(ipv6Primary) : null,
                            ipv6Secondary != "N/A" ? IPAddress.Parse(ipv6Secondary) : null);
                    }
                    catch (Exception ex)
                    {
                        TraceLogger.Log($"Error setting DNS from backup for adapter {adapter.Name}: {ex}", Enums.StatusSeverityType.Error);
                        return false;
                    }
                    return true;
                }
                else
                {
                    SetDNSOnSelectedAdapter(adapter, false, IPAddress.Parse("1.1.1.1"), IPAddress.Parse("1.0.0.1"), IPAddress.Parse("2606:4700:4700::1111"), IPAddress.Parse("2606:4700:4700::1001"));
                    //Backup file does not exist. Log error and revert to cloudflares and return. And tell user about via msgbox.
                    MessageBox.Show($"Backup DNS configuration file for adapter {adapter.Name} not found. SinkDNS has set your {adapter.Name}'s DNS to Cloudflare's DNS as a fallback.", "Backup Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            TraceLogger.Log("RevertToBackupDNS function returned false in the wrong position!", Enums.StatusSeverityType.Warning);
            return false;
        }

        private static bool IsIPv6EnabledOnAdapter(NetworkInterface adapter)
        {
            //Check if the system supports IPv6 and then check if the primary network adapter has IPv6 enabled. If the system does not support IPv6, then we should not try to change the DNS settings to use IPv6 addresses, because it will not work and could cause issues for the user.
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                IPInterfaceProperties iPInterfaceProperties = adapter.GetIPProperties();
                if (iPInterfaceProperties != null && iPInterfaceProperties.UnicastAddresses != null)
                {
                    if (iPInterfaceProperties.UnicastAddresses.Any(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6))
                    {
                        TraceLogger.Log($"IPv6 is enabled on adapter: {adapter.Name}");
                        return true;
                    }
                    else
                    {
                        TraceLogger.Log($"IPv6 is not enabled on adapter: {adapter.Name}");
                        return false;
                    }
                }
            }
            TraceLogger.Log("Unable to determine if IPv6 is enabled on the system. Network is not available.", Enums.StatusSeverityType.Error);
            return false;
        }

        public static bool BackupDNSConfigOfNetworkAdapter(string adapterName)
        {
            IEnumerable<NetworkInterface> targets;

            // Determine which adapters to backup
            if (string.IsNullOrEmpty(adapterName))
            {
                TraceLogger.Log("No primary network adapter selected. Backing up all interfaces...", Enums.StatusSeverityType.Warning);
                targets = NetworkInterface.GetAllNetworkInterfaces();
            }
            else
            {
                var adapter = GetAdapterInterfaceByName(adapterName);
                if (adapter == null)
                {
                    TraceLogger.Log($"Primary network adapter '{adapterName}' not found. Cannot backup DNS configuration.", Enums.StatusSeverityType.Error);
                    return false;
                }

                // Ensure the specific adapter is active before trying to backup
                if (adapter.OperationalStatus != OperationalStatus.Up)
                {
                    TraceLogger.Log($"Primary network adapter '{adapterName}' is not active. Cannot backup DNS configuration.", Enums.StatusSeverityType.Error);
                    return false;
                }
                targets = [adapter];
            }

            // Helper to write backup for a single adapter
            bool allSuccessful = true;
            foreach (var adapter in targets)
            {
                try
                {
                    TraceLogger.Log($"Attempting to backup DNS configuration for adapter: {adapter.Name}");
                    IPInterfaceProperties ipProperties = adapter.GetIPProperties();
                    IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                    if (dnsAddresses.Count == 0)
                    {
                        TraceLogger.Log($"No DNS addresses found for adapter {adapter.Name}. Nothing to backup.", Enums.StatusSeverityType.Warning);
                        // If no DNS config exists (e.g. using DHCP only without DNS configured yet), we shouldn't fail the whole backup necessarily,
                        // but for consistency with the "Backup" concept, we'll mark this adapter as failed.
                        allSuccessful = false;
                        continue;
                    }

                    // Separate IPv4 and IPv6 addresses
                    var ipv4Addresses = dnsAddresses
                        .Cast<IPAddress>()
                        .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                        .OrderBy(ip => ip) // Ordering doesn't strictly matter but provides consistency
                        .ToList();

                    var ipv6Addresses = dnsAddresses
                        .Cast<IPAddress>()
                        .Where(ip => ip.AddressFamily == AddressFamily.InterNetworkV6)
                        .OrderBy(ip => ip)
                        .ToList();

                    if (IOManager.BackupDNSConfigToFolder(Path.Combine(Settings.Default.BackupFolderLocation, $"{adapter.Name}_dns_backup.txt"),
                        ipv4Addresses.Count > 0 ? ipv4Addresses[0].ToString() : "N/A", ipv4Addresses.Count > 1 ? ipv4Addresses[1].ToString() : "N/A",
                        ipv6Addresses.Count > 0 ? ipv6Addresses[0].ToString() : "N/A", ipv6Addresses.Count > 1 ? ipv6Addresses[1].ToString() : "N/A"))
                    {
                        TraceLogger.Log($"Successfully backed up DNS configuration for adapter {adapter.Name}.");
                    }
                    else
                    {
                        TraceLogger.Log($"Failed to backup DNS configuration for adapter {adapter.Name}.", Enums.StatusSeverityType.Error);
                        allSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    TraceLogger.Log($"Error backing up DNS config for adapter {adapter.Name}: {ex}", Enums.StatusSeverityType.Error);
                    allSuccessful = false;
                }
            }
            if (allSuccessful)
            {
                TraceLogger.Log($"DNS configuration backup completed successfully for all targeted adapters.");
            }
            else
            {
                TraceLogger.Log($"DNS configuration backup completed with errors.", Enums.StatusSeverityType.Warning);
            }
            return allSuccessful;
        }
    }
}
