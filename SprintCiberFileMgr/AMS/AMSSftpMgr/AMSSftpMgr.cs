using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Net;
using System.Data;
using System.IO;
using System.Threading;

using Tamir.SharpSsh;

using TruMobility.Utils.Logging;

namespace TruMobility.Sprint.AMS
{

    /// <summary>
    /// class responsible for getting the MAF files off of the sprint server via SFTP
    /// </summary>
    public class AMSSftpMgr
    {
        private string m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogName"];
        private string m_logFile = System.Configuration.ConfigurationManager.AppSettings["LogFileName"];
        private string m_sftpSiteUserName = System.Configuration.ConfigurationManager.AppSettings["SftpSiteUserName"];
        private string m_sftpSite = System.Configuration.ConfigurationManager.AppSettings["SftpSiteIPAddress"];
        private string m_sftpSiteRemoteFolder = System.Configuration.ConfigurationManager.AppSettings["SftpRemoteFileFolder"];
        private string m_moveToProcessFolder = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerMoveToProcessFolder"]; 
        private string m_identityFile = System.Configuration.ConfigurationManager.AppSettings["SftpIdentityFile"];
        
        // processing thread
        private Thread m_procThread;
        private int m_ProcessThreadIntervalInMSecs;

        private AMSDbMgr m_dbMgr;
        private LogFileMgr m_logger;
        
        /// <summary>
        /// ctor
        /// </summary>
        public AMSSftpMgr()
        {
            m_dbMgr = new AMSDbMgr();

            m_logger = LogFileMgr.Instance;
        }

        private bool GetFiles()
        {
            bool status = true;
            
            Sftp sftp = new Sftp( m_sftpSite, m_sftpSiteUserName );
            try
            {
                // add our private key to connect
                // put these in the config file
                sftp.AddIdentityFile( m_identityFile );

                // connect
                sftp.Connect();

                // get a directory list
                ArrayList rFiles = sftp.GetFileList( m_sftpSiteRemoteFolder );
                int indx = 0;
                foreach ( string file in rFiles )
                {
                    indx++;
                    if (file.Equals(".") || file.Equals(".."))
                        continue;

                    if ( this.CheckDb(file) )
                        continue;

                    try
                    {
                        // get the file and put in the watch folder to be processed
                        sftp.Get(m_sftpSiteRemoteFolder + file, m_moveToProcessFolder + file);

                        // update the database to indicate file has been downloaded
                        this.UpdateDb(file);

                        // delete the file on the remote server after pulling it over
                        // sftp.Delete(f);
                    }
                    catch (SystemException se)
                    {
                        LogMsg("NEXCEPTION:AMSSftpMgr::GetFile():ECaught:TryingToGetFile:" + indx + ":" + file + "::" + se.Message);
                    }

                }

                sftp.Close();
                status = true;

            }
            catch (Tamir.SharpSsh.jsch.SftpException se)
            {
                LogMsg("NEXCEPTION:AMSSftpMgr::GetFile():ECaught:" + se.Message);
            }
            catch (Tamir.SharpSsh.jsch.JSchException jse)
            {

                LogMsg("NEXCEPTION:AMSSftpMgr::GetFile():ECaught:" + jse.Message);
            }
            catch (Tamir.SharpSsh.SshTransferException ste)
            {
                LogMsg("NEXCEPTION:AMSSftpMgr::GetFile():ECaught:" + ste.Message);
            }
            catch (SystemException se)
            {
                LogMsg("NEXCEPTION:AMSSftpMgr::GetFile():ECaught:" + se.Message);
            }
                    
            return status;

        } // getFile()

        private void LogMsg(string msg)
        {
            m_logger.WriteToLogFile(msg);

        }// private void LogMsg(string msg)

        public void StopProcessing()
        {
            if (null != m_procThread)
            {
                // stop the watch thread
                m_procThread.Abort();
            }
        }

        public void StartProcessing()
        {
            // set up event logging
            this.CreateEventLog();

            // get the time to sleep between failed file open operations
            m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["SftpHandlerProcessThreadIntervalInSecs"]);

            try
            {

                // launch the job control processing thread monitoring the queue
                m_procThread = new Thread(new ThreadStart(ProcessJobControlFileThread));
                m_procThread.Name = "ProcessJobControlThread";
                m_procThread.Start();

            }
            catch (Exception ex)
            {
                LogMsg("-NEXCEPTION:AMSSftpMgr::StartProcessing():ErrorStartingThread" + " Error>" + ex.Message);
            }
        }// public void StartProcessing()

        private bool CreateEventLog()
        {
            bool Reasult = false; 
 
            try 
            {
                System.Diagnostics.EventLog.CreateEventSource(m_eventLogName, m_eventLogName ); 
                    System.Diagnostics.EventLog SQLEventLog = new System.Diagnostics.EventLog();

                    SQLEventLog.Source = m_eventLogName;
                    SQLEventLog.Log = m_eventLogName;

                    SQLEventLog.Source = m_eventLogName;
                    SQLEventLog.WriteEntry("The " + m_eventLogName + " was successfully initialize component.", EventLogEntryType.Information); 
 
 
                    Reasult = true; 
 
            } 
            catch 
            { 
                Reasult = false; 
            } 
 
 
            return Reasult; 
        }

        private bool CheckDb(string fileName)
        {
            return ( m_dbMgr.CheckIfFileDownloaded( fileName ) );
        }

        private void UpdateDb(string fileName)
        {
            m_dbMgr.AddAmsFile( fileName );

        }

        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        private void ProcessJobControlFileThread()
        {
            try
            {

                // main processing thread
                while (true)
                {
                    try
                    {

                        // see if there are any files to get
                        this.GetFiles();

                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        LogMsg("NEXCEPTION:AMSSftpMgr::ProcessJobControlFile():Service is stopping");
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        LogMsg("NEXCEPTION:AMSSftpMgr::ProcessJobControlFile():ECaught:" + ex.Message);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);

                }// while(true)
            }
            catch (System.Threading.ThreadAbortException)
            {
                LogMsg("NEXCEPTION:AMSSftpMgr::ProcessJobControlFile():ECaught: thread is shutting down");
            }
            catch (System.Exception ex)
            {
                LogMsg("NEXCEPTION:AMSSftpMgr::ProcessJobControlFile():ECaught:" + ex.Message);
            }// catch
            finally
            {
                // clean up

            }// finally

        }// private void ProcessJobControlFile()    

        private static void sshCp_OnTransferStart(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            Console.WriteLine("OnTransferStart");
        }

        private static void sshCp_OnTransferProgress(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            Console.WriteLine("sshCp_OnTransferProgress");
        }

        private static void sshCp_OnTransferEnd(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            Console.WriteLine("sshCp_OnTransferEnd");
        }

    }
}
