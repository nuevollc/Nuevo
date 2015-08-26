
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

namespace TruMobility.Network.Services
{

    /// <summary>
    /// class responsible for getting the MAF files off of the sprint server via SFTP
    /// </summary>
    public class SftpMgr1
    {
        private string m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerEventLogName"];
        private string m_logFile = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerLogFileName"];
        private string m_sftpSiteUserName = System.Configuration.ConfigurationManager.AppSettings["SftpSiteUserName"];
        private string m_sftpSite = System.Configuration.ConfigurationManager.AppSettings["SftpSiteIPAddress"];
        private string m_sftpSiteRemoteFolder = System.Configuration.ConfigurationManager.AppSettings["SftpRemoteFileFolder"];
        private string m_moveToProcessFolder = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerMoveToProcessFolder"]; 
        private string m_watchFolder = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerWatchFolder"];
        private string m_processedFileFolder = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerMoveProcessedFileFolder"];
        private int m_ProcessThreadIntervalInMSecs;

        // queue which will contain all filenames messaged from the file watcher
        private Queue m_fileNameQ;

        // processing thread
        private Thread m_procThread;

        public bool SftpFile( string fileName )
        {
            // add code when needed to support MAF SFTP upload
            // currently not needed in this application  
            
            return false;

        }


        public bool GetFiles()
        {
            bool status = true;
            
            Sftp sftp = new Sftp( m_sftpSite, m_sftpSiteUserName );
            try
            {
                // add our private key to connect
                // put these in the config file
                sftp.AddIdentityFile(@"d:\apps\RSAKeys\opensshkey");

                // connect
                sftp.Connect();

                // get a directory list
                ArrayList rFiles = sftp.GetFileList( m_sftpSiteRemoteFolder );
                foreach ( string file in rFiles )
                {
                    if (file.Equals(".") || file.Equals(".."))
                        continue; 

                    // get the file and put in the watch folder to be processed
                    sftp.Get(m_sftpSiteRemoteFolder + file, m_moveToProcessFolder + file);
                    
                    // update our database that we have downloaded the file from the server
                    // this.updateDb( file );

                    // delete the file on the remote server after pulling it over
                    // sftp.Delete(f);
                }

                sftp.Close();
                status = true;

            }
            catch (Tamir.SharpSsh.jsch.SftpException se)
            {
                LogMsg("NEXCEPTION:SftpMgr::GetFile():ECaught:" + se.Message);
            }
            catch (Tamir.SharpSsh.jsch.JSchException jse)
            {

                LogMsg("NEXCEPTION:SftpMgr::GetFile():ECaught:" + jse.Message);
            }
            catch (Tamir.SharpSsh.SshTransferException ste)
            {
                LogMsg("NEXCEPTION:SftpMgr::GetFile():ECaught:" + ste.Message);
            }
            catch (SystemException se)
            {
                LogMsg("NEXCEPTION:SftpMgr::GetFile():ECaught:" + se.Message);
            }
                    
            return status;

        } // getFile()


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

        private void LogMsg(string msg)
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if (!File.Exists(m_logFile))
            {
                try
                {
                    fs = File.Create(m_logFile);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "-NFORMATIONAL:SftpHandler::LogFileMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_logFile, EventLogEntryType.Error, 3020);
                    return;
                }

            }// created new file and file stream
            else
            {
                // we just append to the file
                try
                {
                    fs = new FileStream(m_logFile, FileMode.Append, FileAccess.Write);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "-NFORMATIONAL:SftpHandler::LogFileMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_logFile, EventLogEntryType.Error, 3020);
                    return;
                }
            }


