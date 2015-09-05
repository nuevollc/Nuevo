using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Strata8.Wireless.Utils;

namespace SftpTestApp
{
    public partial class Form1 : Form
    {
        private Strata8.Wireless.Utils.SftpHandler sftp = new SftpHandler();

        public Form1()
        {
            InitializeComponent();
        }

        private void SFTPFile_Click(object sender, EventArgs e)
        {
            if (sftp != null)
                sftp.StartProcessing();
        }

        private void StopSFTPHandler_Click(object sender, EventArgs e)
        {
            if (sftp != null)
                sftp.StopProcessing();
        }
    }
}
