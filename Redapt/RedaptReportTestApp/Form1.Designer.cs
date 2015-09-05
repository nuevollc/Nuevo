namespace RedaptReportTestApp
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
            this.GenerateCallReport = new System.Windows.Forms.Button();
            this.StartThread = new System.Windows.Forms.Button();
            this.StopThread = new System.Windows.Forms.Button();
            this.GenerateTotalCallReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GenerateCallReport
            // 
            this.GenerateCallReport.Location = new System.Drawing.Point(89, 146);
            this.GenerateCallReport.Name = "GenerateCallReport";
            this.GenerateCallReport.Size = new System.Drawing.Size(129, 23);
            this.GenerateCallReport.TabIndex = 0;
            this.GenerateCallReport.Text = "GenerateCallReport";
            this.GenerateCallReport.UseVisualStyleBackColor = true;
            this.GenerateCallReport.Click += new System.EventHandler(this.GenerateCallReport_Click);
            // 
            // StartThread
            // 
            this.StartThread.Location = new System.Drawing.Point(89, 79);
            this.StartThread.Name = "StartThread";
            this.StartThread.Size = new System.Drawing.Size(75, 23);
            this.StartThread.TabIndex = 1;
            this.StartThread.Text = "StartThread";
            this.StartThread.UseVisualStyleBackColor = true;
            this.StartThread.Click += new System.EventHandler(this.StartThread_Click);
            // 
            // StopThread
            // 
            this.StopThread.Location = new System.Drawing.Point(89, 108);
            this.StopThread.Name = "StopThread";
            this.StopThread.Size = new System.Drawing.Size(75, 23);
            this.StopThread.TabIndex = 2;
            this.StopThread.Text = "StopThread";
            this.StopThread.UseVisualStyleBackColor = true;
            this.StopThread.Click += new System.EventHandler(this.StopThread_Click);
            // 
            // GenerateTotalCallReport
            // 
            this.GenerateTotalCallReport.Location = new System.Drawing.Point(89, 198);
            this.GenerateTotalCallReport.Name = "GenerateTotalCallReport";
            this.GenerateTotalCallReport.Size = new System.Drawing.Size(128, 23);
            this.GenerateTotalCallReport.TabIndex = 3;
            this.GenerateTotalCallReport.Text = "Unassigned";
            this.GenerateTotalCallReport.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.GenerateTotalCallReport);
            this.Controls.Add(this.StopThread);
            this.Controls.Add(this.StartThread);
            this.Controls.Add(this.GenerateCallReport);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button GenerateCallReport;
        private System.Windows.Forms.Button StartThread;
        private System.Windows.Forms.Button StopThread;
        private System.Windows.Forms.Button GenerateTotalCallReport;
    }
}

