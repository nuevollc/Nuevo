﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TruMobility.Network.Services
{
    public partial class Form1 : Form
    {
        private TruMobility.Network.Services.AMSSftpMgr a = new TruMobility.Network.Services.AMSSftpMgr();
        public Form1()
        {
            InitializeComponent();
        }

        private void StartAMSSftpSvc_Click(object sender, EventArgs e)
        {
            a.StartProcessing();
        }
    }
}
