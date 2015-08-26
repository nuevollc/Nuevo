namespace TestForm
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
            this.SFTPStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SFTPStart
            // 
            this.SFTPStart.Location = new System.Drawing.Point(131, 64);
            this.SFTPStart.Name = "SFTPStart";
            this.SFTPStart.Size = new System.Drawing.Size(108, 23);
            this.SFTPStart.TabIndex = 0;
            this.SFTPStart.Text = "SFTPStart";
            this.SFTPStart.UseVisualStyleBackColor = true;
            this.SFTPStart.Click += new System.EventHandler(this.SFTPStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.SFTPStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SFTPStart;
    }
}

