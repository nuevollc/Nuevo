using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Strata8.Telephony.MiddleTier.Services.CDR;

namespace CdrProcessorTestApp
{
    public partial class Form1 : Form
    {
        private Strata8.Telephony.MiddleTier.Services.CDR.CdrHandler m_Handler;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_Handler = new CdrHandler();
            if (m_Handler != null)
                m_Handler.StartProcessing();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_Handler != null)
                m_Handler.StopProcessing();
        }

        private void ServiceProviderCdrHandler_Click(object sender, EventArgs e)
        {
            ServiceProviderCdrHandler sp = new ServiceProviderCdrHandler();
            FtpSiteInfo si = new FtpSiteInfo();
            si.ServiceProvider = "WCI_sp";
            si.Site = "cdrftp01-sttlwa.strata8.net";
            si.Password = "wci3pcs2010!";
            si.Username = "wci";

            //si.Site = "208.99.195.208";
           // si.Password = "neworld";
            //si.Username = "papi";

            si.Filename = @"c:\logs\CDRFile01.csv";
            sp.PostFileToSite(si);
            //sp.Upload(si);

            //sp.ProcessServiceProviderCdrs( @"c:\logs\CDRFile01" , "test", "Premiere_sp" );

        }
    }
}