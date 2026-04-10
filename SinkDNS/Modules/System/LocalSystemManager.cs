//MIT License

//Copyright (c) 2026 Dimon

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
using SinkDNS.Properties;
using System.ServiceProcess;

namespace SinkDNS.Modules.System
{
    //This will manage and monitor DNSCrypt as a service. It will start, stop, and restart the service as needed. Including checking its status.
    internal class LocalSystemManager
    {
        private const string DnsCryptServiceName = "dnscrypt-proxy";

        public static bool IsDNSCryptRunning()
        {
            ServiceController? serviceController = null;

            try
            {
                serviceController = new ServiceController(DnsCryptServiceName);
                return serviceController.Status == ServiceControllerStatus.Running;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Error checking DNSCrypt service status: {ex.ToString()}", Enums.StatusSeverityType.Error);
                return false;
            }
            finally
            {
                try
                {
                    serviceController?.Dispose();
                }
                catch (Exception disposeEx)
                {
                    TraceLogger.Log($"Dispose failed: {disposeEx.ToString()}", Enums.StatusSeverityType.Error);
                }
            }
        }

        public static void IsDNSCryptInstalled()
        {
            //Check if the DNSCryptInstallationLocation + dnscrypt-proxy.exe exists, if not, scan
            if (!File.Exists(Path.Combine(Settings.Default.DNSCryptInstallationLocation, "dnscrypt-proxy.exe")))
            {
                TraceLogger.Log("dnscrypt-proxy executable not found in the configured DNSCryptInstallationLocation! Searching...");
                string? dnsCryptLocation = GetDNSCryptInstallationDirectory();
                if (dnsCryptLocation == null)
                {
                    TraceLogger.LogAndThrowMsgBox("dnscrypt-proxy executable not found in the configured DNSCryptInstallationLocation. Please ensure dnscrypt-proxy is installed and the correct location is set in settings. This instance will now exit.", Modules.Enums.StatusSeverityType.Fatal);
                    return;
                }
                else
                {
                    TraceLogger.Log($"dnscrypt-proxy executable found at {dnsCryptLocation}. Updating settings...");
                    Settings.Default.DNSCryptInstallationLocation = dnsCryptLocation;
                    Settings.Default.Save();
                }
            }
            else
            {
                TraceLogger.Log("DNSCryptInstallationLocation valid.");
            }
        }

        public static string ?GetDNSCryptInstallationDirectory(bool includeExecutablePath = false)
        {
            TraceLogger.Log("Getting DNSCrypt installation directory...");
            if (Settings.Default.DNSCryptInstallationLocation != null && Directory.Exists(Settings.Default.DNSCryptInstallationLocation))
            {
                TraceLogger.Log($"Found DNSCrypt installation directory in settings: {Settings.Default.DNSCryptInstallationLocation}");
                if (includeExecutablePath)
                {
                    return GetDNSCryptExecutablePath(Settings.Default.DNSCryptInstallationLocation);
                }
                else
                {
                    return Settings.Default.DNSCryptInstallationLocation;
                }
            }
            TraceLogger.Log("DNSCrypt installation location not saved in settings - Attempting to get DNSCrypt installation directory from registry...");
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
                                    Settings.Default.DNSCryptInstallationLocation = Path.GetDirectoryName(exePath);
                                    if (includeExecutablePath)
                                    {
                                        return exePath;
                                    }
                                    else
                                    {
                                        return Path.GetDirectoryName(exePath);
                                    }
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
                            Settings.Default.DNSCryptInstallationLocation = path;
                            TraceLogger.Log($"Found DNSCrypt installation directory at: {path}");
                            return path;
                        }
                    }

