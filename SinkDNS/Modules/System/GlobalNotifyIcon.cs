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

using SinkDNS.Modules.SinkDNSInternals;

namespace SinkDNS.Modules.System
{
    public sealed class GlobalNotifyIcon : IDisposable
    {
        private static GlobalNotifyIcon? _instance = null;
        private static readonly Lock _lockObject = new();
        private NotifyIcon? _notifyIcon;
        private bool _disposed = false;
        private Form? _mainForm;

        private GlobalNotifyIcon()
        {
            InitializeNotifyIcon();
        }

        private void InitializeNotifyIcon()
        {
            TraceLogger.Log("Initializing Global NotifyIcon...");
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "SinkDNS",
                Visible = true
            };
            TraceLogger.Log("Global NotifyIcon initialized.");

            var contextMenuStrip = new ContextMenuStrip();
            var exitMenuItem = new ToolStripMenuItem("Exit", null, (sender, e) => Application.Exit());
            contextMenuStrip.Items.Add(exitMenuItem);
            var showMenuItem = new ToolStripMenuItem("Show", null, (sender, e) => ShowMainWindow());
            contextMenuStrip.Items.Insert(0, showMenuItem);
            _notifyIcon.ContextMenuStrip = contextMenuStrip;
            TraceLogger.Log("Context menu for NotifyIcon set.");

            _notifyIcon.DoubleClick += (sender, e) => ShowMainWindow();

            TraceLogger.Log("MouseClick event handler for NotifyIcon set. NotifyIcon Init Complete.");
        }

        public static GlobalNotifyIcon Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        _instance ??= new GlobalNotifyIcon();
                    }
                }
                return _instance;
            }
        }

        public void SetMainForm(Form mainForm)
        {
            TraceLogger.Log($"Setting main form for GlobalNotifyIcon: {mainForm.Name}");
            _mainForm = mainForm;
        }

        // Expose the NotifyIcon for NotificationManager to use
        public NotifyIcon? NotifyIcon => _notifyIcon;

        public void SetText(string text)
        {
            if (!_disposed && _notifyIcon != null)
            {
                _notifyIcon.Text = text;
            }
        }

        public void SetIcon(Icon icon)
        {
            if (!_disposed && _notifyIcon != null)
            {
                _notifyIcon.Icon = icon;
            }
        }

        public void ShowMainWindow()
        {
            if (_mainForm != null && !_disposed && _notifyIcon != null)
            {
                // Ensure the form is shown and brought to front
                _mainForm.Invoke(new Action(() =>
                {
                    _mainForm.Show();
                    _mainForm.BringToFront();
                    _mainForm.WindowState = FormWindowState.Normal;
                }));
                TraceLogger.Log("ShowMainWindow called from GlobalNotifyIcon.");
            }
        }

        public void Dispose()
        {
            TraceLogger.Log("Disposing GlobalNotifyIcon...");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            TraceLogger.Log("GlobalNotifyIcon Dispose called.");
            if (!_disposed && disposing)
            {
                _notifyIcon?.Dispose();
                _notifyIcon = null;
                _disposed = true;
            }
        }

        public void Hide()
        {
            if (!_disposed && _notifyIcon != null)
            {
                _notifyIcon.Visible = false;
            }
        }

        public void Show()
        {
            if (!_disposed && _notifyIcon != null)
            {
                _notifyIcon.Visible = true;
            }
        }
    }
}