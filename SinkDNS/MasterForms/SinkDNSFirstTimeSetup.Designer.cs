namespace SinkDNS
{
    partial class SinkDNSFirstTimeSetup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            label1 = new Label();
            label2 = new Label();
            panel2 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackgroundImage = Properties.Resources.SinkDNSIconImage;
            panel1.BackgroundImageLayout = ImageLayout.Center;
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 45);
            panel1.TabIndex = 12;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Arial", 8F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 45);
            label1.Name = "label1";
            label1.Size = new Size(800, 22);
            label1.TabIndex = 13;
            label1.Text = "Welcome to SinkDNS";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Top;
            label2.Font = new Font("Arial", 9F);
            label2.ForeColor = Color.White;
            label2.Location = new Point(0, 67);
            label2.Name = "label2";
            label2.Size = new Size(800, 22);
            label2.TabIndex = 14;
            label2.Text = "To get started, please go through each tab to configure your system for SinkDNS.";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(6, 26, 64);
            panel2.BackgroundImageLayout = ImageLayout.Center;
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 405);
            panel2.Name = "panel2";
            panel2.Size = new Size(800, 45);
            panel2.TabIndex = 15;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 89);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 316);
            tabControl1.TabIndex = 16;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 288);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Step 1: Configure System";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 288);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Step 2: Select your hostfiles";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // SinkDNSFirstTimeSetup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(3, 83, 164);
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Controls.Add(panel2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel1);
            Name = "SinkDNSFirstTimeSetup";
            Text = "SinkDNSFirstTimeSetup";
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Label label2;
        private Panel panel2;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
    }
}