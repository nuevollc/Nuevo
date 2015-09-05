using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using TruMobility.Utils.Logging;

namespace XSITest.Test
{
    public class XSITestMgr
    {
        private static string _request = "http://www.localcallingguide.com/xmlrcdist.php?";

        public static bool CheckIt(string npaNxx1, string npaNxx2)
        {
            string isLocal = String.Empty;

            // Create a request for the URL. 		
            //WebRequest request = WebRequest.Create("https://xsp2dmz.voip.mobileconverged.net/com.broadsoft.xsi-actions/v2.0/user/12062101077/directories/calllogs/missed");
            WebRequest request = WebRequest.Create(_request );
            request.
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            switch (response.StatusCode)
            {
                case (HttpStatusCode.OK):
                    // we are good to go
                    // Get the stream containing content returned by the server.
                    Stream dataStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content. 
                    string responseFromServer = reader.ReadToEnd();

                    //LogFileMgr.Instance.WriteToFile(@"d:\apps\logs\LDCalculatorResponse.txt", r);
                    //int i = responseFromServer.IndexOf(@"</islocal>");

                    //isLocal = responseFromServer.Substring(i - 1, 1);

                    // LogFileMgr.Instance.WriteToFile(@"d:\apps\logs\LDCalculatorResponse.txt", "LocalCall:: " +isLocal);
                    // Cleanup the streams and the response.
                    reader.Close();
                    dataStream.Close();
                    response.Close();
                    LogFileMgr.Instance.WriteToFile("DataTest.log", "ResponseFromServer:" + response.ToString());
                    LogFileMgr.Instance.WriteToFile("DataTest.log", "ResponseFromServer:" + responseFromServer);

                    break;
                case (HttpStatusCode.BadRequest):
                    // bad
                    LogFileMgr.Instance.WriteToLogFile("LDCalculator:CheckIt():BadResponseFromServer:" + response.StatusDescription);
                    break;

                default:
                    // go home
                    LogFileMgr.Instance.WriteToLogFile("LDCalculator:CheckIt():ResponseFromServer:" + response.StatusDescription);

                    break;
            }

            if (isLocal.Equals("Y"))
                return true;
            else
                //there is nothing else
                return false;
        }
    }
}

