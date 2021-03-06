﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Strata8.Wireless.Cdr
{
    public partial class OmcCdrSvc : ServiceBase
    {

        private OmcCdrHandler m_Handler = null;

        public OmcCdrSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_Handler = new OmcCdrHandler();
            m_Handler.StartProcessing();
        }

        protected override void OnStop()
        {
            m_Handler.StopProcessing();
        }
    }
}
