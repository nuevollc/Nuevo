using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TruMobility.Reporting.CDR.Dept
{
    public partial class Form1 : Form
    {
        CallProcessor _p = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _p = new CallProcessor();
            _p.StartProcessing();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _p.StopProcessing();
        }
    }
}
