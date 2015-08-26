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
    public partial class LTESftpMgrSvc : ServiceBase
    {
        private TruMobility.Network.Services.LTESftpMgr m_mgr;

        public LTESftpMgrSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_mgr = new LTESftpMgr();
            m_mgr.StartProcessing();
        }

        protected override void OnStop()
        {
            m_mgr.StopProcessing();
        }
    }
}
