namespace SinkDNS
{
    partial class SinkDNSMainForm
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
            testBtn = new Button();
            SuspendLayout();
            // 
            // testBtn
            // 
            testBtn.Location = new Point(256, 130);
            testBtn.Name = "testBtn";
            testBtn.Size = new Size(94, 29);
            testBtn.TabIndex = 0;
            testBtn.Text = "button1";
            testBtn.UseVisualStyleBackColor = true;
            testBtn.Click += testBtn_Click;
            // 
            // SinkDNSMainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(testBtn);
            Name = "SinkDNSMainForm";
            Text = "Form1";
            Load += SinkDNSMainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button testBtn;
    }
}
