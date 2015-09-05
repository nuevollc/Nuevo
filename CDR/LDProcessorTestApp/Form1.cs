using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TruMobility.CDR.LD;

namespace LDProcessorTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GetLDMins_Click(object sender, EventArgs e)
        {
            LDProcessor ld = new LDProcessor();
            ld.ProcessCalls();

        }

        private void GetWebResponse_Click(object sender, EventArgs e)
        {
           //string r = LDCalculator.CheckIt();
        }
    }
}
