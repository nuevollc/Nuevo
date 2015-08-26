namespace SftpTestApp
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
            this.SFTPFileHandler = new System.Windows.Forms.Button();
            this.StopSFTPHandler = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SFTPFileHandler
            // 
            this.SFTPFileHandler.Location = new System.Drawing.Point(83, 141);
            this.SFTPFileHandler.Name = "SFTPFileHandler";
            this.SFTPFileHandler.Size = new System.Drawing.Size(129, 23);
            this.SFTPFileHandler.TabIndex = 0;
            this.SFTPFileHandler.Text = "StartSFTPFileHandler";
            this.SFTPFileHandler.UseVisualStyleBackColor = true;
            this.SFTPFileHandler.Click += new System.EventHandler(this.SFTPFile_Click);
            // 
            // StopSFTPHandler
            // 
            this.StopSFTPHandler.Location = new System.Drawing.Point(83, 184);
            this.StopSFTPHandler.Name = "StopSFTPHandler";
            this.StopSFTPHandler.Size = new System.Drawing.Size(129, 23);
            this.StopSFTPHandler.TabIndex = 1;
            this.StopSFTPHandler.Text = "StopSFTPHandler";
            this.StopSFTPHandler.UseVisualStyleBackColor = true;
            this.StopSFTPHandler.Click += new System.EventHandler(this.StopSFTPHandler_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.StopSFTPHandler);
            this.Controls.Add(this.SFTPFileHandler);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SFTPFileHandler;
        private System.Windows.Forms.Button StopSFTPHandler;
    }
}

