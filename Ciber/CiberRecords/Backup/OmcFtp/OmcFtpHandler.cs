
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Xml;


namespace Strata8.Wireless.Cdr
{

    /// <summary>
    /// Class that handles collecting the wireless CDRs off of the OMC
    /// This class runs every configurable interval to do a directory listing
    /// and determine which files need to be downloaded.  The files that are 
    /// downloaded are updated in the database so that they are not downloaded
    /// multiple times.  The file are written to the configurable parameter that
    /// defines the FTPLocalDirectory.
    /// </summary>
    public class OmcFtpHandler
    {
        private string m_eventLogName;
        private string m_LogFile;
        private int m_runIntervalInSeconds;
        private string m_FTPUserName;
        private string m_FTPPwd;
        private Uri m_uri;

        // remote ftp server directory
        private string m_FTPServerDirectory;
        // local directory to store the CDR file
        private string m_FTPLocalDirectory;

        private string m_delimStr = " ";
        private char[] m_delimiter;

        // r processing thread to download the files
        private Thread m_procThread;

        // private our database interface
        private OmcCdrDb m_db;

        // our log file
        //private FileStream m_fs = null;

        public OmcFtpHandler()
        {
            m_delimiter = m_delimStr.ToCharArray();
        }

        public void StartProcessing()
        {
            // inits
            InitializeFromConfig();
            m_db = new OmcCdrDb();

            // launch the file control processing thread
            m_procThread = new Thread(new ThreadStart(ProcessFileControlThread));
            m_procThread.Start();

        }

