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
            panel1 = new Panel();
            SinkDNSDesignLabel = new Label();
            DNSCryptStatusLabel = new Label();
            button1 = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(46, 140, 148);
            panel1.Controls.Add(SinkDNSDesignLabel);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(654, 38);
            panel1.TabIndex = 0;
            // 
            // SinkDNSDesignLabel
            // 
            SinkDNSDesignLabel.AutoSize = true;
            SinkDNSDesignLabel.Font = new Font("Arial", 12F);
            SinkDNSDesignLabel.ForeColor = Color.White;
            SinkDNSDesignLabel.Location = new Point(12, 9);
            SinkDNSDesignLabel.Name = "SinkDNSDesignLabel";
            SinkDNSDesignLabel.Size = new Size(139, 18);
            SinkDNSDesignLabel.TabIndex = 0;
            SinkDNSDesignLabel.Text = "SinkDNS Manager";
            // 
            // DNSCryptStatusLabel
            // 
            DNSCryptStatusLabel.AutoSize = true;
            DNSCryptStatusLabel.ForeColor = Color.White;
            DNSCryptStatusLabel.Location = new Point(12, 43);
            DNSCryptStatusLabel.Name = "DNSCryptStatusLabel";
            DNSCryptStatusLabel.Size = new Size(124, 15);
            DNSCryptStatusLabel.TabIndex = 0;
            DNSCryptStatusLabel.Text = "DNSCrypt Status: N/A";
            // 
            // button1
            // 
            button1.Location = new Point(12, 61);
            button1.Name = "button1";
            button1.Size = new Size(95, 26);
            button1.TabIndex = 1;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // SinkDNSManagerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(48, 68, 97);
            ClientSize = new Size(654, 373);
            Controls.Add(button1);
            Controls.Add(DNSCryptStatusLabel);
            Controls.Add(panel1);
            Font = new Font("Arial", 9F);
            Margin = new Padding(3, 2, 3, 2);
            Name = "SinkDNSManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SinkDNS Manager";
            Load += SinkDNSMainForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label DNSCryptStatusLabel;
        private Label SinkDNSDesignLabel;
        private Button button1;
    }
}
