using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Strata8.Wireless.Cdr
{
    public partial class OmcFtpSvc : ServiceBase
    {

        private static OmcFtpHandler m_Handler;
        public OmcFtpSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            if ( m_Handler == null ) 
                m_Handler = new OmcFtpHandler();
            m_Handler.StartProcessing();

        }

        protected override void OnStop()
        {
            if (m_Handler != null)
                m_Handler.StopProcessing();

        }
    }
}