                    string programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "DNSCrypt");
                    if (Directory.Exists(programDataPath))
                    {
                        Settings.Default.DNSCryptInstallationLocation = programDataPath;
                        TraceLogger.Log($"Found DNSCrypt installation directory at: {programDataPath}");
                        return programDataPath;
                    }
                    TraceLogger.Log("DNSCrypt installation directory not found!", Enums.StatusSeverityType.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.LogAndThrowMsgBox($"Error getting DNSCrypt installation directory: {ex.ToString()}", Enums.StatusSeverityType.Error);
                return null;
            }
        }
        private static string? GetDNSCryptExecutablePath(string installDir)
        {
            if (installDir != null)
            {
                string exePath = Path.Combine(installDir, "dnscrypt-proxy.exe");
                if (File.Exists(exePath))
                {
                    return exePath;
                }
                else
                {
                    //Search for any .exe in the directory that contains "dnscrypt" in the name, just in case.
                    foreach (string file in Directory.GetFiles(installDir, "*.exe"))
                    {
                        if (Path.GetFileName(file).ToLower().Contains("dnscrypt"))
                        {
                            TraceLogger.Log($"Found executable at: {file}");
                            //Run version command check to confirm it's the correct executable, if exits wrongly then skip it.
                            string commandResult = CommandRunner.RunCommand($"{file} --version");
                            if (commandResult == null | commandResult == String.Empty)
                            {
                                TraceLogger.Log($"Executable at {file} did not return version info, skipping.", Enums.StatusSeverityType.Warning);
                                continue;
                            }
                            return file;
                        }
                    }
                    TraceLogger.Log($"Could not find dnscrypt-proxy.exe in installation directory: {installDir}", Enums.StatusSeverityType.Error);
                }
            }
            return null;
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
                TraceLogger.Log($"Error checking DNSCrypt service installation: {ex.ToString()}", Enums.StatusSeverityType.Error);
                return false;
            }
        }        

        public static bool StartDnsCrypt()
        {
            try
            {
                if (!CommandRunner.IsRunAsAdmin())
                {
                    return CommandRunner.RunElevatedCommand("net", $"start {DnsCryptServiceName}");
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
                TraceLogger.LogAndThrowMsgBox($"Error starting DNSCrypt service: {ex.ToString()}", Enums.StatusSeverityType.Error);
                return false;
            }
        }

        public static bool StopDnsCrypt()
        {
            try
            {
                if (!CommandRunner.IsRunAsAdmin())
                {
                    return CommandRunner.RunElevatedCommand("net", $"stop {DnsCryptServiceName}");
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
                TraceLogger.LogAndThrowMsgBox($"Error stopping DNSCrypt service: {ex.ToString()}", Enums.StatusSeverityType.Error);
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
                        TraceLogger.Log($"Error monitoring DNSCrypt service: {ex.ToString()}", Enums.StatusSeverityType.Error);
                    }
                    Task.Delay(10000).Wait(); // Check every 10 seconds
                }
            });
        }
        public static bool RestartDnsCrypt(bool checkRestartWarning = false)
        {
            if (checkRestartWarning)
            { // Used for when the user enable query logging, which requires a restart of the service. This is to prevent accidental restarts without warning.
                if (!Settings.Default.DisableDNSCryptRestartWarning)
                {
                    DialogResult result = MessageBox.Show("Restarting the DNSCrypt service will temporarily disrupt your internet connection. Do you want to proceed?", "Restart DNSCrypt Service", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) { 
                        return false;
                    }
                }
            }
            TraceLogger.Log("Attempting restart of DNSCrypt service...");
            try
            {
                if (!CommandRunner.IsRunAsAdmin())
                {
                    TraceLogger.Log("User is not admin. Restarting DNSCrypt service with elevated commands...");

                    // Create array of commands to run in sequence
                    string[] commands = [
                        $"net stop \"{DnsCryptServiceName}\"",
                        $"net start \"{DnsCryptServiceName}\"",
                        "ipconfig /flushdns"
                    ];

                    // Run all commands in a single elevated process
                    bool result = CommandRunner.RunElevatedCommands(commands);

                    if (!result)
                    {
                        TraceLogger.LogAndThrowMsgBox("Failed to restart DNSCrypt service during restart.", Enums.StatusSeverityType.Error);
                        return false;
                    }

                    return true;
                }
                else
                {
                    if (!StopDnsCrypt())
                    {
                        TraceLogger.LogAndThrowMsgBox("Failed to stop DNSCrypt service during restart.", Enums.StatusSeverityType.Error);
                        return false;
                    }
                    Task.Delay(1000).Wait();
                    if (!StartDnsCrypt())
                    {
                        TraceLogger.LogAndThrowMsgBox("Failed to start DNSCrypt service during restart.", Enums.StatusSeverityType.Error);
                        return false;
                    }
                    // Flush DNS cache
                    return CommandRunner.RunElevatedCommand("ipconfig", "/flushdns");
                }
            }
            catch (Exception ex)
            {
                TraceLogger.LogAndThrowMsgBox($"Error restarting DNSCrypt service: {ex.ToString()}", Enums.StatusSeverityType.Error);
                return false;
            }
        }
    }
}