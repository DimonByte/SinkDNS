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
            ApplyBtn = new Button();
            ApplyAndRestartBtn = new Button();
            customStaticServerTxt = new TextBox();
            AddCustomToListBtn = new Button();
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
            // ApplyBtn
            // 
            ApplyBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ApplyBtn.Location = new Point(423, 338);
            ApplyBtn.Name = "ApplyBtn";
            ApplyBtn.Size = new Size(75, 23);
            ApplyBtn.TabIndex = 3;
            ApplyBtn.Text = "Apply";
            ApplyBtn.UseVisualStyleBackColor = true;
            ApplyBtn.Click += ApplyBtn_Click;
            // 
            // ApplyAndRestartBtn
            // 
            ApplyAndRestartBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ApplyAndRestartBtn.Location = new Point(292, 338);
            ApplyAndRestartBtn.Name = "ApplyAndRestartBtn";
            ApplyAndRestartBtn.Size = new Size(125, 23);
            ApplyAndRestartBtn.TabIndex = 3;
            ApplyAndRestartBtn.Text = "Apply && Restart";
            ApplyAndRestartBtn.UseVisualStyleBackColor = true;
            ApplyAndRestartBtn.Click += ApplyAndRestartBtn_Click;
            // 
            // customStaticServerTxt
            // 
            customStaticServerTxt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            customStaticServerTxt.Location = new Point(13, 300);
            customStaticServerTxt.Name = "customStaticServerTxt";
            customStaticServerTxt.Size = new Size(404, 23);
            customStaticServerTxt.TabIndex = 4;
            // 
            // AddCustomToListBtn
            // 
            AddCustomToListBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            AddCustomToListBtn.Location = new Point(423, 299);
            AddCustomToListBtn.Name = "AddCustomToListBtn";
            AddCustomToListBtn.Size = new Size(75, 23);
            AddCustomToListBtn.TabIndex = 5;
            AddCustomToListBtn.Text = "Add to list";
            AddCustomToListBtn.UseVisualStyleBackColor = true;
            AddCustomToListBtn.Click += AddCustomToListBtn_Click;
            // 
            // DNSConfigurationList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(224, 224, 224);
            Controls.Add(AddCustomToListBtn);
            Controls.Add(customStaticServerTxt);
            Controls.Add(ApplyAndRestartBtn);
            Controls.Add(ApplyBtn);
            Controls.Add(label2);
            Controls.Add(checkedListBox1);
            Controls.Add(label1);
            Name = "DNSConfigurationList";
            Size = new Size(513, 375);
            Load += DNSConfigurationList_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private CheckedListBox checkedListBox1;
        private Button ApplyBtn;
        private Button ApplyAndRestartBtn;
        private TextBox customStaticServerTxt;
        private Button AddCustomToListBtn;
    }
}
