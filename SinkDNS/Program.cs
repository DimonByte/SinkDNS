using SinkDNS.Modules.SinkDNSInternals;

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
            ApplicationConfiguration.Initialize();
            Application.Run(new SinkDNSManagerForm());
        }
    }
}