        /// <summary>
        /// main method used to process the files every configurable interval
        /// </summary>
        protected void ProcessFileControlThread()
        {
            try
            {
                // main processing thread
                while (true)
                {
                    try
                    {
                        // get the directory listing
                        List<string> dirList = FtpUtils.ListDirectoryOnFtpSite(m_uri, m_FTPUserName, m_FTPPwd);
                        // only get the latest file for now loop thru all files


                        // add code here to verify that the file has not already been downloaded
                        // all downloaded files will get processed into the database
                        // if (theFileToGrab) is not in db
                        // download the file
                        foreach (string fileName in dirList)
                        {

                            try
                            {

                                if (!m_db.CheckDbForFileDownloaded(fileName))
                                {
                                    // here we download directly to the directory
                                    // where the CDRHandler will process it
                                    // m_ftplib.OpenDownload(theFileToGrab, m_FtpFileToLocation + theFileToGrab, true);
                                    UriBuilder ub = new UriBuilder(m_FTPServerDirectory + @"/" + fileName);
                                    Uri u = ub.Uri;
                                    string omcCdrs = FtpUtils.GetFileFromSite(u, m_FTPUserName, m_FTPPwd);

                                    // write the CDR file to our server
                                    WriteCdrFile(fileName, omcCdrs);

                                    // update the db so we do not download again
                                    UpdateDateFileDownloaded(fileName);

                                    // log the event so we know things are working
                                    this.LogFileMsg("OmcFtpHandler::ProcessFileControlThread()::FileFTP'dToS8Platform:" + fileName);
                                }
                                else
                                {
                                    // log the event so we know things are working
                                    // this.LogFileMsg("OmcFtpHandler::ProcessFileControlThread()::File*ALREADY*FTP'dToS8Platform:" + fileName);
                                }

                            }
                            catch (SystemException ex)
                            {
                                EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::ProcessFileControlThread():ExceptionCaughtProcessingFile:" + fileName + ex.ToString(), EventLogEntryType.Error, 2001);
                            }

                        }

                        // file is downloaded so we can disconnect
                        // m_ftplib.Disconnect();
                    }
                    catch (System.Exception ex)
                    {
                        EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::ProcessFileControlThread():Exception Caught:" + ex.ToString(), EventLogEntryType.Error, 2001);
                        return;

                    }

                    // wait for the next interval to ftp the next file over 
                    System.Threading.Thread.Sleep(m_runIntervalInSeconds);

                }// while(true)

            }
            catch (System.Threading.ThreadAbortException)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::Service is stopping -- ProcessFileControlThread is shutting down", EventLogEntryType.Information, 3000);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::Exception Caught:" + ex.ToString(), EventLogEntryType.Error, 2001);
            }// catch

        }//ProcessFileControlThread

        /// <summary>
        /// private method to write the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContents"></param>
        private void WriteCdrFile(string fileName, string fileContents)
        {

            try
            {
                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(fileContents);
                XmlWriterSettings xs = new XmlWriterSettings();
                xs.Indent = true;
                xs.OmitXmlDeclaration = true;
                xs.NewLineOnAttributes = true;
                XmlWriter w = XmlWriter.Create(this.m_FTPLocalDirectory + fileName, xs);
                w.WriteRaw(fileContents);
                w.Flush();
                w.Close();

                // save the document
                //XmlTextWriter wr = new XmlTextWriter(this.m_FTPLocalDirectory + fileName, null);
                //wr.Formatting = Formatting.Indented;
                //doc.Save(wr);
            }
            catch (SystemException ex)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::WriteCdrFile()::Exception CaughtProcessingFile:" + fileName + ex.ToString(), EventLogEntryType.Error, 2001);

            }
        }

        /// <summary>
        /// private method to add the file info to the database so that we don't download
        /// or process the file again
        /// </summary>
        /// <param name="fileName"></param>
        private void UpdateDateFileDownloaded(string fileName)
        {
            m_db.AddDateDownloaded(fileName);
        }

        private void LogFileMsg(string msg)
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if (!File.Exists(m_LogFile))
            {
                try
                {
                    fs = File.Create(m_LogFile);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::LogFileMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }

            }// created new file and file stream
            else
            {
                // we just append to the file
                try
                {
                    fs = new FileStream(m_LogFile, FileMode.Append, FileAccess.Write);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::LogFileMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }
            }


            // Create a new streamwriter to write to the file   
            try
            {
                sw = new StreamWriter(fs);
                sw.Write(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + msg + "\r\n");

            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::LogFileMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::LogFileMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

        }// private void LogFileMsg(string msg)

        public void StopProcessing()
        {
            if (null != m_procThread)
            {
                // stop the ProcessFileControlThread
                m_procThread.Abort();
            }

            EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::Service is stopping -- the FTPHandler thread is stopping", EventLogEntryType.Information, 3000);

        }//StopProcessing

        private void InitializeFromConfig()
        {
            try
            {
                // method that initialized params from config file

                // get the event log name
                m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogName"];
                // set up event logging
                if (!EventLog.SourceExists(m_eventLogName))
                {
                    EventLog.CreateEventSource(m_eventLogName, "Application");
                }

                // the the path where to move processed files
                m_runIntervalInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["FtpRunIntervalInSeconds"]) * 1000;
                m_FTPUserName = ConfigurationManager.AppSettings["FTPUserName"];
                m_FTPPwd = ConfigurationManager.AppSettings["FTPPwd"];
                m_LogFile = ConfigurationManager.AppSettings["LogFile"];
                m_FTPServerDirectory = ConfigurationManager.AppSettings["FTPServerDirectory"];

                // where we write the CDR file to
                m_FTPLocalDirectory = ConfigurationManager.AppSettings["FTPLocalDirectory"];
                UriBuilder ub = new UriBuilder(m_FTPServerDirectory);
                m_uri = ub.Uri;
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::InitializeFromConfig():Exception Caught:" + ex.ToString(), EventLogEntryType.Error, 2001);
                return;
            }

            EventLog.WriteEntry(m_eventLogName, "OmcFtpHandler::InitializeFromConfig():ServiceConfigurationIsComplete", EventLogEntryType.Information, 3000);

        }// InitializeFromConfig

    }// OmcFtpHandler Class

}// namespace
