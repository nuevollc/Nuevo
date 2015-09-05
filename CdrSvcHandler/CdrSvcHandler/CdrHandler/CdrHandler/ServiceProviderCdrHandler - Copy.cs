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

namespace Strata8.Telephony.MiddleTier.Services.CDR
{

    /// <summary>
    /// CdrHandler : Responsible for the Cdr processing.
    /// </summary>
    public class ServiceProviderCdrHandler
    {
        private string m_ServiceProviderDirectory;
        private string m_ServiceProviderErrorLogFileName;
        private string m_ServiceProviderFTPSite;
        private string m_ServiceProviderFTPUsername;
        private string m_ServiceProviderFTPPassword;

        char[] m_sep; 
        char[] m_trim;
        char[] m_backslash;
        char[] m_dot;

        public ServiceProviderCdrHandler()
        {
            // get the event log name
            m_ServiceProviderDirectory = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderDirectory"];
            m_ServiceProviderErrorLogFileName = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderErrorLogFileName"];
            m_ServiceProviderFTPSite = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderFTPSite"];
            m_ServiceProviderFTPUsername = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderFTPUsername"];
            m_ServiceProviderFTPPassword = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderFTPPassword"];

            m_sep = new char[] { ',' };
            m_trim = new char[] { '"' };
            m_backslash = new char[] { '\\' };
            m_dot = new char[] { '.' };
        }

        public string ProcessServiceProviderCdrs( string fileName, string line, string serviceProviderName )
        {
            string newFileName = this.ParseFileName(fileName);

            string spFile = m_ServiceProviderDirectory + "Premiere" + newFileName +".csv";

            try
            {
                // parse the line
                string[] controls = line.Split(m_sep);
                if (controls.GetLength(0) < 100)
                {
                    // we have a non-data line -- header or footer... so just write it
                    this.WriteToFile(spFile, line);
                }
                else if ( controls[1].Trim(m_trim).Equals( serviceProviderName ) )
                {
                    this.WriteToFile(spFile, line);
                }

            }
            catch (System.Exception ex)
            {
                WriteToLogFile( ex.Message + "\r\n" + ex.StackTrace );
            }

            return spFile;

        }// private void ProcessServiceProviderCdrs()


        private void WriteToFile(string fileName , string msg)
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if (!File.Exists(fileName))
            {
                try
                {
                    fs = File.Create(fileName);
                }
                catch (System.Exception ex)
                {
                   // EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }

            }// created new file and file stream
            else
            {
                // we just append to the file
                try
                {
                    fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                }
                catch (System.Exception ex)
                {
                    //EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }
            }


            // Create a new streamwriter to write to the file   
            try
            {
                sw = new StreamWriter(fs);
                sw.Write(msg + "\r\n");

            }
            catch (System.Exception ex)
            {
                //EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

            try
            {
                // Close StreamWriter and the file
                if (sw != null)
                    sw.Close();
                fs.Close();
            }
            catch (System.Exception ex)
            {
               // EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

        }// private void LogFileMsg(string msg)

        public void WriteToLogFile(string msg)
        {
            try
            {
                FileStream file = new FileStream(m_ServiceProviderErrorLogFileName, FileMode.Append, FileAccess.Write);

                // Create a new stream to write to the file
                StreamWriter sw = new StreamWriter(file);

                // Write a string to the file
                sw.Write(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + msg + "\r\n");

                // Close StreamWriter
                sw.Close();

                // Close file
                file.Close();
            }
            catch (System.Exception ex)
            {
                WriteToLogFile(ex.Message + "\r\n" + ex.StackTrace);
            }
        }// public void WriteToLogFile

        /// <summary>
        /// method to upload the contents of a file from a remote URI
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public void PostFileToSite( string fileName )
        {
            try
            {
                string parsedFileName = ParseFileName(fileName);
                // contains the URI path and filename to upload to the remote server
                UriBuilder ub = new UriBuilder(m_ServiceProviderFTPSite + "//" + parsedFileName + ".csv");
                Uri ftpUri = ub.Uri;
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                NetworkCredential nc = new NetworkCredential( m_ServiceProviderFTPUsername, m_ServiceProviderFTPPassword );
                CredentialCache cc = new CredentialCache();
                cc.Add(ftpUri, "Basic", nc);
                request.Credentials = cc;

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader(fileName);
                byte[] fileContents = Encoding.UTF8.GetBytes( sourceStream.ReadToEnd() );
                sourceStream.Close();

                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                WriteToLogFile("ServiceProviderCdrHandler::PostFileToSite():FilePosted:" + fileName + "-" + response.StatusDescription);

                response.Close();
            }
            catch (WebException e)
            {
                WriteToLogFile("ServiceProviderCdrHandler::PostFileToSite():ECaught:" + e.Message + "\n" + e.StackTrace);
            }

        }

        private string ParseFileName(string fileName)
        {
            string newFileName = String.Empty;

            if ( fileName.Contains(@"\" ) )
            {// move the file
                int index = fileName.LastIndexOf(@"\");
                newFileName = fileName.Substring(index + 1);

            }
            else
            {
                newFileName = fileName;
            }

            if (newFileName.Contains( @"." ))
            {
                int i = newFileName.IndexOf( @"." );
                newFileName = newFileName.Substring(0, i);
            }

            return newFileName;

        }

    }
}
