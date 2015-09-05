namespace WCIDIDParserApp
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
            this.ParseWCIFile = new System.Windows.Forms.Button();
            this.ParseBWorksFile = new System.Windows.Forms.Button();
            this.ParseBVoxFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ParseWCIFile
            // 
            this.ParseWCIFile.Location = new System.Drawing.Point(87, 90);
            this.ParseWCIFile.Name = "ParseWCIFile";
            this.ParseWCIFile.Size = new System.Drawing.Size(124, 29);
            this.ParseWCIFile.TabIndex = 0;
            this.ParseWCIFile.Text = "ParseWCIFile";
            this.ParseWCIFile.UseVisualStyleBackColor = true;
            this.ParseWCIFile.Click += new System.EventHandler(this.ParseWCIFile_Click);
            // 
            // ParseBWorksFile
            // 
            this.ParseBWorksFile.Location = new System.Drawing.Point(87, 142);
            this.ParseBWorksFile.Name = "ParseBWorksFile";
            this.ParseBWorksFile.Size = new System.Drawing.Size(124, 36);
            this.ParseBWorksFile.TabIndex = 1;
            this.ParseBWorksFile.Text = "ParseBWorksFile";
            this.ParseBWorksFile.UseVisualStyleBackColor = true;
            this.ParseBWorksFile.Click += new System.EventHandler(this.ParseBWorksFile_Click);
            // 
            // ParseBVoxFile
            // 
            this.ParseBVoxFile.Location = new System.Drawing.Point(87, 201);
            this.ParseBVoxFile.Name = "ParseBVoxFile";
            this.ParseBVoxFile.Size = new System.Drawing.Size(124, 23);
            this.ParseBVoxFile.TabIndex = 2;
            this.ParseBVoxFile.Text = "ParseBVoxFile";
            this.ParseBVoxFile.UseVisualStyleBackColor = true;
            this.ParseBVoxFile.Click += new System.EventHandler(this.ParseBVoxFile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.ParseBVoxFile);
            this.Controls.Add(this.ParseBWorksFile);
            this.Controls.Add(this.ParseWCIFile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ParseWCIFile;
        private System.Windows.Forms.Button ParseBWorksFile;
        private System.Windows.Forms.Button ParseBVoxFile;
    }
}

