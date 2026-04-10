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

using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.System;
using SinkDNS.Properties;
using static SinkDNS.Modules.Enums;

namespace SinkDNS
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        private static Mutex? _singleInstanceMutex;
        public static bool ManagerFormCurrentPageHasUnsavedChanges { get; set; } = false;
        public static bool firstTimeSetupRequired = false;
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //Check if another instance of SinkDNS is already running, if so, exit this instance.
            bool createdNew;
            _singleInstanceMutex =
                new Mutex(true, "SinkDNS_singleton_mutex", out createdNew);
            
            if (!createdNew)
            {
                // Another instance already holds the mutex
                TraceLogger.LogAndThrowMsgBox(
                    "Another instance of SinkDNS is already running. This instance will now exit.",
                    StatusSeverityType.Error);
                return;   // Mutex will be released automatically when process exits
            }
            IOManager.CreateNecessaryDirectoriesAndFiles();
            LocalSystemManager.IsDNSCryptInstalled();
            TraceLogger.Log("SinkDNS Program Starting...");
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            TraceLogger.ClearExpiredLogs();
            var notifyIcon = GlobalNotifyIcon.Instance.NotifyIcon;
            if (notifyIcon is not null)
            {
                NotificationManager.SetNotifyIcon(notifyIcon);
                GlobalNotifyIcon.Instance.SetIcon(Resources.SinkDNSIcon);
            }
            else
            {
                TraceLogger.Log("NotifyIcon is null. NotificationManager.SetNotifyIcon was not called. Notifications calls may fail!", Modules.Enums.StatusSeverityType.Warning);
            }
            if (!Settings.Default.EnableDiskLogging)
            {
                TraceLogger.Log("Disk logging is disabled in settings by user.");
            }
            if (firstTimeSetupRequired)
            {
                TraceLogger.Log("First time setup is required due to missing files/folders. Opening first time setup form...");
                Application.Run(new SinkDNSFirstTimeSetup());
            }
            else
            {
                TraceLogger.Log("First time setup is not required. Running main manager form.");
                Application.Run(new SinkDNSManagerForm());
            }
            TraceLogger.Log("SinkDNS Exiting...");
        }
    }
}