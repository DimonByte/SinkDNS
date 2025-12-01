using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.System;

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
            TraceLogger.Log("Application Starting...");
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            NotificationManager.SetNotifyIcon(GlobalNotifyIcon.Instance.NotifyIcon);
            Application.Run(new SinkDNSManagerForm());
            TraceLogger.Log("Application Exiting...");
        }
    }
}