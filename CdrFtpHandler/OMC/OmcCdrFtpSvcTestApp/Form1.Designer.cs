namespace Omc.Cdr.Ftp
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
            this.OmcCdrSvcTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OmcCdrSvcTest
            // 
            this.OmcCdrSvcTest.Location = new System.Drawing.Point(99, 99);
            this.OmcCdrSvcTest.Name = "OmcCdrSvcTest";
            this.OmcCdrSvcTest.Size = new System.Drawing.Size(86, 36);
            this.OmcCdrSvcTest.TabIndex = 0;
            this.OmcCdrSvcTest.Text = "OmcCdrSvcTest";
            this.OmcCdrSvcTest.UseVisualStyleBackColor = true;
            this.OmcCdrSvcTest.Click += new System.EventHandler(this.OmcCdrSvcTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.OmcCdrSvcTest);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OmcCdrSvcTest;

    }
}

