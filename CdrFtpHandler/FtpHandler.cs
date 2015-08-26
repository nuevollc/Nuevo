using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using FtpLib;
using Cdr.Utils;

namespace CdrFtpHandler
{
    public class FtpHandler
    {
        private string m_eventLogName;
        private string m_LogFile;
        private int m_runIntervalInSeconds;
        private string m_FtpFileToLocation;
        private string m_FTPFolder;
        private string m_FTPUserName;
        private string m_FTPPwd;
        private string m_FTPServerDirectory;

        private string m_delimStr = " ";
        private char[] m_delimiter;
       
        // r processing thread to download the files
        private Thread m_procThread;
        private FtpDbManager _dbMgr;


        /*
         * Class to handle FTP from the FTP server
         * This is meant to handle the FTP from Broadworks to Logi
         * Simple FTP from BW to Logi
         * 
         */
        public FtpHandler()
        {

            m_delimiter = m_delimStr.ToCharArray();

            // inits
            InitializeFromConfig();
            _dbMgr = new FtpDbManager();

        }

        public void StartProcessing()
        {

            // launch the file control processing thread
            m_procThread = new Thread(new ThreadStart(ProcessFileControlThread));
            FileWriter.Instance.WriteToLogFile("FtpHandler::StartProcessing():StartingThread");
            m_procThread.Start();
            FileWriter.Instance.WriteToLogFile("FtpHandler::StartProcessing():ThreadStarted");

        }

        /// <summary>
        /// main method used to process the files every configurable interval
        /// </summary>
        public void ProcessFileControlThread()
        {
            string theFileToGrab = String.Empty;
               
            // main processing thread

            while (true)
            {
                try
                {
                    using (FtpLib.FtpConnection ftp = new FtpLib.FtpConnection(m_FTPFolder, m_FTPUserName, m_FTPPwd))
                    {
                        try
                        {
                            ftp.Open(); /* Open the FTP connection */
                            ftp.Login(); /* Login using previously provided credentials */
                            ftp.SetCurrentDirectory(m_FTPServerDirectory); /* change current directory */
                            FtpLib.FtpFileInfo[] flist = ftp.GetFiles();

                            foreach (FtpLib.FtpFileInfo fi in flist)
                            {
                                if (!_dbMgr.CheckDbBeforeDownloadingFile(fi.Name))
                                {
                                    try
                                    {
                                        string fil = m_FTPServerDirectory + fi.Name;
                                        if (ftp.FileExists(fil))  /* check that a file exists */
                                        {
                                            ftp.GetFile(fil, m_FtpFileToLocation + fi.Name, false);
                                            FileWriter.Instance.WriteToLogFile("FtpHandler::ProcessFileControlThread:DownloadedFile" + fi.Name);
                                            // update the db so we do not download again
                                            _dbMgr.UpdateFtpFileNameInDb(fi.Name);
                                        }
                                    }
                                    catch (SystemException se)
                                    {
                                        FileWriter.Instance.WriteToLogFile("FtpHandler::ProcessControlThread:EXCEPTIONCAUGHT(Open/Login/Directory)::" + se.Message);
                                        FileWriter.Instance.WriteToLogFile(se.StackTrace);
                                    }
                                }
                            }
                        }
                        catch (SystemException se)
                        {
                            FileWriter.Instance.WriteToLogFile("FtpHandler::ProcessFileControlThread:EXCEPTIONCAUGHT::" + se.Message);

                        }
                    }
                }

                catch (SystemException se)
                {

                    FileWriter.Instance.WriteToLogFile("FtpHandler::ProcessFileControlThread:EXCEPTIONCAUGHT::TryingToConnect" + se.Message);

                }

                // wait for the next interval to ftp the next file over 
                System.Threading.Thread.Sleep(m_runIntervalInSeconds);

            }// while(true)

         }//ProcessFileControlThread

        public void StopProcessing()
        {
            if (null != m_procThread)
            {
                // stop the ProcessFileControlThread
                m_procThread.Abort();
            }

            FileWriter.Instance.WriteToLogFile("FtpHandler::StopProcessing::");
 
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
                m_FtpFileToLocation = ConfigurationManager.AppSettings["LocationToFTPFileTo"];
                m_runIntervalInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["FtpRunIntervalInSeconds"]) * 1000;
                m_FTPFolder = ConfigurationManager.AppSettings["FTPFolder"];// remote ftp server
                m_FTPUserName = ConfigurationManager.AppSettings["FTPUserName"];
                m_FTPPwd = ConfigurationManager.AppSettings["FTPPwd"];
                m_LogFile = ConfigurationManager.AppSettings["LogFileName"];
                m_FTPServerDirectory = ConfigurationManager.AppSettings["FTPServerDirectory"];
            }
            catch (System.Exception ex)
            {
                FileWriter.Instance.WriteToLogFile("FtpHandler::InitializeFromConfig::" + ex.Message + ex.StackTrace);
                return;
            }

            // init complete
            FileWriter.Instance.WriteToLogFile("FtpHandler::InitializeFromConfig::COMPLETE:");
            FileWriter.Instance.WriteToLogFile("m_runIntervalInSeconds:" + m_runIntervalInSeconds);
            FileWriter.Instance.WriteToLogFile("m_FTPFolder:" + m_FTPFolder);
            FileWriter.Instance.WriteToLogFile("m_FTPUserName:" + m_FTPUserName);
            FileWriter.Instance.WriteToLogFile("m_FTPPwd:" + m_FTPPwd);
            FileWriter.Instance.WriteToLogFile("m_LogFile:" + m_LogFile);
            FileWriter.Instance.WriteToLogFile("m_FTPServerDirectory:" + m_FTPServerDirectory);

        }

    }//class

}// ns
