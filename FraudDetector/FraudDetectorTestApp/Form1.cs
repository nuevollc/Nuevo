using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TruMobility.Services.Fraud
{
    public partial class Form1 : Form
    {
        private FraudDetector _f = new FraudDetector();
        public Form1()
        {
            InitializeComponent();
        }

        private void StartFraudDetector_Click(object sender, EventArgs e)
        {
            // get the cdr dataset every 20 mins
            int numberOfInternationalCalls = _f.GetInternationalCalls();

            // get all calls for this interval to log
            // add code later to work on the same dataset already in memory
            // TODO::
            int numberOfTotalCalls = _f.GetTotalCalls();

            // apply fraud rules and take action if fraud detected
            string msg = _f.CheckCalls(numberOfTotalCalls, numberOfInternationalCalls);

            // we always send out the call report notification
            // for now manual fraud detection, send an email out
            if (!msg.Equals(string.Empty))
                _f.SendCallReportNotification(numberOfInternationalCalls, numberOfTotalCalls, msg);
        }
    }
}
