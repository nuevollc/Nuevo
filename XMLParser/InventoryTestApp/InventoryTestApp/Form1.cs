using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Strata8.Provisioning;

using Strata8.Wireless.Data;

namespace InventoryTestApp
{
    public partial class Form1 : Form
    {

        private Strata8.Provisioning.Reports.InventoryReportParser p = new Strata8.Provisioning.Reports.InventoryReportParser();

        public Form1()
        {
            InitializeComponent();
        }

        private void BeginParse_Click(object sender, EventArgs e)
        {
            p.ProcessInventoryReport();
        }

        private void ParseVerizonTDS_Click(object sender, EventArgs e)
        {
            TechDataSheetProcessor tds = new TechDataSheetProcessor();
            tds.ProcessTheFile();
        }


    }
}
