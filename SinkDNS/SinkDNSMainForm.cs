using SinkDNS.Modules;
namespace SinkDNS
{
    public partial class SinkDNSMainForm : Form
    {
        public SinkDNSMainForm()
        {
            InitializeComponent();
        }

        private void testBtn_Click(object sender, EventArgs e) //Testing
        {
            var configWriter = new DNSCryptConfigurationWriter("C:\\Program Files\\dnscrypt-proxy\\dnscrypt-proxy.toml"); 
            MessageBox.Show(configWriter.GetSetting("[blocked_names]", "blocked_names_file"));
            configWriter.BackupDNSCryptConfiguration();
        }

        private void SinkDNSMainForm_Load(object sender, EventArgs e)
        {
            IOManager.CreateNecessaryDirectories();
        }
    }
}
