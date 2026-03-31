using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.System;
using SinkDNS.Properties;
using System.Diagnostics;

namespace SinkDNS
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //Check if another instance of SinkDNS is already running, if so, exit this instance.
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                TraceLogger.LogAndThrowMsgBox("Another instance of SinkDNS is already running. This instance will now exit.", Modules.Enums.StatusSeverityType.Error);
                return;
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
            }
            else
            {
                TraceLogger.Log("NotifyIcon is null. NotificationManager.SetNotifyIcon was not called. Notifications calls may fail!", Modules.Enums.StatusSeverityType.Warning);
            }
            GlobalNotifyIcon.Instance.SetIcon(Resources.SinkDNSIcon);
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