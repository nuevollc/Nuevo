using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using TruMobility.Utils.Logging;

namespace TruMobility.Sprint.AMS
{
    /// <summary>
    /// handler call responsible for the AMS file processing
    /// </summary>
    public class AMSHandler
    {
        private const int PROCESS_SLEEP_PERIOD = 1000;
        private string m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogName"];
        private string m_filemovePath;
        private int m_ProcessThreadIntervalInMSecs;    

        // queue which will contain all filenames messaged from the file watcher
        private Queue m_fileNameQ;
        // processing thread
        private Thread m_procThread;

        private AMSDbMgr m_dbMgr = null;
        private AMSFileReader m_rdr = null;
        private LogFileMgr m_logger = LogFileMgr.Instance;


        public AMSHandler()
        {
            //
            // TODO: Add constructor logic here
            //

        }

        private bool CreateEventLog()
        {
            bool Reasult = false;

            try
            {
                System.Diagnostics.EventLog.CreateEventSource(m_eventLogName, m_eventLogName);
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

        public void StartProcessing()
        {

            this.CreateEventLog();
            m_dbMgr= new AMSDbMgr();
            m_rdr = new AMSFileReader();

            // the the path where to move processed files
            m_filemovePath = ConfigurationManager.AppSettings["ProcessedFileFolder"];
            string watchFolder = System.String.Empty;

            // get the time to sleep between failed file open operations
            m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["ProcessThreadIntervalInSecs"]);

            try
            {
                // create the Watcher for file type in the watchfolder; config params
                watchFolder = ConfigurationManager.AppSettings["WatchFolder"];
                string fileType = ConfigurationManager.AppSettings["WatchFileType"];
                FileSystemWatcher WatchFile = new FileSystemWatcher(watchFolder, fileType);

                // Watch for changes in LastAccess and LastWrite times
                WatchFile.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite;               
                WatchFile.Created += new FileSystemEventHandler(this.ProcessFileWatcher);
                
                // added this on 11/02
                WatchFile.Changed += new FileSystemEventHandler(this.ProcessFileWatcher);

                WatchFile.EnableRaisingEvents = true;
                //EventLog.WriteEntry(m_eventLogName, "Watching Folder>" + watchFolder, EventLogEntryType.Information, 2002);
                
                // configure the file name FIFO queue
                Queue aQ = new Queue(100);
                
                // synchronize the queue
                m_fileNameQ = Queue.Synchronized(aQ);
                
                // launch the job control processing thread monitoring the queue
                m_procThread = new Thread(new ThreadStart(ProcessJobControlFileThread));
               
                m_logger.WriteToLogFile("-NFORMATIONAL::AMSHandler::StartProcessing():StartingThreadToProcessMAFFIles");              
                m_procThread.Start();
                m_logger.WriteToLogFile("-NFORMATIONAL::AMSHandler::StartProcessing():StartedThreadToProcessMAFFIles");

            }
            catch (Exception ex)
            {
                m_logger.WriteToLogFile("-NEXCEPTIONCAUGHT::AMSHandler::StartProcessing():ExceptionCaught:" + ex.Message + ex.StackTrace );
            }
        }// public void StartProcessing()

 
        public void StopProcessing()
        {
            if (null != m_procThread)
            {
                // stop the watch thread
                m_procThread.Abort();
            }
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
                    // enqueue the new filename
                    m_fileNameQ.Enqueue(e.FullPath);

                }//lock(m_fileNameQ.SyncRoot)
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "Error while trying to enqueue a filename Error is:" + ex.ToString(), EventLogEntryType.Error, 3014);
            }

        }//private void ProcessFileWatcher(object sender, FileSystemEventArgs e)

        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        private void ProcessJobControlFileThread()
        {
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
                                // m_logger.WriteToLogFile("-NFORMATIONAL::AMSHandler::ProcessJobControlFileThread():DequeuingTheFileToProcess");
                                // dequeue the next filename to process
                                fName = (string)m_fileNameQ.Dequeue();
                            }//lock(m_fileNameQ.SyncRoot)
                        }

                        //create the command string
                        if (fName != String.Empty && fName.Contains("IPDR") )
                        {
                            // process the file:
                            // read the MAF records
                            // update the database with the MAF records
                            m_rdr.ProcessTheFile(fName);
                            
                            // move the file to the processed directory
                            this.MoveTheFile(fName);

                            // update the database to show we've processed this file
                            string f = this.ParseFileName(fName);
                            m_dbMgr.AddAmsFile(f);
                        }

                    }//try
                    catch (System.Threading.ThreadAbortException te)
                    {
                        m_logger.WriteToLogFile("-NEXCEPTIONCAUGHT::AMSHandler::ProcessJobControlFileThread():ExceptionCaught:ThreadAbortExceptionCaught" + te.Message + te.StackTrace);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic system exception

                        m_logger.WriteToLogFile("-NEXCEPTIONCAUGHT::AMSHandler::ProcessJobControlFileThread():ExceptionCaught:SystemExceptionCaught" + ex.Message + ex.StackTrace);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);
                
                }// while(true)
            
            }

            catch (System.Threading.ThreadAbortException te)
            {
                m_logger.WriteToLogFile("-NEXCEPTIONCAUGHT::AMSHandler::ProcessJobControlFileThread():ExceptionCaught:ThreadAbortExceptionCaught" + te.Message + te.StackTrace);
            }

            catch (System.Exception ex)
            {
                m_logger.WriteToLogFile("-NEXCEPTIONCAUGHT::AMSHandler::ProcessJobControlFileThread():ExceptionCaught:SystemExceptionCaught" + ex.Message + ex.StackTrace);
            }// catch


        }// private void ProcessJobControlFile()

        /// <summary>
        /// private method to FormatTheTime
        /// </summary>
        /// <param name="theTime"></param>
        /// <returns>string time representation</returns>
        private string FormatTheTime(string theTime)
        {
            string str = null;
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;

            if (theTime.Length > 13)
            {
                year = Convert.ToInt16(theTime.Substring(0, 4));
                month = Convert.ToInt16(theTime.Substring(4, 2));
                day = Convert.ToInt16(theTime.Substring(6, 2));
                hour = Convert.ToInt16(theTime.Substring(8, 2));
                min = Convert.ToInt16(theTime.Substring(10, 2));
                sec = Convert.ToInt16(theTime.Substring(12, 2));
                str = ",'" + new DateTime(year, month, day, hour, min, sec, 0).ToString() + "'";
            }
            else
            {
                // we either fill it, or leave it empty
               str = ",'" + new DateTime(1970, 1, 1, 1, 1, 1, 0).ToString() + "'";
                // leave it empty
                // str = ",'"+"'";
            }

            return str;

        }
     
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
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                m_logger.WriteToLogFile("Could not move file: " + fileName + " to location: " + m_filemovePath + newFileName + "error>" + ex.ToString() + "\r\n");
            }

        }// MoveTheFile()

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

            //if (newFileName.Contains(@"."))
            //{
            //    int i = newFileName.IndexOf(@".");
            //    newFileName = newFileName.Substring(0, i);
            //}

            return newFileName;

        }

    }
}
