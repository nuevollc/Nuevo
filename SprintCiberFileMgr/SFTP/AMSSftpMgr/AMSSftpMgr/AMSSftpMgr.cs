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

using KellermanSoftware.NetSFtpLibrary;
using TruMobility.Utils.Logging;

namespace TruMobility.Network.Services   
{

    /// <summary>
    /// class responsible for getting the MAF files off of the sprint server via SFTP
    /// </summary>
    public class AMSSftpMgr
    {
        private string m_sftpSiteUserName = System.Configuration.ConfigurationManager.AppSettings["SftpSiteUserName"];
        private string m_sftpSite = System.Configuration.ConfigurationManager.AppSettings["SftpSiteIPAddress"];
        private string m_sftpSiteRemoteFolder = System.Configuration.ConfigurationManager.AppSettings["SftpRemoteFileFolder"];
        private string m_moveToProcessFolder = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerMoveToProcessFolder"];
        private string m_moveAfterProcessingFolder = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerMoveAfterProcessingFolder"];
        private string m_moveToCrunchFolder = System.Configuration.ConfigurationManager.AppSettings["MoveToCrunchFolder"];
        private bool m_moveToCrunchFlag = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["MoveToCrunchFlag"]);
        private string m_identityFile = System.Configuration.ConfigurationManager.AppSettings["SftpIdentityFile"];
        private string m_sftpUserName = System.Configuration.ConfigurationManager.AppSettings["SftpUserName"];
        private string m_sftpKey = System.Configuration.ConfigurationManager.AppSettings["SftpLicenseKey"];
       
        // processing thread
        private Thread m_procThread;
        private int m_ProcessThreadIntervalInMSecs;
        private SFTP _sftp = null;
        private AMSDbMgr m_dbMgr = null;
        
        /// <summary>
        /// ctor
        /// </summary>
        public AMSSftpMgr()
        {
            m_dbMgr = new AMSDbMgr();
        }

        public bool GetFiles()
        {
            bool status = false;

            _sftp = new SFTP(m_sftpUserName, m_sftpKey);
            _sftp.EnableLogging();
            _sftp.HostAddress = m_sftpSite;
            _sftp.UserName = m_sftpSiteUserName;
            _sftp.SshKeyFile = m_identityFile;
            _sftp.SshPassphrase = @"t0y0ta69";
            try
            {
                try
                {
                    // connect
                    _sftp.Connect();

                }
                catch (KellermanSoftware.NetSFtpLibrary.Implementation.SftpException ke)
                {

                    LogMsg("NEXCEPTION:AMSSftpMgr::_sftp.Connect():ECaught:" + ke.Message);
                    return status;

                }
                // get a directory list
                List<FTPFileInfo> rFiles = _sftp.GetAllFiles(m_sftpSiteRemoteFolder);

                // process the list
                foreach (FTPFileInfo file in rFiles)
                {
                    if (file.Equals(".") || file.Equals(".."))
                        continue;

                    string f = this.ParseFileName(file.FileName);

                    // if we got it, don't get it 
                   if (this.CheckDb(f))
                      continue;

                    try
                    {
                        // get the file and put in the watch folder to be processed
                        _sftp.DownloadFile(m_moveToProcessFolder + f, file.FileName);
                        
                        // if we're crunching it then do it
                        if( this.m_moveToCrunchFlag )
                            _sftp.DownloadFile(m_moveToCrunchFolder + f, file.FileName);

                        string newPlace = m_moveAfterProcessingFolder + f;
                        bool moved =_sftp.MoveFile(file.FileName, newPlace );
                        bool dirDone =_sftp.CreateDirectory("tested");

                        // update the database to indicate file has been downloaded
                        this.UpdateDb(f);
                       
                    }
                    catch (SystemException se)
                    {
                        LogMsg("NEXCEPTION:AMSSftpMgr::GetFile():ECaught::" + se.Message + se.StackTrace);
                    }

                }

                _sftp.Disconnect();
                _sftp.Dispose();

                status = true;

            }
            catch (SystemException se)
            {
                LogMsg("NEXCEPTION:AMSSftpMgr::GetFile():ECaught:TryingToGetFile::" + se.Message);
            }
                    
            return status;

        } // getFile()

        private void LogMsg(string msg)
        {
            LogFileMgr.Instance.WriteToLogFile(msg);

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


        private bool CheckDb(string fileName)
        {
            return ( m_dbMgr.CheckDbFileDownloaded(fileName) );
        }

        private void UpdateDb(string fileName)
        {
            m_dbMgr.AddFileDownLoaded(fileName);

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

        private string ParseFileName(string fileName)
        {
            string newFileName = String.Empty;

            if (fileName.Contains('/'))
            {// move the file
                int index = fileName.LastIndexOf('/');
                newFileName = fileName.Substring(index + 1);
            }
            else
            {
                return newFileName = fileName;
            }

            return newFileName;

        }

    }
}
