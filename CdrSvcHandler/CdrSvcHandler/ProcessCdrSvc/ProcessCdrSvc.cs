using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;


namespace Strata8.Telephony.MiddleTier.Services.CDR
{
    public partial class ProcessCdrSvc : ServiceBase
    {
        private CDR.CdrHandler myHandler;

        public ProcessCdrSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.

            myHandler = new CdrHandler();
            myHandler.StartProcessing();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            myHandler.StopProcessing();
        }
    }
}
