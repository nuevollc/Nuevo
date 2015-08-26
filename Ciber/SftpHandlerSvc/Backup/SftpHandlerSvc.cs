using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;


namespace Strata8.Wireless.Utils
{
    public partial class SftpHandlerSvc : ServiceBase
    {

        private SftpHandler m_Handler;

        public SftpHandlerSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_Handler = new SftpHandler();
            m_Handler.StartProcessing();
        }

        protected override void OnStop()
        {
            m_Handler.StopProcessing();
        }

    }
}
