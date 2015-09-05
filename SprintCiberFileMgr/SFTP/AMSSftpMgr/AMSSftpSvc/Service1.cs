using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TruMobility.Network.Services
{
    public partial class Service1 : ServiceBase
    {
        private AMSSftpMgr _mgr = null;
        public Service1()
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
            if (_mgr != null)
                _mgr.StopProcessing();
        }
    }
}
