using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Strata8.Provisioning.Tools;

namespace WCIDIDParserApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ParseWCIFile_Click(object sender, EventArgs e)
        {
            Strata8.Provisioning.Tools.WCIDIDParser p = new Strata8.Provisioning.Tools.WCIDIDParser();
            p.ProcessWCIFile();

        }

        private void ParseBWorksFile_Click(object sender, EventArgs e)
        {
            Strata8.Provisioning.Tools.BWorksDNListParser p = new BWorksDNListParser();
            p.ProcessFile();
        }

        private void ParseBVoxFile_Click(object sender, EventArgs e)
        {
            Strata8.Provisioning.Tools.BVoxDIDParser p = new BVoxDIDParser();
            p.ProcessFile();

        }
    }
}
