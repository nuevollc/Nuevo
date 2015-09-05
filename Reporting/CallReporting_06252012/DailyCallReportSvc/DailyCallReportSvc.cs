using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TruMobility.Reporting.CDR
{
    public partial class DailyCallReportSvc : ServiceBase
    {
        private CdrProcessor _proc = null;

        public DailyCallReportSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _proc = new CdrProcessor();
            _proc.StartProcessing();
        }

        protected override void OnStop()
        {
            if (_proc != null )
                _proc.StopProcessing();
        }
    }
}
