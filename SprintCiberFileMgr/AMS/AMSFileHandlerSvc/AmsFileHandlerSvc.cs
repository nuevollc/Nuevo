using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TruMobility.Sprint.AMS
{
    public partial class AmsFileHandlerSvc : ServiceBase
    {
        AMSHandler _handler = new AMSHandler();
        public AmsFileHandlerSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _handler.StartProcessing();
        }

        protected override void OnStop()
        {
            _handler.StopProcessing();
        }
    }
}
