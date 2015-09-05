using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace TruMobility.Services.Fraud
{
    public partial class FraudService : ServiceBase
    {
        private FraudDetector myFraudDetector;
        public FraudService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            myFraudDetector = new FraudDetector();
            myFraudDetector.StartProcessing();

        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            myFraudDetector.StopProcessing();
        
        }
    }
}
