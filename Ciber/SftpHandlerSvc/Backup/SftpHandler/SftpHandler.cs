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

namespace Strata8.Wireless.Utils
{
    public class SftpHandler
    {
        private string m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerEventLogName"];
        private string m_logFile = System.Configuration.ConfigurationManager.AppSettings["SftpHandlerLogFileName"];
        private string m_sftpSiteUserName = System.Configuration.ConfigurationManager.AppSettings["SftpSiteUserName"];
        private string m_sftpSitePassword = System.Configuration.ConfigurationManager.AppSettings["SftpSitePassword"];
        private string m_sftpSite = System.Configuration.ConfigurationManager.AppSettings["SftpSiteIPAddress"];
        private string m_sftpSiteRemoteFolder = System.Configuration.ConfigurationManager.AppSettings["SftpRemoteFileFolder"];

        private int m_ProcessThreadIntervalInMSecs;
        private string m_filemovePath;

        // queue which will contain all filenames messaged from the file watcher
        private Queue m_fileNameQ;

        // processing thread
        private Thread m_procThread;

        public bool SftpFile( string fileName )
        {
            SshTransferProtocolBase sshCp = new Sftp( m_sftpSite, m_sftpSiteUserName );

            sshCp.Password = m_sftpSitePassword;
            sshCp.OnTransferStart += new FileTransferEvent(sshCp_OnTransferStart);
            sshCp.OnTransferProgress += new FileTransferEvent(sshCp_OnTransferProgress);
            sshCp.OnTransferEnd += new FileTransferEvent(sshCp_OnTransferEnd);
            // string lfile = @"d:\apps\logs\ReadWriteMgrLog.log";
            string rfile = ParseFileName(fileName);
            string remoteFileName = m_sftpSiteRemoteFolder + rfile;
            sshCp.Connect();

            try
            {
                sshCp.Put(fileName, remoteFileName);
            }
            catch (Tamir.SharpSsh.jsch.SftpException e)
            {
                LogMsg("SftpHandler::SftpFile():ECaught:UnableToCreateEventLogFile::" + e.Message + e.InnerException);
                return false;
            }
            catch (Tamir.SharpSsh.SshTransferException e)
            {
                LogMsg("SftpHandler::SftpFile():ECaught:UnableToCreateEventLogFile::" + e.Message + e.InnerException);
                return false;
            }
            catch (SystemException se)
            {
                LogMsg("SftpHandler::SftpFile():ECaught:UnableToCreateEventLogFile::" + se.Message);
                return false;
            }
            
            return true;

        }

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
                    EventLog.WriteEntry(m_eventLogName, "SftpHandler::LogFileMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_logFile, EventLogEntryType.Error, 3020);
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
                    EventLog.WriteEntry(m_eventLogName, "SftpHandler::LogFileMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_logFile, EventLogEntryType.Error, 3020);
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
                LogMsg("SftpHandler::LogFileMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + "\r\n" + ex.StackTrace);
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
                LogMsg("SftpHandler::LogFileMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + "\r\n" + ex.StackTrace);
                return;
            }

        }// private void LogFileMsg(string msg)

        /// <summary>
        /// ctor
        /// </summary>
        public SftpHandler()
        {
            // the the path where to move processed files
            m_filemovePath = ConfigurationManager.AppSettings["SftpHandlerMoveFolder"];
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
                LogMsg( "SftpHandler::ProcessFileWatcher():ECaught trying to enqueue a filename::" + ex.Message );
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
                    LogMsg("SftpHandler::StartProcessing():ECaught:UnableToCreateEventLogFile::" + se.Message);
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

                LogMsg("SftpHandler::StartProcessing():WatchFolder:" + watchFolder + " FileType:" + fileType);

            }
            catch (Exception ex)
            {
                LogMsg("SftpHandler::StartProcessing():Error setting up watch folder>" + watchFolder + " Error>" + ex.Message );
            }
        }// public void StartProcessing()

        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        private void ProcessJobControlFileThread()
        {
            string fileName = String.Empty;
            string fName = String.Empty;
            try
            {
                while (true)
                {
                    fName = String.Empty;
                    try
                    {
                        if (m_fileNameQ.Count > 0)
                        {
                            // lock the root
                            lock (m_fileNameQ.SyncRoot)
                            {
                                // dequeue the next filename to process
                                fName = (string)m_fileNameQ.Dequeue();
                            }//lock(m_fileNameQ.SyncRoot)
                        }

                        //create the command string
                        if (fName != String.Empty)
                        {
                            // method to parse the CDRs
                            string parsedFileName = ParseFileName(fName);
                            this.SftpFile( fName );

                            // housekeeping, update our database that we have processed this file
                            UpdateFileInfo(parsedFileName);

                            // move the file with the full path name 
                            this.MoveTheFile(fName);
                        }

                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        LogMsg("SftpHandler::ProcessJobControlFile():Service is stopping" );
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        LogMsg("SftpHandler::ProcessJobControlFile():ECaught:" + ex.Message);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);
                }// while(true)
            }
            catch (System.Threading.ThreadAbortException)
            {
                LogMsg("SftpHandler::ProcessJobControlFile():ECaught: thread is shutting down");
            }
            catch (System.Exception ex)
            {
                LogMsg("SftpHandler::ProcessJobControlFile():ECaught:" + ex.Message);
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
                File.Move(fileName, m_filemovePath + newFileName);
                LogMsg("-NFORMATIONAL:moved file: " + fileName + " to location: " + m_filemovePath + newFileName + "\r\n");
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                LogMsg("Could not move file: " + fileName + " to location: " + m_filemovePath + newFileName + "error>" + ex.Message + "\r\n");
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
