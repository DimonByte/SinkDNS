//MIT License

//Copyright (c) 2025 Dimon

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

//TODO: SinkDNS will run from localappdata to allow non-admin writes to blocklists and such.

using SinkDNS.Modules;
using SinkDNS.Modules.DNSCrypt;
using SinkDNS.Modules.SinkDNSInternals;
using SinkDNS.Modules.System;

namespace SinkDNS
{
    public partial class SinkDNSManagerForm : Form
    {
        public SinkDNSManagerForm()
        {
            InitializeComponent();
        }
        private void SinkDNSMainForm_Load(object sender, EventArgs e)
        {
            IOManager.CreateNecessaryDirectories();
            NotificationManager.SetContextMenu(MainContextMenuStrip);
            GlobalNotifyIcon.Instance.SetMainForm(this);
            if (ServiceManager.IsDNSCryptRunning())
            {
                //Since DNSCrypt is running, don't show this manager, since this program couldve been started at startup.
                BeginInvoke(new MethodInvoker(delegate
                {
                    Hide();
                }));
                GlobalNotifyIcon.Instance.SetText("SinkDNS - DNSCrypt Running");
            }
            else
            {
                GlobalNotifyIcon.Instance.SetText("SinkDNS - DNSCrypt Stopped");
                Show();
                MessageBox.Show("DNSCrypt is not running. Please start DNSCrypt to use SinkDNS features.", "DNSCrypt Not Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TraceLogger.Log("DNSCrypt is not running. Showing the manager form.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //NotificationManager.ShowNotification("Updating Blocklists", "Downloading and updating blocklists...", Enums.StatusSeverityType.Information);
            //BlocklistManager.DownloadBlocklistsAsync().GetAwaiter().GetResult();
            //GlobalNotifyIcon.Instance.SetIcon(Properties.Resources.WarningIcon);
            NotificationManager.ShowNotification("Stopping DNSCrypt", "Attempting to stop DNSCrypt...", Enums.StatusSeverityType.Information);
            //ServiceManager.StopDnsCrypt();
        }

        private void SinkDNSManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                TraceLogger.Log("SinkDNS Manager Form is closing.");
            }
        }

        private void exitSinkDNSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void restartDNSCryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ServiceManager.RestartDnsCrypt())
            {
                NotificationManager.ShowNotification("DNSCrypt Restarted", "DNSCrypt has been restarted successfully.", Enums.StatusSeverityType.Information);
                TraceLogger.Log("DNSCrypt restarted successfully.");
            }
            else
            {
                NotificationManager.ShowNotification("DNSCrypt Restart Failed", "Attempted to restart DNSCrypt.", Enums.StatusSeverityType.Warning);
                TraceLogger.Log("Attempted to restart DNSCrypt, but servicemanager returned false. Failed.", Enums.StatusSeverityType.Warning);
            }
        }

        private void updateBlocklistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotificationManager.ShowNotification("Updating Blocklists", "Downloading and updating blocklists...", Enums.StatusSeverityType.Information);
            GlobalNotifyIcon.Instance.SetIcon(Properties.Resources.DownloadingIcon);
            BlocklistManager.DownloadBlocklistsAsync().GetAwaiter().GetResult();
            GlobalNotifyIcon.Instance.SetIcon(Properties.Resources.UpdateAvailableIcon);
            if (ServiceManager.RestartDnsCrypt())
            {
                NotificationManager.ShowNotification("Blocklists Updated", "Blocklists have been updated and DNSCrypt restarted successfully.", Enums.StatusSeverityType.Information);
            }
            else
            {
                NotificationManager.ShowNotification("Blocklists Updated", "Blocklists have been updated, but DNSCrypt restart failed.", Enums.StatusSeverityType.Warning);
            }
            GlobalNotifyIcon.Instance.SetIcon(Properties.Resources.SinkDNSIcon);
        }

        private void openManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
    }
}
