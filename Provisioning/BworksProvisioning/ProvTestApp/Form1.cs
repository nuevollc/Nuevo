using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using com.strata8.clients.oci;

using Strata8.Telephony.Provisioning.Services;

namespace ProvTestApp
{
    public partial class Form1 : Form
    {

        //private static OCIUtilClient ociUtilClient;
        private Strata8.Telephony.Provisioning.Services.BworksProvisioner bp = new BworksProvisioner();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, EventArgs e)
        {
            // bp.ProcessLogIn();
            // 1202776055759 : 5215137aefbadba6e4be95e70ee24cf4
            // 1202776155043 : fa1ec0c8421a99d3aff45ecd39725e86
            // 1203462160522 : 9040daff8f107aaa84e6f89dd496b3f6
            // bp.ProcessLogIn();

            string nonceValue = "1203462160522";
            string pass1 = bp.EncryptPassword(nonceValue);
            string pass2 = OCIUtilClient.ComputeMessageDigest("admin", nonceValue );

            // get/send the LoginRequest Message  to login to the platform
            string msg = bp.GetLoginMsg(pass2);
            OCIUtilClient.SendOCIMsg(msg);


        }
    }
}