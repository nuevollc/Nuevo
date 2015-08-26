using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omc.Cdr.Ftp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OmcCdrSvcTest_Click(object sender, EventArgs e)
        {
            OmcCdrFtpHandler ftp = new OmcCdrFtpHandler();
            ftp.ProcessFileControlThread();
        }
    }
}
