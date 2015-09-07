using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Threading;
using TruMobility.Utils.Logging; 
using System.Net;
using System.Text;
using FtpLib;

namespace Nuevo.CallCruncher.CDR.FTP
{

    /// <summary>
    /// FtpMgr : FTP Manager to move files from our FTP server to
    /// a defined FTP site in the configuration file
    /// </summary>
    public class FtpMgr
    {
        private const int PROCESS_SLEEP_PERIOD = 1000;

        // get the time to sleep between failed file open operations
        private int m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["ProcessThreadIntervalInSecs"]);

        // directory where to put the service provider cdrs
        private string _connection = ConfigurationManager.AppSettings["BWorksCdr_SQLConnectString"];
        private string _cdrArchive = ConfigurationManager.AppSettings["CDRArchiveFolder"];
        private string _watchFolder = ConfigurationManager.AppSettings["WatchFolder"];
        private string _fileType = ConfigurationManager.AppSettings["WatchFileType"];

        private FtpSiteInfo _ftpSite = null;

        // queue which will contain all filenames messaged from the file watcher
        private Queue m_fileNameQ;
        // processing thread
        private Thread m_procThread;

        private FileSystemWatcher _watchFile = null;

        public FtpMgr()

        {

            _ftpSite = GetSiteInfo();

        }

        private void LogIt(string msg)
        {
            LogFileMgr.Instance.WriteToLogFile(msg);
        }

        public void StartProcessing()
        {

            try
            {
                // create the Watcher for file type in the watchfolder; config params
                _watchFile = new FileSystemWatcher(_watchFolder, _fileType);

                // Watch for changes in LastAccess and LastWrite times
                _watchFile.NotifyFilter = NotifyFilters.LastWrite  | NotifyFilters.CreationTime |
                    NotifyFilters.LastAccess | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                // Add event handlers.
                _watchFile.Renamed += new RenamedEventHandler(this.OnRenamed);
                _watchFile.Created += new FileSystemEventHandler(this.OnFileCreated);
                _watchFile.Changed += new FileSystemEventHandler(this.OnFileChanged);

                _watchFile.EnableRaisingEvents = true;

                // configure the file name FIFO queue
                Queue aQ = new Queue(100);

                // synchronize the queue
                m_fileNameQ = Queue.Synchronized(aQ);

                // launch the job control processing thread monitoring the queue
                m_procThread = new Thread(new ThreadStart(ProcessFileThread));
                m_procThread.Start();
            }
            catch (Exception ex)
            {
                LogIt("FtpMgr::StartProcessing()::ExceptionCaught:" + ex.Message);
            }
        }// public void StartProcessing()

        private FtpSiteInfo GetSiteInfo()
        {
            FtpSiteInfo si = new FtpSiteInfo();

            try
            {
                si.ServiceProvider = "TruMobility";
                // get the site credentials 
                si.Site = ConfigurationManager.AppSettings["FTPSite"];
                si.Username = ConfigurationManager.AppSettings["FTPSiteUsername"];
                si.Password = ConfigurationManager.AppSettings["FTPSitePassword"];
            }
            catch (SystemException se)
            {
                LogIt("FtpMgr::GetSiteInfo()::ExceptionCaught:" + se.Message);
            }

            return si;
        }

        public void StopProcessing()
        {
            if (null != m_procThread)
            {
                // stop the watch thread
                m_procThread.Abort();
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
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
                LogIt("FtpMgr::OnRenamed()::ExceptionCaught:" + ex.Message);
            }
        }

