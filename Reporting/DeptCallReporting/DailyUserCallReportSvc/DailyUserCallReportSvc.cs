using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TruMobility.Reporting.CDR.Groups
{
    public partial class DailyUserCallReportSvc : ServiceBase
    {

        // our processor
        private UserCallReportProcessor _proc = null;

        public DailyUserCallReportSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _proc = new UserCallReportProcessor();
            _proc.StartProcessing();
        }

        protected override void OnStop()
        {
            _proc.StopProcessing();
        }
    }
}
