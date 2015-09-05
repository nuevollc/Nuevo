
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Web.Services;
using System.Web.Services.Protocols;

using BworksProvisioning.net.strata8.lab.ews1;

namespace Strata8.Telephony.Provisioning.Services
{

    /// <summary>
    /// class to communicate to the Broadworks OCI interface
    /// 
    /// </summary>
    public  class BworksService
    {
        private static
            BworksProvisioning.net.strata8.lab.ews1.BWProvisioningServiceService bp = null;

        private static SoapHttpClientProtocol scp = null;

        public BworksService()
        {
            bp = new BWProvisioningServiceService();
            //scp = new SoapHttpClientProtocol();
             
        }

        public string sendMsg(string msg)
        {
            string xmlResponse = String.Empty;
            try
            {
                // bp = new BWProvisioningServiceService();
 
                // we use our static service to send message
                 xmlResponse = bp.processOCIMessage(msg);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("ECaught:" + ex.Message + "  StackTrace: " + ex.StackTrace);
            }

            return xmlResponse;

        }//sendMsg
    }
}//namespace
