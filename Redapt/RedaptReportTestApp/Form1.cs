using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Strata8.CDR.Reporting;

namespace RedaptReportTestApp
{
    public partial class Form1 : Form
    {
        private DMCdrProcessor rp = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartThread_Click(object sender, EventArgs e)
        {
            if ( rp == null )
                rp = new DMCdrProcessor();
            rp.StartProcessing();
        }

        private void StopThread_Click(object sender, EventArgs e)
        {
            rp.StopProcessing();
        }

        private void GenerateCallReport_Click(object sender, EventArgs e)
        { 
            // add code to generate the call report
        }

    }
}
