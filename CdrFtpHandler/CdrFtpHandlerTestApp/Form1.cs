using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CdrFtpHandlerTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void TestFtp_Click(object sender, EventArgs e)
        {
            CdrFtpHandler.FtpHandler ftp = new CdrFtpHandler.FtpHandler();
            ftp.ProcessFileControlThread();
        }
    }
}
