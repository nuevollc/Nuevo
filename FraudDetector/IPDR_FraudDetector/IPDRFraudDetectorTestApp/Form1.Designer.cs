namespace TruMobility.Services.Fraud
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
            this.StartFraud = new System.Windows.Forms.Button();
            this.StopTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartFraud
            // 
            this.StartFraud.Location = new System.Drawing.Point(32, 46);
            this.StartFraud.Name = "StartFraud";
            this.StartFraud.Size = new System.Drawing.Size(198, 80);
            this.StartFraud.TabIndex = 0;
            this.StartFraud.Text = "StartFraud";
            this.StartFraud.UseVisualStyleBackColor = true;
            this.StartFraud.Click += new System.EventHandler(this.TestFraud_Click);
            // 
            // StopTest
            // 
            this.StopTest.Location = new System.Drawing.Point(32, 147);
            this.StopTest.Name = "StopTest";
            this.StopTest.Size = new System.Drawing.Size(198, 57);
            this.StopTest.TabIndex = 1;
            this.StopTest.Text = "StopTest";
            this.StopTest.UseVisualStyleBackColor = true;
            this.StopTest.Click += new System.EventHandler(this.StopTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.StopTest);
            this.Controls.Add(this.StartFraud);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartFraud;
        private System.Windows.Forms.Button StopTest;
    }
}

