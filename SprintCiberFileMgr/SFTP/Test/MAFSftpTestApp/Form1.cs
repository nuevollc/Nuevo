using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TruMobility.Network.Services;

namespace MAFSftpTestApp
{
    public partial class Form1 : Form
    {
        private TruMobility.Network.Services.SftpMgr s = new TruMobility.Network.Services.SftpMgr();
        public Form1()
        {
            InitializeComponent();
        }

        private void GetFileViaSFTP_Click(object sender, EventArgs e)
        {

            s.StartProcessing();
           //  s.GetFiles();


        }

        private void StopProcessor_Click(object sender, EventArgs e)
        {
            if (s != null )
                s.StopProcessing();
            
        }


 
    }
}
