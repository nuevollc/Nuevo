using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TruMobility.Reporting.CDR.Dept
{
    public partial class Service1 : ServiceBase
    {
        CallProcessor _proc = null;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _proc = new CallProcessor();
            _proc.StartProcessing();
        }

        protected override void OnStop()
        {
            if (_proc != null )
            _proc.StopProcessing();
        }
    }
}
