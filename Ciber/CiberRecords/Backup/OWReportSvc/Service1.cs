using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Strata8.Wireless.Cdr.Reporting;

namespace OWReportSvc
{
    public partial class OWReportSvc : ServiceBase
    {
        private static WirelessCdrProcessor m_Handler;

        public OWReportSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (m_Handler == null) 
                m_Handler = new WirelessCdrProcessor();
            m_Handler.StartProcessing();

        }

        protected override void OnStop()
        {
            if (m_Handler != null) 
                m_Handler.StopProcessing();

        }
    }
}
