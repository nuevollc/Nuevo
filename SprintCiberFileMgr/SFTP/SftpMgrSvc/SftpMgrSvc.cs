using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace TruMobility.Network.Services
{
    public partial class SftpMgrSvc : ServiceBase
    {
        private TruMobility.Network.Services.SftpMgr m_mgr;

        public SftpMgrSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_mgr = new SftpMgr();
            m_mgr.StartProcessing();
        }

        protected override void OnStop()
        {
            m_mgr.StopProcessing();
        }
    }
}