            // Create a new streamwriter to write to the file   
            try
            {
                sw = new StreamWriter(fs);
                sw.Write(DateTime.Now.ToShortDateString() + ":" + DateTime.Now.ToLongTimeString() + " : " + msg + "\r\n");

            }
            catch (System.Exception ex)
            {
                LogMsg("-NEXCEPTION:SftpHandler::LogFileMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + "\r\n" + ex.StackTrace);
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
                LogMsg("-NEXCEPTION:SftpHandler::LogFileMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + "\r\n" + ex.StackTrace);
                return;
            }

        }// private void LogFileMsg(string msg)

        /// <summary>
        /// ctor
        /// </summary>
        public SftpMgr1()
        {

        }

        /// <summary>
        /// ProcessFileWatcher: Method that monitors the directory and queues up
        /// the file to be processed onto the queue for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessFileWatcher(object sender, FileSystemEventArgs e)
        {
            try
            {
                // lock the root
                lock (m_fileNameQ.SyncRoot)
                {
                    // enqueue the new filename if we have not queued it up yet
                    if ( !m_fileNameQ.Contains( e.FullPath ) ) 
                        m_fileNameQ.Enqueue(e.FullPath);

                }//lock(m_fileNameQ.SyncRoot)
            }
            catch (System.Exception ex)
            {
                LogMsg("-NEXCEPTION:SftpHandler::ProcessFileWatcher():ECaught trying to enqueue a filename::" + ex.Message);
            }

        }//private void ProcessFileWatcher(object sender, FileSystemEventArgs e)

        private void UpdateFileInfo(string fileName)
        { 
            // update our file info to reflect that we have ftp'd file 
        
        }// UpdateFileInfo

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        private void UpdateProcessedFileNameInDb( string fileName )
        {

        }// UpdateDownloadedFileNameInDb()

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
            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerEventLogName"];
            // set up event logging
            if (!EventLog.SourceExists(m_eventLogName))
            {
                try
                {
                    EventLog.CreateEventSource(m_eventLogName, "Application");
                }
                catch (SystemException se)
                {
                    LogMsg("-NEXCEPTION:SftpHandler::StartProcessing():ECaught:UnableToCreateEventLogFile::" + se.Message);
                }

            }
            string watchFolder = System.String.Empty;

            // get the time to sleep between failed file open operations
            m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["SftpHandlerProcessThreadIntervalInSecs"]);

            try
            {
                // create the Watcher for file type in the watchfolder; config params
                watchFolder = ConfigurationManager.AppSettings["SftpHandlerWatchFolder"];
                string fileType = ConfigurationManager.AppSettings["SftpHandlerWatchFileType"];
                FileSystemWatcher WatchFile = new FileSystemWatcher(watchFolder, fileType);

                // Watch for changes in LastAccess and LastWrite times
                WatchFile.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                WatchFile.Created += new FileSystemEventHandler(this.ProcessFileWatcher);
                WatchFile.Changed += new FileSystemEventHandler(this.ProcessFileWatcher);

                WatchFile.EnableRaisingEvents = true;

                // configure the file name FIFO queue
                Queue aQ = new Queue(100);

                // synchronize the queue
                m_fileNameQ = Queue.Synchronized(aQ);

                // launch the job control processing thread monitoring the queue
                m_procThread = new Thread(new ThreadStart(ProcessJobControlFileThread));
                m_procThread.Start();

                LogMsg("-NFORMATIONAL:SftpHandler::StartProcessing():WatchFolder:" + watchFolder + " FileType:" + fileType);

            }
            catch (Exception ex)
            {
                LogMsg("-NEXCEPTION:SftpHandler::StartProcessing():Error setting up watch folder>" + watchFolder + " Error>" + ex.Message);
            }
        }// public void StartProcessing()

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
                        LogMsg("NEXCEPTION:SftpMgr::ProcessJobControlFile():Service is stopping");
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        LogMsg("NEXCEPTION:SftpMgr::ProcessJobControlFile():ECaught:" + ex.Message);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);
                }// while(true)
            }
            catch (System.Threading.ThreadAbortException)
            {
                LogMsg("NEXCEPTION:SftpMgr::ProcessJobControlFile():ECaught: thread is shutting down");
            }
            catch (System.Exception ex)
            {
                LogMsg("NEXCEPTION:SftpMgr::ProcessJobControlFile():ECaught:" + ex.Message);
            }// catch
            finally
            {
                // clean up

            }// finally

        }// private void ProcessJobControlFile()

        private void MoveTheFile(string fileName)
        {
            // move the file
            int index = fileName.LastIndexOf('\\');
            string newFileName = fileName.Substring(index + 1);

            // add a date time stamp to the filename; removed for now
            DateTime aTime = DateTime.Now;
            // newFileName += "."+ aTime.Month.ToString() + aTime.Day.ToString() + aTime.Year.ToString() + "-" + aTime.Hour.ToString() + aTime.Minute.ToString() + aTime.Second.ToString() + aTime.Millisecond.ToString();
            try
            {
                File.Move(fileName, m_processedFileFolder + newFileName);
                LogMsg("-NFORMATIONAL:moved file: " + fileName + " to location: " + m_processedFileFolder + newFileName + "\r\n");
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                LogMsg("-NEXCEPTION:Could not move file: " + fileName + " to location: " + m_processedFileFolder + newFileName + "error>" + ex.Message + "\r\n");
            }

        }// MoveTheFile()

        private string ParseFileName(string fileName)
        {
            string newFileName = String.Empty;

            if (fileName.Contains('\\'))
            {// move the file
                int index = fileName.LastIndexOf('\\');
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
