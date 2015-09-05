using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TruMobility.Sprint.AMS;

namespace AMSHandlerTestApp
{
    public partial class Form1 : Form
    {
        private AMSHandler _handler = null;
        private AMSSftpMgr _smgr = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, EventArgs e)
        {

            // this is the sftp manager, it gets the files
            //_smgr = new AMSSftpMgr();
            //_smgr.StartProcessing();

            // this is for the AMSHandler, this will process the files
             _handler = new AMSHandler();
            _handler.StartProcessing();

           // AMSFileReader rdr = new AMSFileReader();
            //rdr.ProcessTheFile(@"d:\apps\data\ams\amsTest.dat");

        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if ( _smgr != null )
                _smgr.StopProcessing();

            if (_handler != null )
                _handler.StopProcessing();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

    }
}
