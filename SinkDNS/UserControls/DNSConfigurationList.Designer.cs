namespace SinkDNS.UserControls
{
    partial class DNSConfigurationList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            checkedListBox1 = new CheckedListBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // checkedListBox1
            // 
            checkedListBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.IntegralHeight = false;
            checkedListBox1.Location = new Point(0, 34);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(513, 241);
            checkedListBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(513, 34);
            label1.TabIndex = 1;
            label1.Text = "Select the DNS server that you want DNSCrypt to communicate with.";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.Location = new Point(0, 275);
            label2.Name = "label2";
            label2.Size = new Size(513, 22);
            label2.TabIndex = 2;
            label2.Text = "Or add a custom static server below:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DNSConfigurationList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(224, 224, 224);
            Controls.Add(label2);
            Controls.Add(checkedListBox1);
            Controls.Add(label1);
            Name = "DNSConfigurationList";
            Size = new Size(513, 375);
            Load += DNSConfigurationList_Load;
            ResumeLayout(false);
        }

        #endregion
        private Label label1;
        private Label label2;
        private CheckedListBox checkedListBox1;
    }
}
