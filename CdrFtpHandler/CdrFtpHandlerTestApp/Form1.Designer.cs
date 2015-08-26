namespace CdrFtpHandlerTestApp
{
    partial class Form1
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
            this.TestFtp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TestFtp
            // 
            this.TestFtp.Location = new System.Drawing.Point(74, 139);
            this.TestFtp.Name = "TestFtp";
            this.TestFtp.Size = new System.Drawing.Size(104, 37);
            this.TestFtp.TabIndex = 0;
            this.TestFtp.Text = "TestFtp";
            this.TestFtp.UseVisualStyleBackColor = true;
            this.TestFtp.Click += new System.EventHandler(this.TestFtp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.TestFtp);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TestFtp;
    }
}

