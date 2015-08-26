using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Strata8.Wireless.Cdr.Ciber;

namespace OmcCiberProcessorSvc
{
    public partial class OmcCiberProcessor : ServiceBase
    {
        private OmcCiberHandler m_Handler;

        public OmcCiberProcessor()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_Handler = new OmcCiberHandler();
            m_Handler.StartProcessing();
        }

        protected override void OnStop()
        {
            m_Handler.StopProcessing();
        }
    }
}
