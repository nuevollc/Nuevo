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

namespace EPCS.Syniverse.TN
{

       /// <summary>
    /// Class that handles creating and putting a TN file up to the Syniverse
    /// servers on a daily basis.  Two files are loaded, one that includes the
    /// additions and another that includes the deletes.
    /// This class runs every configurable interval to do a directory listing
    /// and determine which files need to be uploaded.  The files that are 
    /// uploaded are updated in the database so that they are not uploaded
    /// multiple times.  The file are written to the configurable parameter that
    /// defines the FTPServerDirectory, they are read from the TNLocalDirectory param.
    /// The file name is: customer.[add|del].yyyymmdD.csv
    /// A file is provided every day, even if it is an empty file.
    /// The CSV File Format ascii, comma delimited. At a minimum, the 10D phone number and customerid fields are req'd.
    /// There must be an end of line on each line, no carriage return
    ///  
    /// MasterCustomerName, PhoneNumber, Product, InstalledDate,CustomerID, CustomerName, ConsumptionStatus,CustomerServiceName
    /// EPCS,2062101077,EPCSProduct, 10/21/2010 6:55, [EPCS1|EPCS2],EPCS,Assigned, EPCS
    ///
    /// </summary> 
    public class FtpHandler
    {

        private string m_eventLogName;
        private string m_LogFile;
        private int m_runIntervalInSeconds;
        private string m_FTPUserName;
        private string m_FTPPwd;
        private Uri m_uri;

        // remote ftp server directory, where the TN file is going to
        private string m_FTPServerDirectory;
        // local directory to store the CDR file
        private string m_TNLocalDirectory;

        private string m_delimStr = " ";
        private char[] m_delimiter;

        // r processing thread to download the files
        private Thread m_procThread;

        // private our database interface
        private FtpDbMgr m_db;

        // our logger
        private FtpFileWriter m_logger = FtpFileWriter.Instance;

        // our log file
        //private FileStream m_fs = null;

         
        public FtpHandler()
        {
            m_delimiter = m_delimStr.ToCharArray();
        }

         
        public void StartProcessing()
        {
            // inits
            InitializeFromConfig();
            m_db = new FtpDbMgr();

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
                        // check to see if we have a file, if so we process it, if not we create a
                        // dummy file to upload (an add and a delete file)
                        // get the local directory listing 
                        List<string> dirList = FtpUtils.ListDirectoryOnFtpSite(m_uri, m_FTPUserName, m_FTPPwd);
                        // only get the latest file for now loop thru all files

                        // Only get files that our csv format "*.csv"
                        string[] dirs = Directory.GetFiles(this.m_TNLocalDirectory, "*.csv");

                        // add code here to verify that the file has not already been downloaded
                        // all downloaded files will get processed into the database
                        // if (theFileToGrab) is not in db
                        // download the file
                        foreach (string fileName in dirs)
                        {
                            try
                            {
                                if (!m_db.CheckDbForSyniverseFileUploaded(fileName))
                                {
                                    // here we download directly to the directory
                                    // where the CDRHandler will process it
                                    // m_ftplib.OpenDownload(theFileToGrab, m_FtpFileToLocation + theFileToGrab, true);
                                    UriBuilder ub = new UriBuilder(m_FTPServerDirectory + @"/" + fileName);
                                    Uri u = ub.Uri;
                                    byte[] b = FtpUtils.PutFileToSite(u, fileName, m_FTPUserName, m_FTPPwd);

                                    // update the db so we do not download again
                                    this.m_db.UpdateDateFileUploaded( fileName );

                                    // log the event so we know things are working
                                    this.LogFileMsg("FtpHandler::ProcessFileControlThread()::FileFTP'dToSyniverse:" + fileName);
                                }
                                else
                                {
                                    // log the event so we know things are working
                                    // this.LogFileMsg("FtpHandler::ProcessFileControlThread()::File*ALREADY*FTP'dToS8Platform:" + fileName);
                                }

                            }
                            catch (SystemException ex)
                            {
                                m_logger.WriteToEventLog("FtpHandler::ProcessFileControlThread():ExceptionCaughtProcessingFile:" + fileName + ex.Message);
                            }

                        }

                        // file is downloaded so we can disconnect
                        // m_ftplib.Disconnect();
                    }
                    catch (System.Exception ex)
                    {
                        m_logger.WriteToEventLog( "FtpHandler::ProcessFileControlThread():Exception Caught:" + ex.Message );
                        return;

                    }

                    // wait for the next interval to ftp the next file over 
                    System.Threading.Thread.Sleep(m_runIntervalInSeconds);

                }// while(true)

            }
            catch (System.Threading.ThreadAbortException)
            {
                m_logger.WriteToEventLog("FtpHandler::Service is stopping -- ProcessFileControlThread is shutting down");
            }
            catch (System.Exception ex)
            {
                m_logger.WriteToEventLog("FtpHandler::Exception Caught:" + ex.Message);
            }// catch

        }//ProcessFileControlThread

        private void LogFileMsg(string msg)
        {
            m_logger.WriteToLogFile(msg);
        }

        public void StopProcessing()
        {
            if (null != m_procThread)
            {
                // stop the ProcessFileControlThread
                m_procThread.Abort();
            }

            m_logger.WriteInformationalToEventLog("FtpHandler::Service is stopping -- the FTPHandler thread is stopping" );

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

                // where we get the TN file from if there is one available
                m_TNLocalDirectory = ConfigurationManager.AppSettings["TNLocalDirectory"];
                UriBuilder ub = new UriBuilder(m_FTPServerDirectory);
                m_uri = ub.Uri;
            }
            catch (System.Exception ex)
            {
                m_logger.WriteToLogFile("FtpHandler::InitializeFromConfig():Exception Caught:" + ex.Message);
                return;
            }

            m_logger.WriteInformationalToEventLog("FtpHandler::InitializeFromConfig():ServiceConfigurationIsComplete");

        }// InitializeFromConfig

    }
}

 