using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Strata8.CDR.Reporting;

namespace RedaptCallReportSvc
{
    public partial class RedaptCallReportSvc : ServiceBase
    {
        private Strata8.CDR.Reporting.RedaptCdrProcessor rProcessor = null;

        public RedaptCallReportSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            rProcessor = new RedaptCdrProcessor();
            rProcessor.StartProcessing();
        }

        protected override void OnStop()
        {
            if ( rProcessor != null )
                rProcessor.StopProcessing();
        }
    }
}
