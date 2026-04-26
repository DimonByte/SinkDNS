namespace SinkDNS
{
    partial class SinkDNSManagerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            button1 = new Button();
            MainContextMenuStrip = new ContextMenuStrip(components);
            openManagerToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            addNewBlocklistToolStripMenuItem = new ToolStripMenuItem();
            addNewWhitelistToolStripMenuItem = new ToolStripMenuItem();
            addURLToBlocklistToolStripMenuItem = new ToolStripMenuItem();
            addURLToWhitelistToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            traceLogViewerToolStripMenuItem = new ToolStripMenuItem();
            updateBlocklistsToolStripMenuItem = new ToolStripMenuItem();
            checkForProgramUpdatesToolStripMenuItem = new ToolStripMenuItem();
            bypassFilteringToolStripMenuItem = new ToolStripMenuItem();
            restartDNSCryptToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            aboutSinkDNSToolStripMenuItem = new ToolStripMenuItem();
            exitSinkDNSToolStripMenuItem = new ToolStripMenuItem();
            MainPanel = new SplitContainer();
            SettingsBtn = new Button();
            PublicResolversBtn = new Button();
            QueryLogBtn = new Button();
            UserDomainsBtn = new Button();
            ListsBtn = new Button();
            ToggleDNSCryptBtn = new Button();
            DashboardBtn = new Button();
            label1 = new Label();
            panel1 = new Panel();
            MainContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MainPanel).BeginInit();
            MainPanel.Panel1.SuspendLayout();
            MainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(52, 322);
            button1.Name = "button1";
            button1.Size = new Size(95, 26);
            button1.TabIndex = 1;
            button1.Text = "Test";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_ClickAsync;
            // 
            // MainContextMenuStrip
            // 
            MainContextMenuStrip.Items.AddRange(new ToolStripItem[] { openManagerToolStripMenuItem, toolStripSeparator3, addNewBlocklistToolStripMenuItem, addNewWhitelistToolStripMenuItem, addURLToBlocklistToolStripMenuItem, addURLToWhitelistToolStripMenuItem, toolStripSeparator2, traceLogViewerToolStripMenuItem, updateBlocklistsToolStripMenuItem, checkForProgramUpdatesToolStripMenuItem, bypassFilteringToolStripMenuItem, restartDNSCryptToolStripMenuItem, toolStripSeparator1, aboutSinkDNSToolStripMenuItem, exitSinkDNSToolStripMenuItem });
            MainContextMenuStrip.Name = "MainContextMenuStrip";
            MainContextMenuStrip.Size = new Size(230, 286);
            // 
            // openManagerToolStripMenuItem
            // 
            openManagerToolStripMenuItem.Image = Properties.Resources.SinkDNSDarkIcon;
            openManagerToolStripMenuItem.Name = "openManagerToolStripMenuItem";
            openManagerToolStripMenuItem.Size = new Size(229, 22);
            openManagerToolStripMenuItem.Text = "Open SinkDNS Manager";
            openManagerToolStripMenuItem.Click += openManagerToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(226, 6);
            // 
            // addNewBlocklistToolStripMenuItem
            // 
            addNewBlocklistToolStripMenuItem.Image = Properties.Resources.wifi_off;
            addNewBlocklistToolStripMenuItem.Name = "addNewBlocklistToolStripMenuItem";
            addNewBlocklistToolStripMenuItem.Size = new Size(229, 22);
            addNewBlocklistToolStripMenuItem.Text = "Add New Blocklist";
            addNewBlocklistToolStripMenuItem.ToolTipText = "Add a new blocklist that contains a list of urls from a repository.";
            // 
            // addNewWhitelistToolStripMenuItem
            // 
            addNewWhitelistToolStripMenuItem.Image = Properties.Resources.wifi_signal_none_solid;
            addNewWhitelistToolStripMenuItem.Name = "addNewWhitelistToolStripMenuItem";
            addNewWhitelistToolStripMenuItem.Size = new Size(229, 22);
            addNewWhitelistToolStripMenuItem.Text = "Add New Whitelist";
            addNewWhitelistToolStripMenuItem.ToolTipText = "Add a new whitelist that contains a list of urls from a repository.";
            // 
            // addURLToBlocklistToolStripMenuItem
            // 
            addURLToBlocklistToolStripMenuItem.Image = Properties.Resources.wifi_off;
            addURLToBlocklistToolStripMenuItem.Name = "addURLToBlocklistToolStripMenuItem";
            addURLToBlocklistToolStripMenuItem.Size = new Size(229, 22);
            addURLToBlocklistToolStripMenuItem.Text = "Add URL to Blocklist";
            // 
            // addURLToWhitelistToolStripMenuItem
            // 
            addURLToWhitelistToolStripMenuItem.Image = Properties.Resources.wifi_signal_none_solid;
            addURLToWhitelistToolStripMenuItem.Name = "addURLToWhitelistToolStripMenuItem";
            addURLToWhitelistToolStripMenuItem.Size = new Size(229, 22);
            addURLToWhitelistToolStripMenuItem.Text = "Add URL to Whitelist";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(226, 6);
            // 
            // traceLogViewerToolStripMenuItem
            // 
            traceLogViewerToolStripMenuItem.Name = "traceLogViewerToolStripMenuItem";
            traceLogViewerToolStripMenuItem.Size = new Size(229, 22);
            traceLogViewerToolStripMenuItem.Text = "Trace Log Viewer";
            traceLogViewerToolStripMenuItem.Click += traceLogViewerToolStripMenuItem_Click;
            // 
            // updateBlocklistsToolStripMenuItem
            // 
            updateBlocklistsToolStripMenuItem.Image = Properties.Resources.database_backup;
            updateBlocklistsToolStripMenuItem.Name = "updateBlocklistsToolStripMenuItem";
            updateBlocklistsToolStripMenuItem.Size = new Size(229, 22);
            updateBlocklistsToolStripMenuItem.Text = "Update all lists";
            updateBlocklistsToolStripMenuItem.ToolTipText = "Manually updates the blocklist and whitelist for the latest updates.";
            updateBlocklistsToolStripMenuItem.Click += updateBlocklistsToolStripMenuItem_Click;
            // 
            // checkForProgramUpdatesToolStripMenuItem
            // 
            checkForProgramUpdatesToolStripMenuItem.Name = "checkForProgramUpdatesToolStripMenuItem";
            checkForProgramUpdatesToolStripMenuItem.Size = new Size(229, 22);
            checkForProgramUpdatesToolStripMenuItem.Text = "Check for Program Updates...";
            // 
            // bypassFilteringToolStripMenuItem
            // 
            bypassFilteringToolStripMenuItem.CheckOnClick = true;
            bypassFilteringToolStripMenuItem.Image = Properties.Resources.BypassFilter;
            bypassFilteringToolStripMenuItem.Name = "bypassFilteringToolStripMenuItem";
            bypassFilteringToolStripMenuItem.Size = new Size(229, 22);
            bypassFilteringToolStripMenuItem.Text = "Bypass Filtering";
            bypassFilteringToolStripMenuItem.ToolTipText = "Turns off DNSCrypt and reverts to the DNS settings that was set before installing SinkDNS. If no setting is found, the DNS will be set to 1.1.1.1.";
            bypassFilteringToolStripMenuItem.Click += bypassFilteringToolStripMenuItem_Click;
            // 
            // restartDNSCryptToolStripMenuItem
            // 
            restartDNSCryptToolStripMenuItem.Image = Properties.Resources.restart;
            restartDNSCryptToolStripMenuItem.Name = "restartDNSCryptToolStripMenuItem";
            restartDNSCryptToolStripMenuItem.Size = new Size(229, 22);
            restartDNSCryptToolStripMenuItem.Text = "Restart DNSCrypt";
            restartDNSCryptToolStripMenuItem.Click += restartDNSCryptToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(226, 6);
            // 
            // aboutSinkDNSToolStripMenuItem
            // 
            aboutSinkDNSToolStripMenuItem.Image = Properties.Resources.info_circle;
            aboutSinkDNSToolStripMenuItem.Name = "aboutSinkDNSToolStripMenuItem";
            aboutSinkDNSToolStripMenuItem.Size = new Size(229, 22);
            aboutSinkDNSToolStripMenuItem.Text = "About SinkDNS...";
            aboutSinkDNSToolStripMenuItem.Click += aboutSinkDNSToolStripMenuItem_Click;
            // 
            // exitSinkDNSToolStripMenuItem
            // 
            exitSinkDNSToolStripMenuItem.Name = "exitSinkDNSToolStripMenuItem";
            exitSinkDNSToolStripMenuItem.Size = new Size(229, 22);
            exitSinkDNSToolStripMenuItem.Text = "Exit SinkDNS";
            exitSinkDNSToolStripMenuItem.Click += exitSinkDNSToolStripMenuItem_Click;
            // 
            // MainPanel
            // 
            MainPanel.Dock = DockStyle.Fill;
            MainPanel.FixedPanel = FixedPanel.Panel1;
            MainPanel.Location = new Point(0, 0);
            MainPanel.Name = "MainPanel";
            // 
            // MainPanel.Panel1
            // 
            MainPanel.Panel1.BackColor = Color.FromArgb(3, 83, 164);
            MainPanel.Panel1.Controls.Add(SettingsBtn);
            MainPanel.Panel1.Controls.Add(PublicResolversBtn);
            MainPanel.Panel1.Controls.Add(button1);
            MainPanel.Panel1.Controls.Add(QueryLogBtn);
            MainPanel.Panel1.Controls.Add(UserDomainsBtn);
            MainPanel.Panel1.Controls.Add(ListsBtn);
            MainPanel.Panel1.Controls.Add(ToggleDNSCryptBtn);
            MainPanel.Panel1.Controls.Add(DashboardBtn);
            MainPanel.Panel1.Controls.Add(label1);
            MainPanel.Panel1.Controls.Add(panel1);
            MainPanel.Panel1MinSize = 200;
            // 
            // MainPanel.Panel2
            // 
            MainPanel.Panel2.BackColor = Color.FromArgb(6, 26, 64);
            MainPanel.Panel2MinSize = 400;
            MainPanel.Size = new Size(650, 390);
            MainPanel.SplitterDistance = 200;
            MainPanel.SplitterWidth = 3;
            MainPanel.TabIndex = 3;
            // 
            // SettingsBtn
            // 
            SettingsBtn.Dock = DockStyle.Top;
            SettingsBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 109, 170);
            SettingsBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 53, 89);
            SettingsBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 109, 170);
            SettingsBtn.FlatStyle = FlatStyle.Flat;
            SettingsBtn.ForeColor = Color.White;
            SettingsBtn.Location = new Point(0, 271);
            SettingsBtn.Name = "SettingsBtn";
            SettingsBtn.Size = new Size(200, 34);
            SettingsBtn.TabIndex = 5;
            SettingsBtn.Text = "All Settings";
            SettingsBtn.UseVisualStyleBackColor = true;
            SettingsBtn.Click += SettingsBtn_Click;
            // 
            // PublicResolversBtn
            // 
            PublicResolversBtn.Dock = DockStyle.Top;
            PublicResolversBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 109, 170);
            PublicResolversBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 53, 89);
            PublicResolversBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 109, 170);
            PublicResolversBtn.FlatStyle = FlatStyle.Flat;
            PublicResolversBtn.ForeColor = Color.White;
            PublicResolversBtn.Location = new Point(0, 237);
            PublicResolversBtn.Name = "PublicResolversBtn";
            PublicResolversBtn.Size = new Size(200, 34);
            PublicResolversBtn.TabIndex = 13;
            PublicResolversBtn.Text = "Public Resolvers";
            PublicResolversBtn.UseVisualStyleBackColor = true;
            PublicResolversBtn.Click += PublicResolversBtn_Click;
            // 
            // QueryLogBtn
            // 
            QueryLogBtn.Dock = DockStyle.Top;
            QueryLogBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 109, 170);
            QueryLogBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 53, 89);
            QueryLogBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 109, 170);
            QueryLogBtn.FlatStyle = FlatStyle.Flat;
            QueryLogBtn.ForeColor = Color.White;
            QueryLogBtn.Location = new Point(0, 203);
            QueryLogBtn.Name = "QueryLogBtn";
            QueryLogBtn.Size = new Size(200, 34);
            QueryLogBtn.TabIndex = 9;
            QueryLogBtn.Text = "Query Logs";
            QueryLogBtn.UseVisualStyleBackColor = true;
            // 
            // UserDomainsBtn
            // 
            UserDomainsBtn.Dock = DockStyle.Top;
            UserDomainsBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 109, 170);
            UserDomainsBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 53, 89);
            UserDomainsBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 109, 170);
            UserDomainsBtn.FlatStyle = FlatStyle.Flat;
            UserDomainsBtn.ForeColor = Color.White;
            UserDomainsBtn.Location = new Point(0, 169);
            UserDomainsBtn.Name = "UserDomainsBtn";
            UserDomainsBtn.Size = new Size(200, 34);
            UserDomainsBtn.TabIndex = 8;
            UserDomainsBtn.Text = "User Specified Domains";
            UserDomainsBtn.UseVisualStyleBackColor = true;
            // 
            // ListsBtn
            // 
            ListsBtn.Dock = DockStyle.Top;
            ListsBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 109, 170);
            ListsBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 53, 89);
            ListsBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 109, 170);
            ListsBtn.FlatStyle = FlatStyle.Flat;
            ListsBtn.ForeColor = Color.White;
            ListsBtn.Location = new Point(0, 135);
            ListsBtn.Name = "ListsBtn";
            ListsBtn.Size = new Size(200, 34);
            ListsBtn.TabIndex = 7;
            ListsBtn.Text = "Lists";
            ListsBtn.UseVisualStyleBackColor = true;
            // 
            // ToggleDNSCryptBtn
            // 
            ToggleDNSCryptBtn.Dock = DockStyle.Top;
            ToggleDNSCryptBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 109, 170);
            ToggleDNSCryptBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 53, 89);
            ToggleDNSCryptBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 109, 170);
            ToggleDNSCryptBtn.FlatStyle = FlatStyle.Flat;
            ToggleDNSCryptBtn.ForeColor = Color.White;
            ToggleDNSCryptBtn.Location = new Point(0, 101);
            ToggleDNSCryptBtn.Name = "ToggleDNSCryptBtn";
            ToggleDNSCryptBtn.Size = new Size(200, 34);
            ToggleDNSCryptBtn.TabIndex = 6;
            ToggleDNSCryptBtn.Text = "Disable DNSCrypt";
            ToggleDNSCryptBtn.UseVisualStyleBackColor = true;
            // 
            // DashboardBtn
            // 
            DashboardBtn.Dock = DockStyle.Top;
            DashboardBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 109, 170);
            DashboardBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 53, 89);
            DashboardBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 109, 170);
            DashboardBtn.FlatStyle = FlatStyle.Flat;
            DashboardBtn.ForeColor = Color.White;
            DashboardBtn.Location = new Point(0, 67);
            DashboardBtn.Name = "DashboardBtn";
            DashboardBtn.Size = new Size(200, 34);
            DashboardBtn.TabIndex = 10;
            DashboardBtn.Text = "Monitoring";
            DashboardBtn.UseVisualStyleBackColor = true;
            DashboardBtn.Click += DashboardBtn_Click;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Arial", 8F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 45);
            label1.Name = "label1";
            label1.Size = new Size(200, 22);
            label1.TabIndex = 12;
            label1.Text = "SinkDNS Manager Window";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.BackgroundImage = Properties.Resources.SinkDNSIconImage;
            panel1.BackgroundImageLayout = ImageLayout.Center;
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 45);
            panel1.TabIndex = 11;
            // 
            // SinkDNSManagerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(3, 83, 194);
            ClientSize = new Size(650, 390);
            Controls.Add(MainPanel);
            Font = new Font("Arial", 9F);
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(666, 429);
            Name = "SinkDNSManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SinkDNS Manager";
            WindowState = FormWindowState.Minimized;
            FormClosing += SinkDNSManagerForm_FormClosing;
            Load += SinkDNSMainForm_Load;
            MainContextMenuStrip.ResumeLayout(false);
            MainPanel.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)MainPanel).EndInit();
            MainPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private ContextMenuStrip MainContextMenuStrip;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitSinkDNSToolStripMenuItem;
        private ToolStripMenuItem bypassFilteringToolStripMenuItem;
        private ToolStripMenuItem openManagerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem addURLToBlocklistToolStripMenuItem;
        private ToolStripMenuItem addNewBlocklistToolStripMenuItem;
        private ToolStripMenuItem addURLToWhitelistToolStripMenuItem;
        private ToolStripMenuItem addNewWhitelistToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem restartDNSCryptToolStripMenuItem;
        private ToolStripMenuItem updateBlocklistsToolStripMenuItem;
        private ToolStripMenuItem aboutSinkDNSToolStripMenuItem;
        private ToolStripMenuItem checkForProgramUpdatesToolStripMenuItem;
        private SplitContainer MainPanel;
        private Button DashboardBtn;
        private Button QueryLogBtn;
        private Button UserDomainsBtn;
        private Button ListsBtn;
        private Button ToggleDNSCryptBtn;
        private Button SettingsBtn;
        private Panel panel1;
        private Label label1;
        private ToolStripMenuItem traceLogViewerToolStripMenuItem;
        private Button PublicResolversBtn;
    }
}
