using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TruMobility.Sprint.AMS
{
    public partial class SftpSvc : ServiceBase
    {

        private AMSSftpMgr _mgr;

        public SftpSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _mgr = new AMSSftpMgr();
            _mgr.StartProcessing();
        }

        protected override void OnStop()
        {
            _mgr.StopProcessing();
        }

    }
}
