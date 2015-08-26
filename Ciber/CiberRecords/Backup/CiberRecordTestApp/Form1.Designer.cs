namespace CiberRecordTestApp
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
            this.ReadCiberRecord = new System.Windows.Forms.Button();
            this.FtpStart = new System.Windows.Forms.Button();
            this.FTPStop = new System.Windows.Forms.Button();
            this.StartOmcCdrProcessorSvc = new System.Windows.Forms.Button();
            this.StopOmcCdrHandler = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.ReadSyniverseSampleFile = new System.Windows.Forms.Button();
            this.ImportNpaNxxData = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.StartProcessingCdrToCiber = new System.Windows.Forms.Button();
            this.StopProcessingCdrToCiber = new System.Windows.Forms.Button();
            this.CreateBworksCdr = new System.Windows.Forms.Button();
            this.CreateCsvReport = new System.Windows.Forms.Button();
            this.CreateOWReport = new System.Windows.Forms.Button();
            this.StopOWReportSvc = new System.Windows.Forms.Button();
            this.StartDirListAndParseOmcCdrFiles = new System.Windows.Forms.Button();
            this.StopDirListAndParseOmcCdrFiles = new System.Windows.Forms.Button();
            this.PicoCellReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ReadCiberRecord
            // 
            this.ReadCiberRecord.Location = new System.Drawing.Point(324, 221);
            this.ReadCiberRecord.Name = "ReadCiberRecord";
            this.ReadCiberRecord.Size = new System.Drawing.Size(188, 23);
            this.ReadCiberRecord.TabIndex = 0;
            this.ReadCiberRecord.Text = "ReadCiberRecord";
            this.ReadCiberRecord.UseVisualStyleBackColor = true;
            this.ReadCiberRecord.Click += new System.EventHandler(this.ReadCiberRecord_Click);
            // 
            // FtpStart
            // 
            this.FtpStart.Location = new System.Drawing.Point(185, 24);
            this.FtpStart.Name = "FtpStart";
            this.FtpStart.Size = new System.Drawing.Size(75, 23);
            this.FtpStart.TabIndex = 2;
            this.FtpStart.Text = "FTPStart";
            this.FtpStart.UseVisualStyleBackColor = true;
            this.FtpStart.Click += new System.EventHandler(this.FtpTest_Click);
            // 
            // FTPStop
            // 
            this.FTPStop.Location = new System.Drawing.Point(185, 53);
            this.FTPStop.Name = "FTPStop";
            this.FTPStop.Size = new System.Drawing.Size(75, 23);
            this.FTPStop.TabIndex = 3;
            this.FTPStop.Text = "FTPStop";
            this.FTPStop.UseVisualStyleBackColor = true;
            this.FTPStop.Click += new System.EventHandler(this.FTPStop_Click);
            // 
            // StartOmcCdrProcessorSvc
            // 
            this.StartOmcCdrProcessorSvc.Location = new System.Drawing.Point(351, 329);
            this.StartOmcCdrProcessorSvc.Name = "StartOmcCdrProcessorSvc";
            this.StartOmcCdrProcessorSvc.Size = new System.Drawing.Size(161, 23);
            this.StartOmcCdrProcessorSvc.TabIndex = 4;
            this.StartOmcCdrProcessorSvc.Text = "StartOmcCdrProcessorSvc";
            this.StartOmcCdrProcessorSvc.UseVisualStyleBackColor = true;
            this.StartOmcCdrProcessorSvc.Click += new System.EventHandler(this.StartOmcCdrProcessorSvc_Click);
            // 
            // StopOmcCdrHandler
            // 
            this.StopOmcCdrHandler.Location = new System.Drawing.Point(351, 358);
            this.StopOmcCdrHandler.Name = "StopOmcCdrHandler";
            this.StopOmcCdrHandler.Size = new System.Drawing.Size(161, 23);
            this.StopOmcCdrHandler.TabIndex = 5;
            this.StopOmcCdrHandler.Text = "StopOmcCdrHandler";
            this.StopOmcCdrHandler.UseVisualStyleBackColor = true;
            this.StopOmcCdrHandler.Click += new System.EventHandler(this.OmcCdrHandlerStop_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(104, 320);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(197, 21);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "OmcCdrHandlerSvc Start/Stop";
            // 
            // ReadSyniverseSampleFile
            // 
            this.ReadSyniverseSampleFile.Location = new System.Drawing.Point(324, 250);
            this.ReadSyniverseSampleFile.Name = "ReadSyniverseSampleFile";
            this.ReadSyniverseSampleFile.Size = new System.Drawing.Size(188, 23);
            this.ReadSyniverseSampleFile.TabIndex = 7;
            this.ReadSyniverseSampleFile.Text = "ReadSyniverseSampleFile";
            this.ReadSyniverseSampleFile.UseVisualStyleBackColor = true;
            this.ReadSyniverseSampleFile.Click += new System.EventHandler(this.ReadSyniverseSampleFile_Click);
            // 
            // ImportNpaNxxData
            // 
            this.ImportNpaNxxData.Location = new System.Drawing.Point(144, 140);
            this.ImportNpaNxxData.Name = "ImportNpaNxxData";
            this.ImportNpaNxxData.Size = new System.Drawing.Size(116, 23);
            this.ImportNpaNxxData.TabIndex = 8;
            this.ImportNpaNxxData.Text = "ImportNpaNxxData";
            this.ImportNpaNxxData.UseVisualStyleBackColor = true;
            this.ImportNpaNxxData.Click += new System.EventHandler(this.ImportNpaNxxData_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.Location = new System.Drawing.Point(104, 223);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(197, 21);
            this.richTextBox2.TabIndex = 9;
            this.richTextBox2.Text = "One Time Ciber Processing";
            // 
            // StartProcessingCdrToCiber
            // 
            this.StartProcessingCdrToCiber.Location = new System.Drawing.Point(324, 157);
            this.StartProcessingCdrToCiber.Name = "StartProcessingCdrToCiber";
            this.StartProcessingCdrToCiber.Size = new System.Drawing.Size(188, 23);
            this.StartProcessingCdrToCiber.TabIndex = 10;
            this.StartProcessingCdrToCiber.Text = "StartProcessingCdrToCiber";
            this.StartProcessingCdrToCiber.UseVisualStyleBackColor = true;
            this.StartProcessingCdrToCiber.Click += new System.EventHandler(this.ProcessMulitpleSidBids_Click);
            // 
            // StopProcessingCdrToCiber
            // 
            this.StopProcessingCdrToCiber.Location = new System.Drawing.Point(324, 186);
            this.StopProcessingCdrToCiber.Name = "StopProcessingCdrToCiber";
            this.StopProcessingCdrToCiber.Size = new System.Drawing.Size(188, 23);
            this.StopProcessingCdrToCiber.TabIndex = 11;
            this.StopProcessingCdrToCiber.Text = "StopProcessingCdrToCiber";
            this.StopProcessingCdrToCiber.UseVisualStyleBackColor = true;
            this.StopProcessingCdrToCiber.Click += new System.EventHandler(this.StopProcessingCdrToCiber_Click);
            // 
            // CreateBworksCdr
            // 
            this.CreateBworksCdr.Location = new System.Drawing.Point(386, 40);
            this.CreateBworksCdr.Name = "CreateBworksCdr";
            this.CreateBworksCdr.Size = new System.Drawing.Size(126, 23);
            this.CreateBworksCdr.TabIndex = 12;
            this.CreateBworksCdr.Text = "CreateBworksCdr";
            this.CreateBworksCdr.UseVisualStyleBackColor = true;
            this.CreateBworksCdr.Click += new System.EventHandler(this.CreateBworksCdr_Click);
            // 
            // CreateCsvReport
            // 
            this.CreateCsvReport.Location = new System.Drawing.Point(0, 0);
            this.CreateCsvReport.Name = "CreateCsvReport";
            this.CreateCsvReport.Size = new System.Drawing.Size(75, 23);
            this.CreateCsvReport.TabIndex = 18;
            // 
            // CreateOWReport
            // 
            this.CreateOWReport.Location = new System.Drawing.Point(386, 100);
            this.CreateOWReport.Name = "CreateOWReport";
            this.CreateOWReport.Size = new System.Drawing.Size(125, 23);
            this.CreateOWReport.TabIndex = 14;
            this.CreateOWReport.Text = "CreateOWReport";
            this.CreateOWReport.UseVisualStyleBackColor = true;
            this.CreateOWReport.Click += new System.EventHandler(this.CreateOWReport_Click);
            // 
            // StopOWReportSvc
            // 
            this.StopOWReportSvc.Location = new System.Drawing.Point(386, 130);
            this.StopOWReportSvc.Name = "StopOWReportSvc";
            this.StopOWReportSvc.Size = new System.Drawing.Size(124, 23);
            this.StopOWReportSvc.TabIndex = 15;
            this.StopOWReportSvc.Text = "StopOWReportSvc";
            this.StopOWReportSvc.UseVisualStyleBackColor = true;
            this.StopOWReportSvc.Click += new System.EventHandler(this.StopOWReportSvc_Click);
            // 
            // StartDirListAndParseOmcCdrFiles
            // 
            this.StartDirListAndParseOmcCdrFiles.Location = new System.Drawing.Point(344, 413);
            this.StartDirListAndParseOmcCdrFiles.Name = "StartDirListAndParseOmcCdrFiles";
            this.StartDirListAndParseOmcCdrFiles.Size = new System.Drawing.Size(166, 23);
            this.StartDirListAndParseOmcCdrFiles.TabIndex = 16;
            this.StartDirListAndParseOmcCdrFiles.Text = "StartDirListAndParseOmcCdrFiles";
            this.StartDirListAndParseOmcCdrFiles.UseVisualStyleBackColor = true;
            this.StartDirListAndParseOmcCdrFiles.Click += new System.EventHandler(this.DirListAndParseOmcCdrFiles_Click);
            // 
            // StopDirListAndParseOmcCdrFiles
            // 
            this.StopDirListAndParseOmcCdrFiles.Location = new System.Drawing.Point(344, 453);
            this.StopDirListAndParseOmcCdrFiles.Name = "StopDirListAndParseOmcCdrFiles";
            this.StopDirListAndParseOmcCdrFiles.Size = new System.Drawing.Size(165, 23);
            this.StopDirListAndParseOmcCdrFiles.TabIndex = 17;
            this.StopDirListAndParseOmcCdrFiles.Text = "StopDirListAndParseOmcCdrFiles";
            this.StopDirListAndParseOmcCdrFiles.UseVisualStyleBackColor = true;
            this.StopDirListAndParseOmcCdrFiles.Click += new System.EventHandler(this.StopDirListAndParseOmcCdrFiles_Click);
            // 
            // PicoCellReport
            // 
            this.PicoCellReport.Location = new System.Drawing.Point(386, 70);
            this.PicoCellReport.Name = "PicoCellReport";
            this.PicoCellReport.Size = new System.Drawing.Size(123, 23);
            this.PicoCellReport.TabIndex = 19;
            this.PicoCellReport.Text = "PicoCellReport";
            this.PicoCellReport.UseVisualStyleBackColor = true;
            this.PicoCellReport.Click += new System.EventHandler(this.PicoCellReport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 496);
            this.Controls.Add(this.PicoCellReport);
            this.Controls.Add(this.StopDirListAndParseOmcCdrFiles);
            this.Controls.Add(this.StartDirListAndParseOmcCdrFiles);
            this.Controls.Add(this.StopOWReportSvc);
            this.Controls.Add(this.CreateOWReport);
            this.Controls.Add(this.CreateCsvReport);
            this.Controls.Add(this.CreateBworksCdr);
            this.Controls.Add(this.StopProcessingCdrToCiber);
            this.Controls.Add(this.StartProcessingCdrToCiber);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.ImportNpaNxxData);
            this.Controls.Add(this.ReadSyniverseSampleFile);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.StopOmcCdrHandler);
            this.Controls.Add(this.StartOmcCdrProcessorSvc);
            this.Controls.Add(this.FTPStop);
            this.Controls.Add(this.FtpStart);
            this.Controls.Add(this.ReadCiberRecord);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ReadCiberRecord;
        private System.Windows.Forms.Button FtpStart;
        private System.Windows.Forms.Button FTPStop;
        private System.Windows.Forms.Button StartOmcCdrProcessorSvc;
        private System.Windows.Forms.Button StopOmcCdrHandler;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button ReadSyniverseSampleFile;
        private System.Windows.Forms.Button ImportNpaNxxData;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Button StartProcessingCdrToCiber;
        private System.Windows.Forms.Button StopProcessingCdrToCiber;
        private System.Windows.Forms.Button CreateBworksCdr;
        private System.Windows.Forms.Button CreateCsvReport;
        private System.Windows.Forms.Button CreateOWReport;
        private System.Windows.Forms.Button StopOWReportSvc;
        private System.Windows.Forms.Button StartDirListAndParseOmcCdrFiles;
        private System.Windows.Forms.Button StopDirListAndParseOmcCdrFiles;
        private System.Windows.Forms.Button PicoCellReport;
    }
}