        /// <summary>
        /// ProcessFileWatcher: Method that monitors the directory and queues up
        /// the file to be processed onto the queue for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileChanged(object sender, FileSystemEventArgs e)
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
                LogIt("FtpMgr::ProcessFileChanged()::ExceptionCaught:" + ex.Message);
            }

        }//private void ProcessFileWatcher(object sender, FileSystemEventArgs e)

        /// <summary>
        /// ProcessFileWatcher: Method that monitors the directory and queues up
        /// the file to be processed onto the queue for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileCreated(object sender, FileSystemEventArgs e)
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
                LogIt("FtpMgr::ProcessFileCreated()::ExceptionCaught:" + ex.Message);
            }

        }//private void ProcessFileWatcher(object sender, FileSystemEventArgs e)
        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        private void ProcessFileThread()
        { 
            try
            {
                while (true)
                {
                    string fName = String.Empty;
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
                            PostFileToSite(fName);

                            // move to the processed archive directory
                            MoveTheFile(fName);
                        }

                    }//try
                    catch (System.Threading.ThreadAbortException ta)
                    {
                        LogIt("FtpMgr::ProcessFileThread()::ExceptionCaught:File:" + fName + "::"+ ta.Message);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic system exception
                        LogIt("FtpMgr::ProcessFileThread()::ExceptionCaught:File:" + fName + "::" + ex.Message);
                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);

                }// while(true)

            }
            catch (System.Threading.ThreadAbortException ta)
            {
                LogIt("FtpMgr::ProcessFileThread()::ExceptionCaught:" + ta.Message);
            }
            catch (System.Exception ex)
            {
                LogIt("FtpMgr::ProcessFileThread()::ExceptionCaught:" + ex.Message);
            }// catch

        }// private void ProcessJobControlFile()

        private void MoveTheFile(string fileName)
        {
            // move the file
            int index = fileName.LastIndexOf('\\');
            string newFileName = fileName.Substring(index + 1);

            // add a date time stamp to the filename; removed for now
            DateTime aTime = DateTime.Now;
            // newFileName += "."+ aTime.Month.ToString() + aTime.Day.ToString() + aTime.Year.ToString() + "-" + aTime.Hour.ToString() + aTime.Minute.ToString() + aTime.Second.ToString() + aTime.Millisecond.ToString();
            StringBuilder sb = new StringBuilder(_cdrArchive + "/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/");
            // check the directory
            if (!Directory.Exists(sb.ToString()))
            {
                Directory.CreateDirectory(sb.ToString());
            }

            sb.Append(newFileName);
            try
            {
                File.Move(fileName, sb.ToString());
                LogIt("FtpMgr::MoveTheFile()::File:" + fileName + "  MovedTo::" + sb.ToString());
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                LogIt("FtpMgr::MoveTheFile()::IOExceptionCaught:" + ex.Message);
            }

        }// MoveTheFile()

        /// <summary>
        /// method to parse and load the service providers being processed
        /// configuration parameters delimited by commas
        /// </summary>
        /// <param name="userList"></param>
        private List<string> ParseList(string userList)
        {
            List<string> theList = new List<string>();
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = userList.Trim().Split(delimiter);

            foreach (string s in split)
            {
                theList.Add(s.Trim());

            }

            return theList;

        }//ParseList

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

        /// <summary>
        /// method to upload the contents of a file from a remote URI
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public void PostFileToSite(string fileName)
        { 
            try
            {
                using (FtpLib.FtpConnection ftp = new FtpLib.FtpConnection(_ftpSite.Site, _ftpSite.Username, _ftpSite.Password))
                {
                    try
                    {
                        ftp.Open(); /* Open the FTP connection */
                        ftp.Login(); /* Login using previously provided credentials */
         
                        //ftp.SetCurrentDirectory(m_FTPServerDirectory); /* change current directory */
                        ftp.PutFile(fileName);
                    }
                    catch (FtpLib.FtpException e)
                    {
                        LogIt("FtpMgr::PostFileToSite():FtpExceptionCaught: " + e.Message);
                        LogIt(e.StackTrace);
                    }
                }
            }
            catch (FtpLib.FtpException e)
            {
                LogIt("FtpMgr::PostFileToSite():FtpExceptionCaught: " + e.Message);
                LogIt(e.StackTrace);
            }
             
        }
  
        public void Test()
        {
            string f = @"\testFile.txt";
            PostFileToSite(_watchFolder + f);
            MoveTheFile(_watchFolder + f);

        }


    }
}
