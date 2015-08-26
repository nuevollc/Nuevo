using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TruMobility.Services.Fraud
{
    public partial class Form1 : Form
    {
        IPDRFraudDetector _fd = new IPDRFraudDetector();
        public Form1()
        {
            InitializeComponent();
        }

        private void TestFraud_Click(object sender, EventArgs e)
        {
            
            _fd.StartProcessing();
        }

        private void StopTest_Click(object sender, EventArgs e)
        {
            _fd.StartProcessing();
        }
    }
}
