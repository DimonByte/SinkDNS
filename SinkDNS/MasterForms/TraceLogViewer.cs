using SinkDNS.Properties;

namespace SinkDNS.MasterForms
{
    public partial class TraceLogViewer : Form
    {
        public TraceLogViewer()
        {
            InitializeComponent();
        }

        private void TraceLogViewer_Load(object sender, EventArgs e)
        {
            if (File.Exists(Settings.Default.LogsFolderLocation + "\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".log"))
            {
                LoadLogFile();
                FileSystemWatcher logWatch = new FileSystemWatcher(Settings.Default.LogsFolderLocation, DateTime.Now.ToString("dd-MM-yyyy") + ".log");
                if (logWatch != null)
                {
                    logWatch.Changed += LogWatch_Changed;
                    logWatch.EnableRaisingEvents = true;
                }
            }
            else
            {
                listBox1.Items.Add("No log file found for today.");
                listBox1.Items.Add("Expected location: " + Settings.Default.LogsFolderLocation + "\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".log");
                listBox1.Items.Add("Please ensure that the application has permission to write logs to this location and that logging is enabled in the application settings.");
            }
        }

        private void LoadLogFile()
        {
            string[] logEntries = File.ReadAllLines(Settings.Default.LogsFolderLocation + "\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".log");
            //listBox1.Items.AddRange(logEntries);
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(logEntries);
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                });
            }
            catch { } //No need to catch this is poorly made anyway.
        }

        private void LogWatch_Changed(object sender, FileSystemEventArgs e)
        {
            LoadLogFile();
        }
    }
}
