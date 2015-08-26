namespace TruMobility.Reporting.CDR
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
            this.StartDailyCallReport = new System.Windows.Forms.Button();
            this.WholesaleDataProcessor = new System.Windows.Forms.Button();
            this.ReportFromFile = new System.Windows.Forms.Button();
            this.GroupCallReport = new System.Windows.Forms.Button();
            this.StartUserCallReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StartDailyCallReport
            // 
            this.StartDailyCallReport.Location = new System.Drawing.Point(81, 182);
            this.StartDailyCallReport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.StartDailyCallReport.Name = "StartDailyCallReport";
            this.StartDailyCallReport.Size = new System.Drawing.Size(201, 41);
            this.StartDailyCallReport.TabIndex = 0;
            this.StartDailyCallReport.Text = "StartDailyCallReport";
            this.StartDailyCallReport.UseVisualStyleBackColor = true;
            this.StartDailyCallReport.Click += new System.EventHandler(this.StartDailyCallReport_Click);
            // 
            // WholesaleDataProcessor
            // 
            this.WholesaleDataProcessor.Location = new System.Drawing.Point(81, 126);
            this.WholesaleDataProcessor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.WholesaleDataProcessor.Name = "WholesaleDataProcessor";
            this.WholesaleDataProcessor.Size = new System.Drawing.Size(201, 41);
            this.WholesaleDataProcessor.TabIndex = 1;
            this.WholesaleDataProcessor.Text = "WholesaleDataProcessor";
            this.WholesaleDataProcessor.UseVisualStyleBackColor = true;
            this.WholesaleDataProcessor.Click += new System.EventHandler(this.WholesaleDataProcessor_Click);
            // 
            // ReportFromFile
            // 
            this.ReportFromFile.Location = new System.Drawing.Point(81, 64);
            this.ReportFromFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ReportFromFile.Name = "ReportFromFile";
            this.ReportFromFile.Size = new System.Drawing.Size(201, 46);
            this.ReportFromFile.TabIndex = 2;
            this.ReportFromFile.Text = "ReportFromFile";
            this.ReportFromFile.UseVisualStyleBackColor = true;
            this.ReportFromFile.Click += new System.EventHandler(this.ReportFromFile_Click);
            // 
            // GroupCallReport
            // 
            this.GroupCallReport.Location = new System.Drawing.Point(81, 230);
            this.GroupCallReport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GroupCallReport.Name = "GroupCallReport";
            this.GroupCallReport.Size = new System.Drawing.Size(179, 58);
            this.GroupCallReport.TabIndex = 3;
            this.GroupCallReport.Text = "GroupCallReport";
            this.GroupCallReport.UseVisualStyleBackColor = true;
            this.GroupCallReport.Click += new System.EventHandler(this.GroupCallReport_Click);
            // 
            // StartUserCallReport
            // 
            this.StartUserCallReport.Location = new System.Drawing.Point(81, 309);
            this.StartUserCallReport.Name = "StartUserCallReport";
            this.StartUserCallReport.Size = new System.Drawing.Size(201, 58);
            this.StartUserCallReport.TabIndex = 4;
            this.StartUserCallReport.Text = "StartUserCallReport";
            this.StartUserCallReport.UseVisualStyleBackColor = true;
            this.StartUserCallReport.Click += new System.EventHandler(this.StartUserCallReport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 452);
            this.Controls.Add(this.StartUserCallReport);
            this.Controls.Add(this.GroupCallReport);
            this.Controls.Add(this.ReportFromFile);
            this.Controls.Add(this.WholesaleDataProcessor);
            this.Controls.Add(this.StartDailyCallReport);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartDailyCallReport;
        private System.Windows.Forms.Button WholesaleDataProcessor;
        private System.Windows.Forms.Button ReportFromFile;
        private System.Windows.Forms.Button GroupCallReport;
        private System.Windows.Forms.Button StartUserCallReport;
    }
}

