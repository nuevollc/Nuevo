using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TruMobility.Reporting.Sprint;
using TruMobility.Reporting.CDR.Groups;


namespace TruMobility.Reporting.CDR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void StartDailyCallReport_Click(object sender, EventArgs e)
        {
            CdrProcessor p = new CdrProcessor();
            p.TestCallReport();
        }

        private void WholesaleDataProcessor_Click(object sender, EventArgs e)
        {
            WholeSaleDataProcessor dp = new WholeSaleDataProcessor();
            dp.CreateWholeSaleReport();
        }

        private void ReportFromFile_Click(object sender, EventArgs e)
        {
            WholeSaleDataProcessor dp = new WholeSaleDataProcessor();
            dp.CreateReportFromFile(@"C:\Users\rhernandez\Google Drive\TruMobility\Billing\Invoices\2011\SprintWholesale/Sprint04_March2011_Usage.txt");

        }

        private void GroupCallReport_Click(object sender, EventArgs e)
        {
            GroupReportProcessor p = new GroupReportProcessor();
            p.TestCallReport();
            // p.StartProcessing();
            
        }

        private void StartUserCallReport_Click(object sender, EventArgs e)
        {
            UserCallReportProcessor p = new UserCallReportProcessor();
            p.StartProcessing();
        }
    }
}
