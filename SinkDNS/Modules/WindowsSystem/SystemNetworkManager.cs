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
using System.Net.NetworkInformation;

namespace SinkDNS.Modules.WindowsSystem
{
    //This will change the DNS settings on the system to the DNSCrypt local 127.0.0.1 address.
    //But also will save the previous DNS settings to be restored later if the user wants to disable SinkDNS.
    //And will get list of adapters. So basically a system network manager.
    internal class SystemNetworkManager
    {
        public static void SaveSelectedNetworkAdapter(string adapterName)
        {
            //This is the adaptor that will have DNS changed to DNSCRYPT.
            //Check first that this is a valid adapter name and that it is currently active. If it is not active, then we should not save it as the primary adapter, because we will not be able to change the DNS settings on it.
            if (adapterName != null)
            {
                //Now test that this adapter is active and has an IPv4 address. If it does not have an IPv4 address, then we should not save it as the primary adapter, because we will not be able to change the DNS settings on it.
                if (!string.IsNullOrEmpty(adapterName))
                {
                    if (IsAdapterActive(adapterName) == OperationalStatus.Up)
                    {
                        Settings.Default.PrimaryNetworkAdapter = adapterName;
                        Settings.Default.Save();
                        TraceLogger.Log($"Saved primary network adapter: {adapterName}");
                        if (IOManager.BackupDNSConfigOfPrimaryNetworkAdapter(adapterName) == null)
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
        }

        private static OperationalStatus IsAdapterActive(string adapterName)
        {
            //Does this adapter exist and is it active? We can check this by looking at the network interfaces on the system and seeing if there is one that matches the adapter name and is currently up.
            if (adapterName != null)
            {
                if (!string.IsNullOrEmpty(adapterName))
                {
                    var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (var networkInterface in networkInterfaces)
                    {
                        if (networkInterface.Name == adapterName)
                        {
                            TraceLogger.Log($"Adapter {adapterName} found. Operational status: {networkInterface.OperationalStatus}");
                            return networkInterface.OperationalStatus;
                        }
                    }
                }
            }
            TraceLogger.Log($"Adapter {adapterName} not found among network interfaces.", Enums.StatusSeverityType.Error);
            return OperationalStatus.Unknown;
        }

        public static void SetDNStoDNSCrypt()
        {
            if (Settings.Default.PrimaryNetworkAdapter != null)
            {
                if (IsIPv6EnabledOnSystem())
                {
                    
                }
                else
                {

                }
            }
            else
            {
                TraceLogger.Log("No primary network adapter selected. Cannot set DNS to DNSCrypt.", Enums.StatusSeverityType.Error);
            }
        }

        private static bool IsIPv6EnabledOnSystem()
        {
            //Check if the system supports IPv6 and then check if the primary network adapter has IPv6 enabled. If the system does not support IPv6, then we should not try to change the DNS settings to use IPv6 addresses, because it will not work and could cause issues for the user.
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var networkInterface in networkInterfaces)
                {
                    if (networkInterface.Name == Settings.Default.PrimaryNetworkAdapter)
                    {
                        var ipProperties = networkInterface.GetIPProperties();
                        if (ipProperties.UnicastAddresses.Any(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6))
                        {
                            TraceLogger.Log($"IPv6 is enabled on primary network adapter: {Settings.Default.PrimaryNetworkAdapter}");
                            return true;
                        }
                        else
                        {
                            TraceLogger.Log($"IPv6 is not enabled on primary network adapter: {Settings.Default.PrimaryNetworkAdapter}", Enums.StatusSeverityType.Warning);
                            return false;
                        }
                    }
                }
            }
            TraceLogger.Log("Unable to determine if IPv6 is enabled on the system. Network is not available.", Enums.StatusSeverityType.Error);
            return false;
        }
    }
}