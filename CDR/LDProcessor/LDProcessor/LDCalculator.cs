using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using TruMobility.Utils.Logging;

namespace TruMobility.CDR.LD
{
    public class LDCalculator
    {
        private static string _request = "http://www.localcallingguide.com/xmlrcdist.php?";

        public static bool CheckIt(string npaNxx1, string npaNxx2 )
        {
            string isLocal = String.Empty;

            string npa1 = npaNxx1.Substring(0, 3);
            string nxx1 = npaNxx1.Substring(3, 3);
            string npa2 = npaNxx2.Substring(0, 3);
            string nxx2 = npaNxx2.Substring(3, 3);

            string rString = "npa1=" + npa1 + "&nxx1=" + nxx1 + "&npa2=" + npa2 + "&nxx2=" + nxx2;

            // Create a request for the URL. 		
            //WebRequest request = WebRequest.Create("http://www.localcallingguide.com/xmlrcdist.php?npa1=416&nxx1=423&npa2=212&nxx2=733");
            WebRequest request = WebRequest.Create(_request + rString);

            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
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
                    int i = responseFromServer.IndexOf(@"</islocal>");

                    isLocal = responseFromServer.Substring(i - 1, 1);

                    // LogFileMgr.Instance.WriteToFile(@"d:\apps\logs\LDCalculatorResponse.txt", "LocalCall:: " +isLocal);
                    // Cleanup the streams and the response.
                    reader.Close();
                    dataStream.Close();
                    response.Close();
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

            if ( isLocal.Equals("Y") )
                return true;
            else
                //there is nothing else
                return false;
        }
    }
}

