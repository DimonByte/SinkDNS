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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SinkDNSManagerForm));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            SinkDNSDesignLabel = new Label();
            DNSCryptStatusLabel = new Label();
            button1 = new Button();
            SinkDNSSystemTray = new NotifyIcon(components);
            MainContextMenuStrip = new ContextMenuStrip(components);
            openManagerToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            addURLToBlocklistToolStripMenuItem = new ToolStripMenuItem();
            addNewBlocklistToolStripMenuItem = new ToolStripMenuItem();
            addURLToWhitelistToolStripMenuItem = new ToolStripMenuItem();
            addNewWhitelistToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            updateBlocklistsToolStripMenuItem = new ToolStripMenuItem();
            bypassFilteringToolStripMenuItem = new ToolStripMenuItem();
            restartDNSCryptToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitSinkDNSToolStripMenuItem = new ToolStripMenuItem();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            MainContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(46, 140, 148);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(SinkDNSDesignLabel);
            panel1.Controls.Add(DNSCryptStatusLabel);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(654, 38);
            panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.SinkDNSIcon;
            pictureBox1.Location = new Point(4, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(32, 32);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // SinkDNSDesignLabel
            // 
            SinkDNSDesignLabel.AutoSize = true;
            SinkDNSDesignLabel.Font = new Font("Arial", 12F);
            SinkDNSDesignLabel.ForeColor = Color.White;
            SinkDNSDesignLabel.Location = new Point(33, 9);
            SinkDNSDesignLabel.Name = "SinkDNSDesignLabel";
            SinkDNSDesignLabel.Size = new Size(139, 18);
            SinkDNSDesignLabel.TabIndex = 0;
            SinkDNSDesignLabel.Text = "SinkDNS Manager";
            // 
            // DNSCryptStatusLabel
            // 
            DNSCryptStatusLabel.AutoSize = true;
            DNSCryptStatusLabel.ForeColor = Color.White;
            DNSCryptStatusLabel.Location = new Point(178, 12);
            DNSCryptStatusLabel.Name = "DNSCryptStatusLabel";
            DNSCryptStatusLabel.Size = new Size(124, 15);
            DNSCryptStatusLabel.TabIndex = 0;
            DNSCryptStatusLabel.Text = "DNSCrypt Status: N/A";
            // 
            // button1
            // 
            button1.Location = new Point(547, 335);
            button1.Name = "button1";
            button1.Size = new Size(95, 26);
            button1.TabIndex = 1;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // SinkDNSSystemTray
            // 
            SinkDNSSystemTray.ContextMenuStrip = MainContextMenuStrip;
            SinkDNSSystemTray.Icon = (Icon)resources.GetObject("SinkDNSSystemTray.Icon");
            SinkDNSSystemTray.Text = "SinkDNS";
            SinkDNSSystemTray.Visible = true;
            SinkDNSSystemTray.DoubleClick += SinkDNSSystemTray_DoubleClick;
            // 
            // MainContextMenuStrip
            // 
            MainContextMenuStrip.Items.AddRange(new ToolStripItem[] { openManagerToolStripMenuItem, toolStripSeparator3, addURLToBlocklistToolStripMenuItem, addNewBlocklistToolStripMenuItem, addURLToWhitelistToolStripMenuItem, addNewWhitelistToolStripMenuItem, toolStripSeparator2, updateBlocklistsToolStripMenuItem, bypassFilteringToolStripMenuItem, restartDNSCryptToolStripMenuItem, toolStripSeparator1, exitSinkDNSToolStripMenuItem });
            MainContextMenuStrip.Name = "MainContextMenuStrip";
            MainContextMenuStrip.Size = new Size(202, 220);
            // 
            // openManagerToolStripMenuItem
            // 
            openManagerToolStripMenuItem.Name = "openManagerToolStripMenuItem";
            openManagerToolStripMenuItem.Size = new Size(201, 22);
            openManagerToolStripMenuItem.Text = "Open SinkDNS Manager";
            openManagerToolStripMenuItem.Click += openManagerToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(198, 6);
            // 
            // addURLToBlocklistToolStripMenuItem
            // 
            addURLToBlocklistToolStripMenuItem.Name = "addURLToBlocklistToolStripMenuItem";
            addURLToBlocklistToolStripMenuItem.Size = new Size(201, 22);
            addURLToBlocklistToolStripMenuItem.Text = "Add URL to Blocklist";
            // 
            // addNewBlocklistToolStripMenuItem
            // 
            addNewBlocklistToolStripMenuItem.Name = "addNewBlocklistToolStripMenuItem";
            addNewBlocklistToolStripMenuItem.Size = new Size(201, 22);
            addNewBlocklistToolStripMenuItem.Text = "Add New Blocklist";
            // 
            // addURLToWhitelistToolStripMenuItem
            // 
            addURLToWhitelistToolStripMenuItem.Name = "addURLToWhitelistToolStripMenuItem";
            addURLToWhitelistToolStripMenuItem.Size = new Size(201, 22);
            addURLToWhitelistToolStripMenuItem.Text = "Add URL to Whitelist";
            // 
            // addNewWhitelistToolStripMenuItem
            // 
            addNewWhitelistToolStripMenuItem.Name = "addNewWhitelistToolStripMenuItem";
            addNewWhitelistToolStripMenuItem.Size = new Size(201, 22);
            addNewWhitelistToolStripMenuItem.Text = "Add New Whitelist";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(198, 6);
            // 
            // updateBlocklistsToolStripMenuItem
            // 
            updateBlocklistsToolStripMenuItem.Name = "updateBlocklistsToolStripMenuItem";
            updateBlocklistsToolStripMenuItem.Size = new Size(201, 22);
            updateBlocklistsToolStripMenuItem.Text = "Update all lists";
            updateBlocklistsToolStripMenuItem.Click += updateBlocklistsToolStripMenuItem_Click;
            // 
            // bypassFilteringToolStripMenuItem
            // 
            bypassFilteringToolStripMenuItem.CheckOnClick = true;
            bypassFilteringToolStripMenuItem.Name = "bypassFilteringToolStripMenuItem";
            bypassFilteringToolStripMenuItem.Size = new Size(201, 22);
            bypassFilteringToolStripMenuItem.Text = "Bypass Filtering";
            // 
            // restartDNSCryptToolStripMenuItem
            // 
            restartDNSCryptToolStripMenuItem.Name = "restartDNSCryptToolStripMenuItem";
            restartDNSCryptToolStripMenuItem.Size = new Size(201, 22);
            restartDNSCryptToolStripMenuItem.Text = "Restart DNSCrypt";
            restartDNSCryptToolStripMenuItem.Click += restartDNSCryptToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(198, 6);
            // 
            // exitSinkDNSToolStripMenuItem
            // 
            exitSinkDNSToolStripMenuItem.Name = "exitSinkDNSToolStripMenuItem";
            exitSinkDNSToolStripMenuItem.Size = new Size(201, 22);
            exitSinkDNSToolStripMenuItem.Text = "Exit SinkDNS";
            exitSinkDNSToolStripMenuItem.Click += exitSinkDNSToolStripMenuItem_Click;
            // 
            // SinkDNSManagerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 68, 97);
            ClientSize = new Size(654, 373);
            Controls.Add(button1);
            Controls.Add(panel1);
            Font = new Font("Arial", 9F);
            Margin = new Padding(3, 2, 3, 2);
            Name = "SinkDNSManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SinkDNS Manager";
            WindowState = FormWindowState.Minimized;
            FormClosing += SinkDNSManagerForm_FormClosing;
            Load += SinkDNSMainForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            MainContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label DNSCryptStatusLabel;
        private Label SinkDNSDesignLabel;
        private Button button1;
        private NotifyIcon SinkDNSSystemTray;
        private PictureBox pictureBox1;
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
    }
}
