using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System;
using System.Web;
using System.Net;

using TruMobility.Utils.Logging;

namespace Strata8.Telephony.MiddleTier.Services.CDR
{

    /// <summary>
    /// CdrHandler : Responsible for the Cdr processing.
    /// </summary>
    public class ServiceProviderCdrHandler
    {
        private string m_ServiceProviderErrorLogFileName;

        private List<FtpSiteInfo> m_ftpInfo = new List<FtpSiteInfo>();

        char[] m_sep; 
        char[] m_trim;
        char[] m_backslash;
        char[] m_dot;

        public ServiceProviderCdrHandler()
        {
            // get the event log name
            m_ServiceProviderErrorLogFileName = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderErrorLogFileName"];

            m_sep = new char[] { ',' };
            m_trim = new char[] { '"' };
            m_backslash = new char[] { '\\' };
            m_dot = new char[] { '.' };
        }

        private void LogIt(string msg)
        {
            LogFileMgr.Instance.WriteToLogFile(msg);
        }

        public void ProcessServiceProviderCdrs( string fileName, string line, FtpSiteInfo si )
        {

            try
            {
                // parse the line
                string[] controls = line.Split(m_sep);
                if (controls.GetLength(0) < 100)
                {
                    // we have a non-data line -- header or footer... so just write it
                    LogFileMgr.Instance.WriteToFile(si.Filename, line);
                }
                else if ( controls[1].Trim(m_trim).Equals( si.ServiceProvider ) )
                {
                    LogFileMgr.Instance.WriteToFile(si.Filename, line);
                }

            }
            catch (System.Exception ex)
            {
                LogIt("ProcessServiceProviderCdrs():: " + ex.Message);
            }

            return;

        }// private void ProcessServiceProviderCdrs()

        /// <summary>
        /// method to upload the contents of a file from a remote URI
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public void PostFileToSite( FtpSiteInfo si )
        {
            string parsedFileName = string.Empty;
            Uri ftpUri = null;

            try
            {
                parsedFileName = ParseFileName(si.Filename );

                // contains the URI path and filename to upload to the remote server
                // always using the cdrs directory below the users root directory
                UriBuilder ub = new UriBuilder("ftp", si.Site, -1, "/cdrs/" + parsedFileName + ".csv"); 
 
                ftpUri = ub.Uri;

                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                NetworkCredential nc = new NetworkCredential( si.Username, si.Password );
                CredentialCache cc = new CredentialCache();
                cc.Add(ftpUri, "Basic", nc);
                request.Credentials = cc;
                //request.Timeout = 300000;
                //request.ReadWriteTimeout = 10000000;
                //request.KeepAlive = false;

                request.UseBinary = true;
                request.UsePassive = false;

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader( si.Filename );
                byte[] fileContents = Encoding.UTF8.GetBytes( sourceStream.ReadToEnd() );
                sourceStream.Close();

                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException e)
            {
                LogIt("ServiceProviderCdrHandler::PostFileToSite():ECaught: " + e.Message );
            }
            catch( UriFormatException ue )
            {
                LogIt("ServiceProviderCdrHandler::PostFileToSite():ECaught: " + ue.Message);
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


        public void Upload( FtpSiteInfo si) 
        {
            Stream requestStream = null;
            FileStream fileStream = null;
            FtpWebResponse uploadResponse = null;
            string parsedFileName = null;
            Uri ftpUri = null;


            try
            {
                parsedFileName = ParseFileName(si.Filename);

                // contains the URI path and filename to upload to the remote server
                //UriBuilder ub = new UriBuilder(si.Site + "//" + parsedFileName + ".csv");
                UriBuilder ub = new UriBuilder("ftp", si.Site, -1, "/cdrs/"+parsedFileName + ".csv");
                // UriBuilder ub = new UriBuilder("ftp", "208.99.195.208", -1, parsedFileName + ".csv");

                ftpUri = ub.Uri;
                FtpWebRequest uploadRequest =
                    (FtpWebRequest)WebRequest.Create( ftpUri );
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;

                // UploadFile is not supported through an Http proxy
                // so we disable the proxy for this request.
                uploadRequest.Proxy = null;

                NetworkCredential nc = new NetworkCredential(si.Username, si.Password);
                CredentialCache cc = new CredentialCache();
                cc.Add(ftpUri, "Basic", nc);
                uploadRequest.Credentials = cc;
                uploadRequest.UseBinary = true;
                uploadRequest.UsePassive = false;

                requestStream = uploadRequest.GetRequestStream();
                fileStream = File.Open(si.Filename, FileMode.Open);

                byte[] buffer = new byte[1024];
                int bytesRead;
                while (true)
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    requestStream.Write(buffer, 0, bytesRead);
                }

                // The request stream must be closed before getting 
                // the response.
                requestStream.Close();

                uploadResponse =
                    (FtpWebResponse)uploadRequest.GetResponse();
            }
            catch (UriFormatException ex)
            {
                LogIt("Upload::PostFileToSite():ECaught: " + ex.Message);
            }
            catch (IOException ex)
            {
                LogIt("Upload::PostFileToSite():ECaught: " + ex.Message);
            }
            catch (WebException ex)
            {
                LogIt("Upload::PostFileToSite():ECaught: " + ex.Message);

            }
            finally
            {
                if (uploadResponse != null)
                    uploadResponse.Close();
                if (fileStream != null)
                    fileStream.Close();
                if (requestStream != null)
                    requestStream.Close();
            }
        }

    }
}
