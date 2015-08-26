using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Strata8.Test;

namespace TestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SFTPStart_Click(object sender, EventArgs e)
        {
            TestClass ts = new TestClass();
            ts.TestSftp();


        }
    }
}
