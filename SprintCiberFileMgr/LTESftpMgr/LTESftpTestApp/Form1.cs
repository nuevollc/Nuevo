using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LTESftpTestApp
{
    public partial class Form1 : Form
    {
        private TruMobility.Network.Services.LTESftpMgr s = new TruMobility.Network.Services.LTESftpMgr();
        public Form1()
        {
            InitializeComponent();
        }

        private void LTESftpTest_Click(object sender, EventArgs e)
        {
            s.StartProcessing();
        }

    }
}
