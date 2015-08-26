using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using XSITest.Test;

namespace BroadsoftXSITestApp
{
    public partial class Form1 : Form
    {
        private XSITestMgr _mgr = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void POST_IT_Click(object sender, EventArgs e)
        {
            XSITestMgr.CheckIt("one", "two");
        }
    }
}
