using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using FTPLib;
using System.Threading;
using System.Data;
using System.Data.SqlClient;

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

        // create an instance of the ftp library
        private FTPLib.FTP m_ftplib = null;

        // our log file
        //private FileStream m_fs = null;

        public FtpHandler()
        {
            m_delimiter = m_delimStr.ToCharArray();
            //
            // TODO: Add constructor logic here
            //
            try
            {
                m_ftplib = new FTP();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void StartProcessing()
        {
            // inits
            InitializeFromConfig();

            // launch the file control processing thread
            m_procThread = new Thread(new ThreadStart(ProcessFileControlThread));
            m_procThread.Start();

        }

        /// <summary>
        /// main method used to process the files every configurable interval
        /// </summary>
        public void ProcessFileControlThread()
        {
            string theFileToGrab = String.Empty;

            try
            {
                // main processing thread
                while (true)
                {
                    try
                    {
                        // try connecting to the site
                        m_ftplib.Connect(m_FTPFolder, m_FTPUserName, m_FTPPwd);
                    }
                    catch (System.Exception ex)
                    {
                        EventLog.WriteEntry(m_eventLogName, "ProcessFileControlThread()::UnableToLoginToFTPSite:Exception Caught:" + ex.ToString(), EventLogEntryType.Error, 2001);
                        return;
                    }

                    int perc = 0;
                    try
                    {
                        // open the file with resume support if it already exists, the last 
                        // peram should be false for no resume
                        if (m_ftplib.IsConnected)
                        {
                            m_ftplib.ChangeDir(m_FTPServerDirectory);
                            ArrayList fList = m_ftplib.ListFiles();

                            foreach (string str in fList)
                            {
                                LogFileMsg(DateTime.Now.ToShortDateString() + ":" + DateTime.Now.ToLongTimeString() + "-INFORMATIONAL:FileInDirectory::" + str);

                            }
                            

                            //grab the first file listed and parse the string
                            string fileToGrab = (string)fList[0];
                            string[] teststr = fileToGrab.Split(m_delimiter, StringSplitOptions.RemoveEmptyEntries);

                            // the last element is the file we want
                            int len = teststr.Length;
                            theFileToGrab = teststr[len - 1];

                            // add code here to verify that the file has not already been downloaded
                            // all downloaded files will get processed into the database
                            // if (theFileToGrab) is not in db
                            // download the file
                            if (!CheckDbBeforeDownloadingFile(theFileToGrab))
                            {

                                // here we download directly to the directory
                                // where the CDRHandler will process it
                                m_ftplib.OpenDownload(theFileToGrab, m_FtpFileToLocation + theFileToGrab, true);
                                while (m_ftplib.DoDownload() > 0)
                                {
                                    // perc = (int)((m_ftplib.BytesTotal * 100) / m_ftplib.FileSize);
                                    // Console.Write("\rDownloading: {0}/{1} {2}%",
                                    // m_ftplib.BytesTotal, m_ftplib.FileSize, perc);
                                    // Console.Out.Flush();
                                }


                                // update the db so we do not download again
                                UpdateFtpFileNameInDb(theFileToGrab);
                            }
                            else
                            {
                                EventLog.WriteEntry(m_eventLogName, "ProcessFileControlThread()::FileAlreadyFTP'dToS8Platform:"+theFileToGrab, EventLogEntryType.Warning, 3000);
                            }

                            // file is downloaded so we can disconnect
                            m_ftplib.Disconnect();
                        }


                    }
                    catch (System.Exception ex)
                    {
                        EventLog.WriteEntry(m_eventLogName, "Exception Caught:"+ex.ToString(), EventLogEntryType.Error, 2001);
                    }

                    // wait for the next interval to ftp the next file over 
                    System.Threading.Thread.Sleep(m_runIntervalInSeconds);

                }// while(true)

            }
            catch (System.Threading.ThreadAbortException)
            {
                EventLog.WriteEntry(m_eventLogName, "Service is stopping -- ProcessFileControlThread is shutting down", EventLogEntryType.Information, 3000);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "Exception Caught:" + ex.ToString(), EventLogEntryType.Error, 2001);
            }// catch

        }//ProcessFileControlThread

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
                    EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
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
                    EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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

            EventLog.WriteEntry(m_eventLogName, "Service is stopping -- the FTPHandler thread is stopping", EventLogEntryType.Information, 3000);

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
                m_FTPFolder = ConfigurationManager.AppSettings["FTPFolder"];
                m_FTPUserName = ConfigurationManager.AppSettings["FTPUserName"];
                m_FTPPwd = ConfigurationManager.AppSettings["FTPPwd"];
                m_LogFile = ConfigurationManager.AppSettings["LogFile"];
                m_FTPServerDirectory = ConfigurationManager.AppSettings["FTPServerDirectory"];
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "Exception Caught:" + ex.ToString(), EventLogEntryType.Error, 2001);
                return;
            }

            EventLog.WriteEntry(m_eventLogName, "CdrFtpSvc()::ServiceConfigurationIsComplete", EventLogEntryType.Information, 3000);
        }

        /// <summary>
        /// method to open the database connection
        /// </summary>
        /// <param name="dataConnection"></param>
        private void OpenDataConn(ref SqlConnection dataConnection)
        {
            try
            {
                dataConnection = new SqlConnection();
                dataConnection.ConnectionString = ConfigurationManager.AppSettings["SQLConnectString"];
                dataConnection.Open();

            }// try 
            catch (Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "FtpHandlerSvc::OpenDataConnection():FailedEstablishingDBConnection:" + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void OpenDataConn( ref SqlConnection dataConnection ) 

        /// <summary>
        /// method used to close the database connection
        /// </summary>
        /// <param name="dataConnection"></param>
        private void CloseDataConn(ref SqlConnection dataConnection)
        {
            try
            {
                if (dataConnection != null)
                {
                    dataConnection.Close();
                    dataConnection = null;
                }

            }// try 
            catch (Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "FtpHandlerSvc::OpenDataConnection():FailedTryingToCloseTheDBConnection:" + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void CloseDataConn( ref SqlConnection dataConnection )

        private bool CheckDbBeforeDownloadingFile(string fileName)
        {
            bool fileDownloaded = true;
            StringBuilder cmdStr = new StringBuilder("SELECT fileName from BworksCdrFilesDownloaded where fileName =");
            DataSet mySet = new DataSet();
            SqlDataAdapter rDapter = null;
            SqlConnection dataConnection = null;

            try
            {
                // make the connection
                OpenDataConn(ref dataConnection);

                cmdStr.Append("'" + fileName + "'");
                // execute and fill
                rDapter = new SqlDataAdapter(cmdStr.ToString(), dataConnection);
                rDapter.Fill(mySet);

                // have one table
                DataTable theTable = mySet.Tables[0];
                DataRow[] currentRows = theTable.Select(
                    null, null, DataViewRowState.CurrentRows);

                if (currentRows.Length < 1)
                {
                    Console.WriteLine("No Current Rows Found");
                    fileDownloaded = false;
                }

            }
            catch (System.Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "FtpHandlerSvc::UpdateFtpFileNameInDb():FailedTryingToUpdateTheFileNameInTheDB:" + e.ToString(), EventLogEntryType.Error, 3012);
            }

            CloseDataConn(ref dataConnection);

            return fileDownloaded;

        }//CheckDbBeforeDownloadingFile

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        private void UpdateFtpFileNameInDb( string fileName )
        {
            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("INSERT INTO BworksCdrFilesDownloaded ");
                cmdStr.Append("(fileName, downloaded) VALUES(");
                cmdStr.Append("'" + fileName + "', 1 )");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "FtpHandlerSvc::UpdateFtpFileNameInDb():FailedTryingToUpdateTheFileNameInTheDB:" + e.ToString(), EventLogEntryType.Error, 3012); 
            }

            CloseDataConn(ref dataConnection);

        }// UpdateFtpFileNameInDb()

    }
}
