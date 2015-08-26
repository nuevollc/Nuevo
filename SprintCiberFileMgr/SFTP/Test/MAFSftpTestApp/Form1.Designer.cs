namespace MAFSftpTestApp
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
            this.GetFileViaSFTP = new System.Windows.Forms.Button();
            this.StopProcessor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GetFileViaSFTP
            // 
            this.GetFileViaSFTP.Location = new System.Drawing.Point(99, 80);
            this.GetFileViaSFTP.Name = "GetFileViaSFTP";
            this.GetFileViaSFTP.Size = new System.Drawing.Size(134, 54);
            this.GetFileViaSFTP.TabIndex = 0;
            this.GetFileViaSFTP.Text = "GetFileViaSFTP";
            this.GetFileViaSFTP.UseVisualStyleBackColor = true;
            this.GetFileViaSFTP.Click += new System.EventHandler(this.GetFileViaSFTP_Click);
            // 
            // StopProcessor
            // 
            this.StopProcessor.Location = new System.Drawing.Point(99, 164);
            this.StopProcessor.Name = "StopProcessor";
            this.StopProcessor.Size = new System.Drawing.Size(134, 45);
            this.StopProcessor.TabIndex = 1;
            this.StopProcessor.Text = "StopProcessor";
            this.StopProcessor.UseVisualStyleBackColor = true;
            this.StopProcessor.Click += new System.EventHandler(this.StopProcessor_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.StopProcessor);
            this.Controls.Add(this.GetFileViaSFTP);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button GetFileViaSFTP;
        private System.Windows.Forms.Button StopProcessor;
    }
}

