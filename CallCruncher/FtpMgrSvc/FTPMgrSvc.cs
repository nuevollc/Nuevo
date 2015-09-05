using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Nuevo.CallCruncher.CDR.FTP
{
    public partial class FTPMgrSvc : ServiceBase
    {
        private FtpMgr _mgr = null; 
        public FTPMgrSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _mgr = new FtpMgr();
            _mgr.StartProcessing();
        }

        protected override void OnStop()
        {
            if (!_mgr.Equals(null))
                _mgr.StopProcessing();
        }
    }
}
