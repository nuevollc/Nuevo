using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using TruMobility.Reporting.CDR;

namespace TruMobility.Reporting.CDR.Groups
{
    public partial class DailyGroupCallReportSvc : ServiceBase
    {
        // our processor
        private GroupReportProcessor _proc = null;

        public DailyGroupCallReportSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _proc = new GroupReportProcessor();
            _proc.StartProcessing();
        }

        protected override void OnStop()
        {
            if (_proc != null)
                _proc.StopProcessing();

        }
    }
}
