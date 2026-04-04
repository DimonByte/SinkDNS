using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.System;
using SinkDNS.Properties;
using System.Diagnostics;
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
                    Modules.Enums.StatusSeverityType.Error);
                return;   // Mutex will be released automatically when process exits
            }
            IOManager.CreateNecessaryDirectoriesAndFiles();
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
            TraceLogger.Log("Creating application main form...");
            Application.Run(new SinkDNSManagerForm());
            TraceLogger.Log("SinkDNS Exiting...");
        }
    }
}