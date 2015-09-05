using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VoiceMailProvisioningTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string phoneNumber = "2062106969";
            VMInterface vm = new VMInterface();
            vm.ProvisionVoiceMailUser(phoneNumber);
        }
    }
}
