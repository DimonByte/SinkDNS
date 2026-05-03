using SinkDNS.Modules;
using SinkDNS.Modules.DNSCrypt;
using SinkDNS.Modules.SinkDNSInternals;

namespace SinkDNS
{
    public partial class SinkDNSFirstTimeSetup : Form
    {
        public SinkDNSFirstTimeSetup()
        {
            InitializeComponent();
        }

        private void SinkDNSFirstTimeSetup_Load(object sender, EventArgs e)
        {
            //Check if DNSCrypt Service is running. if it isn't throw error.
            if (!DnsCryptServiceManager.IsDNSCryptRunning())
            {
                TraceLogger.LogAndThrowMsgBox("Error\n\nSinkDNS has detected that DNSCrypt isn't running. \n\n\nPlease ensure that the DNSCrypt service is running in services and try running SinkDNS again to continue setup.\nSinkDNS will now close", Enums.StatusSeverityType.Fatal);
            }
        }
    }
}
