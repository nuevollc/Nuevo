using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Omc.Cdr.Ftp
{
    public partial class OmcCdrFtpSvc : ServiceBase
    {
        private OmcCdrFtpHandler _handler;

        public OmcCdrFtpSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _handler = new OmcCdrFtpHandler();
            _handler.StartProcessing();
        }

        protected override void OnStop()
        {
            _handler.StopProcessing();
        }
    }
}
