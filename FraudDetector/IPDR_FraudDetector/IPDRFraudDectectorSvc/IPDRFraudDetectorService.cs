using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TruMobility.Services.Fraud
{
    public partial class IPDRFraudDetectorService : ServiceBase
    {

        private IPDRFraudDetector _rSvc = new IPDRFraudDetector();
        public IPDRFraudDetectorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _rSvc.StartProcessing();
        }

        protected override void OnStop()
        {
            _rSvc.StopProcessing();
        }
    }
}
