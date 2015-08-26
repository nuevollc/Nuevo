using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using CdrFtpHandler;

namespace CdrFtpHandlerSvc
{
    public partial class CdrFtpSvc : ServiceBase
    {
        private FtpHandler myHandler;
        public CdrFtpSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            myHandler = new FtpHandler();
            myHandler.StartProcessing();
        }

        protected override void OnStop()
        {
            myHandler.StopProcessing();
        }
    }
}
