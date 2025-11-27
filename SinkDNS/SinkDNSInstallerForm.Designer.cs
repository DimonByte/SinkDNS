namespace SinkDNS
{
    partial class SinkDNSInstallerForm
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
            progressBar1 = new ProgressBar();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 329);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(679, 24);
            progressBar1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(703, 323);
            panel1.TabIndex = 1;
            // 
            // SinkDNSInstallerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(703, 365);
            Controls.Add(panel1);
            Controls.Add(progressBar1);
            Name = "SinkDNSInstallerForm";
            Text = "SinkDNSInstallerForm";
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar progressBar1;
        private Panel panel1;
    }
}