using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Net;
using System.IO;


namespace Strata8.Test
{
    public class TestClass
    {

        /// <summary>
        /// method to upload the contents of a file from a remote URI
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public void PostFileToSite()
        {


            try
            {
                string u = @"ftp:\\10.0.16.60";
                string fileName = @"d:\apps\logs\ReadWriteMgrLog.log";

                string parsedFileName = ParseFileName(fileName);
                // contains the URI path and filename to upload to the remote server
                UriBuilder ub = new UriBuilder(u + "//" + parsedFileName + ".csv");
                Uri ftpUri = ub.Uri;
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.EnableSsl = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;

                NetworkCredential nc = new NetworkCredential("cdrreader", "r34dm3");
                CredentialCache cc = new CredentialCache();
                cc.Add(ftpUri, "Basic", nc);
                request.Credentials = cc;

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader(fileName);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();

                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //WriteToLogFile("ServiceProviderCdrHandler::PostFileToSite():FilePosted:" + fileName + "-" + response.StatusDescription);

                response.Close();
            }
            catch (WebException e)
            {
                //WriteToLogFile("ServiceProviderCdrHandler::PostFileToSite():ECaught:" + e.Message + "\n" + e.StackTrace);
                Console.WriteLine(e.Message + e.StackTrace);
            }

        }



        private string ParseFileName(string fileName)
        {
            string newFileName = String.Empty;

            if (fileName.Contains(@"\"))
            {// move the file
                int index = fileName.LastIndexOf(@"\");
                newFileName = fileName.Substring(index + 1);

            }
            else
            {
                newFileName = fileName;
            }

            if (newFileName.Contains(@"."))
            {
                int i = newFileName.IndexOf(@".");
                newFileName = newFileName.Substring(0, i);
            }
            return newFileName;
        }

    }
}
