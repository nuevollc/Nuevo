using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Strata8.CDR.Reporting;

namespace S8ReportSvc
{
    public partial class Service1 : ServiceBase
    {
        private Strata8.CDR.Reporting.S8CdrProcessor rProcessor = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            rProcessor = new S8CdrProcessor();
            rProcessor.StartProcessing();
        }

        protected override void OnStop()
        {
            if (rProcessor != null)
                rProcessor.StopProcessing();
        }
    }
}
