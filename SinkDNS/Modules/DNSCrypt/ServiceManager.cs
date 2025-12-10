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

using Microsoft.Win32;
using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.System;
using System.Diagnostics;
using System.ServiceProcess;

namespace SinkDNS.Modules.DNSCrypt
{
    //This will manage and monitor DNSCrypt as a service. It will start, stop, and restart the service as needed. Including checking its status.
    internal class ServiceManager
    {
        private const string DnsCryptServiceName = "dnscrypt-proxy";

        public static bool IsDNSCryptRunning()
        {
            try
            {
                using var serviceController = new ServiceController(DnsCryptServiceName);
                return serviceController.Status == ServiceControllerStatus.Running;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error checking DNSCrypt service status: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }

        public static string GetDNSCryptInstallationDirectory()
        {
            TraceLogger.Log("Attempting to get DNSCrypt installation directory from registry...");
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\dnscrypt-proxy"))
                {
                    if (key != null)
                    {
                        TraceLogger.Log("Found DNSCrypt service registry key.");
                        var imagePath = key.GetValue("ImagePath") as string;
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            TraceLogger.Log($"Found DNSCrypt ImagePath in registry: {imagePath}");
                            // Extract the directory from the image path
                            // ImagePath is typically something like: "C:\Program Files\DNSCrypt\dnscrypt-proxy.exe" -config "C:\Program Files\DNSCrypt\config.toml"
                            if (imagePath.StartsWith("\"") && imagePath.Contains("\""))
                            {
                                int start = imagePath.IndexOf('"') + 1;
                                int end = imagePath.IndexOf('"', start);
                                if (end > start)
                                {
                                    string exePath = imagePath.Substring(start, end - start);
                                    TraceLogger.Log($"Found DNSCrypt executable path: {exePath}");
                                    return Path.GetDirectoryName(exePath);
                                }
                            }
                            else
                            {
                                if (imagePath.Contains("\\"))
                                {
                                    string[] parts = imagePath.Split('\\');
                                    if (parts.Length >= 2)
                                    {
                                        Array.Resize(ref parts, parts.Length - 1);
                                        TraceLogger.Log($"Found DNSCrypt installation directory: {string.Join("\\", parts)}");
                                        return string.Join("\\", parts);
                                    }
                                }
                            }
                        }
                    }
                }
                TraceLogger.Log("Could not find DNSCrypt installation directory from registry. Trying common paths...", Enums.StatusSeverityType.Warning);

                string[] commonPaths = {
                @"C:\Program Files\DNSCrypt",
                @"C:\Program Files (x86)\DNSCrypt",
                @"C:\Program Files\dnscrypt-proxy",
                @"C:\Program Files (x86)\dnscrypt-proxy"
            };

                foreach (string path in commonPaths)
                {
                    if (Directory.Exists(path))
                    {
                        TraceLogger.Log($"Found DNSCrypt installation directory at: {path}");
                        return path;
                    }
                }

                string programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "DNSCrypt");
                if (Directory.Exists(programDataPath))
                {
                    TraceLogger.Log($"Found DNSCrypt installation directory at: {programDataPath}");
                    return programDataPath;
                }
                TraceLogger.Log("DNSCrypt installation directory not found in common paths.", Enums.StatusSeverityType.Error);
                return null;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error getting DNSCrypt installation directory: {ex.Message}", Enums.StatusSeverityType.Error);
                return null;
            }
        }

        public static bool IsDnsCryptInstalled()
        {
            try
            {
                using var service = new ServiceController(DnsCryptServiceName);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error checking DNSCrypt service installation: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }        

        public static bool StartDnsCrypt()
        {
            try
            {
                if (!ElevatedProcessHelper.IsRunAsAdmin())
                {
                    return ElevatedProcessHelper.RunElevatedCommand("net", $"start {DnsCryptServiceName}");
                }
                else
                {
                    using var service = new ServiceController(DnsCryptServiceName);
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        TraceLogger.Log("Starting DNSCrypt service...");
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        return true;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error starting DNSCrypt service: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }

        public static bool StopDnsCrypt()
        {
            try
            {
                if (!ElevatedProcessHelper.IsRunAsAdmin())
                {
                    return ElevatedProcessHelper.RunElevatedCommand("net", $"stop {DnsCryptServiceName}");
                }
                else
                {
                    using var service = new ServiceController(DnsCryptServiceName);
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        TraceLogger.Log("Stopping DNSCrypt service...");
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        return true;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error stopping DNSCrypt service: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }
        //Monitor the service and alert globalnotifyicon if it stops unexpectedly.
        public static void MonitorDnsCryptService()
        {
            Task.Run(() =>
            {
                TraceLogger.Log("Starting DNSCrypt service monitor...");
                while (true)
                {
                    try
                    {
                        using var service = new ServiceController(DnsCryptServiceName);
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            TraceLogger.Log("DNSCrypt service is not running!");
                            GlobalNotifyIcon.Instance.SetText("SinkDNS - DNSCrypt Stopped!");
                            //StartDnsCrypt();
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceLogger.Log($"Error monitoring DNSCrypt service: {ex.Message}", Enums.StatusSeverityType.Error);
                    }
                    Task.Delay(10000).Wait(); // Check every 10 seconds
                }
            });
        }
        public static bool RestartDnsCrypt()
        {
            TraceLogger.Log("Attempting restart of DNSCrypt service...");
            try
            {
                if (!ElevatedProcessHelper.IsRunAsAdmin())
                {
                    TraceLogger.Log("User is not admin. Restarting DNSCrypt service with elevated commands...");

                    // Create array of commands to run in sequence
                    string[] commands = [
                        $"net stop \"{DnsCryptServiceName}\"",
                        $"net start \"{DnsCryptServiceName}\"",
                        "ipconfig /flushdns"
                    ];

                    // Run all commands in a single elevated process
                    bool result = ElevatedProcessHelper.RunElevatedCommands(commands);

                    if (!result)
                    {
                        TraceLogger.Log("Failed to restart DNSCrypt service during restart.", Enums.StatusSeverityType.Error);
                        return false;
                    }

                    return true;
                }
                else
                {
                    if (!StopDnsCrypt())
                    {
                        TraceLogger.Log("Failed to stop DNSCrypt service during restart.", Enums.StatusSeverityType.Error);
                        return false;
                    }
                    Task.Delay(1000).Wait();
                    if (!StartDnsCrypt())
                    {
                        TraceLogger.Log("Failed to start DNSCrypt service during restart.", Enums.StatusSeverityType.Error);
                        return false;
                    }
                    // Flush DNS cache
                    return ElevatedProcessHelper.RunElevatedCommand("ipconfig", "/flushdns");
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error restarting DNSCrypt service: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }
    }
}