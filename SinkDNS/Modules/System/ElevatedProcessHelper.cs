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

using SinkDNS.Modules.SinkDNSInternals;
using System.Diagnostics;
using System.Security.Principal;

namespace SinkDNS.Modules.System
{
    public static class ElevatedProcessHelper
    {
        public static bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new(identity);
                TraceLogger.Log($"IsRunAsAdmin check: User={identity.Name}, IsAdmin={principal.IsInRole(WindowsBuiltInRole.Administrator)}");
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                TraceLogger.Log("Failed to determine if running as admin.", Enums.StatusSeverityType.Warning);
                return false;
            }
        }

        public static bool RunElevatedCommands(string[] commands)
        {
            try
            {
                string allCommands = string.Join(";", commands);
                TraceLogger.Log($"Attempting to run elevated commands: {allCommands}");
                ProcessStartInfo startInfo = new()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -WindowStyle Hidden -Command \"{allCommands}\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                using Process? process = Process.Start(startInfo);
                if (process == null)
                {
                    TraceLogger.Log("Failed to start process for elevated commands.", Enums.StatusSeverityType.Error);
                    return false;
                }
                TraceLogger.Log("Waiting for elevated commands to complete...");
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    TraceLogger.Log($"Command failure! Elevated commands exited with exit code: {process.ExitCode}", Enums.StatusSeverityType.Error);
                    return false;
                }
                else
                {
                    TraceLogger.Log("Elevated commands executed successfully. Returned exit code 0.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Failed to run elevated commands: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }

        public static bool RunElevatedCommand(string command, string arguments = "")
        {
            try
            {
                string fullArguments = $"{arguments}";
                TraceLogger.Log($"Attempting to run elevated command: {command} {fullArguments}");
                ProcessStartInfo startInfo = new()
                {
                    FileName = command,
                    Arguments = fullArguments,
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                using Process? process = Process.Start(startInfo);
                if (process == null)
                {
                    TraceLogger.Log("Failed to start process for elevated command.", Enums.StatusSeverityType.Error);
                    return false;
                }
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    TraceLogger.Log($"Elevated command exited with code: {process.ExitCode}", Enums.StatusSeverityType.Error);
                }
                else
                {
                    TraceLogger.Log("Elevated command executed successfully.");
                }
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Failed to run elevated command: {ex.Message}", Enums.StatusSeverityType.Error);
                return false;
            }
        }
    }
}