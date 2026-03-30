using System.Reflection;

namespace SinkDNS
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            verLabel.Text = $"SinkDNS \nv{Assembly.GetExecutingAssembly().GetName().Version}";
        }
    }
}
