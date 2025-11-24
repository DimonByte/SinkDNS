using SinkDNS.Modules;
namespace SinkDNS
{
    public partial class SinkDNSMainForm : Form
    {
        public SinkDNSMainForm()
        {
            InitializeComponent();
        }

        private void testBtn_Click(object sender, EventArgs e)
        {

        }

        private void SinkDNSMainForm_Load(object sender, EventArgs e)
        {
            FileManager.CreateNecessaryDirectories();
        }
    }
}
