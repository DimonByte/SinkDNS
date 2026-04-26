namespace SinkDNS.ChildForms
{
    partial class HostListView
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
            groupBox1 = new GroupBox();
            groupBox3 = new GroupBox();
            groupBox4 = new GroupBox();
            groupBox2 = new GroupBox();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.White;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(304, 231);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Overview";
            // 
            // groupBox3
            // 
            groupBox3.BackColor = Color.White;
            groupBox3.Location = new Point(313, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(304, 231);
            groupBox3.TabIndex = 0;
            groupBox3.TabStop = false;
            groupBox3.Text = "Top Allowed Domains";
            // 
            // groupBox4
            // 
            groupBox4.BackColor = Color.White;
            groupBox4.Location = new Point(313, 240);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(304, 231);
            groupBox4.TabIndex = 0;
            groupBox4.TabStop = false;
            groupBox4.Text = "Top Denied Domains";
            // 
            // groupBox2
            // 
            groupBox2.BackColor = Color.White;
            groupBox2.Location = new Point(3, 240);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(304, 231);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "Recent Domains";
            // 
            // HostListView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(224, 224, 224);
            Controls.Add(groupBox2);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox1);
            Name = "HostListView";
            Size = new Size(626, 481);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox2;
    }
}