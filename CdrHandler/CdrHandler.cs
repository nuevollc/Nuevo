
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System;

namespace Strata8.Telephony.MiddleTier.Services.CDR
{

    /// <summary>
    /// CdrHandler : Responsible for the Cdr processing.
    /// </summary>
    public class CdrHandler
    {
        private const int PROCESS_SLEEP_PERIOD = 1000;
        private string m_eventLogName;
        private string m_filemovePath;
        private int m_ProcessThreadIntervalInMSecs;    
        private string m_LogFile; 

        // queue which will contain all filenames messaged from the file watcher
        private Queue m_fileNameQ;
        // processing thread
        private Thread m_procThread;
        
        public CdrHandler()
        {
            //
            // TODO: Add constructor logic here
            //
            // ADD log files and locations

        }

        public void StartProcessing()
        {
            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogname"];
            // set up event logging
            if (!EventLog.SourceExists(m_eventLogName))
            {
                EventLog.CreateEventSource(m_eventLogName, "Application");
            }
            // the the path where to move processed files
            m_filemovePath = ConfigurationManager.AppSettings["MoveFolder"];
            string watchFolder = System.String.Empty;

            // get the time to sleep between failed file open operations
            m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["ProcessThreadIntervalInSecs"]);  

            // get the error log filename
            m_LogFile = ConfigurationManager.AppSettings["LogFile"];  ;  

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
                EventLog.WriteEntry(m_eventLogName, "Watching Folder>" + watchFolder, EventLogEntryType.Information, 2002);
                
                // configure the file name FIFO queue
                Queue aQ = new Queue(100);
                
                // synchronize the queue
                m_fileNameQ = Queue.Synchronized(aQ);
                
                // launch the job control processing thread monitoring the queue
                m_procThread = new Thread(new ThreadStart(ProcessJobControlFileThread));
                m_procThread.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "Error setting up watch folder>" + watchFolder + " Error>" + ex.ToString(), EventLogEntryType.Error, 2000);
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

            SqlDataAdapter dataDapter = null;
            DataSet dataSet = new DataSet();
            string guid = String.Empty;
            ArrayList theControls;
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
                            theControls = ParseTheCdr(fName);

                            // add code to update the Parsed, and Parsed date fields in 
                            // the BworksCdrFilesDownloaded table
                            bool dbUpdate = InsertCdrIntoDb(theControls);

                            // create the standard S8 Format

                            this.MoveTheFile(fName);
                        }

                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        EventLog.WriteEntry(m_eventLogName, "Service is stopping -- the provisioning thread is shutting down", EventLogEntryType.Information, 2001);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        EventLog.WriteEntry(m_eventLogName, "ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);
                }// while(true)
            }
            catch (System.Threading.ThreadAbortException)
            {
                EventLog.WriteEntry(m_eventLogName, "Service is stopping -- the provisioning thread is shutting down", EventLogEntryType.Information, 3000);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "Failed to process origin file>>" + fName + " destination Path and File>>" + m_filemovePath + fileName + " Exception Is:" + ex.ToString(), EventLogEntryType.Error, 2001);
            }// catch
            finally
            {
                // clean up
                if (dataDapter != null)
                {
                    dataDapter = null;
                }

            }// finally

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

        /// <summary>
        /// private method to format/write the cdrs to the database
        /// </summary>
        /// <param name="theCdr"></param>
        /// <returns></returns>
        private bool InsertCdrIntoDb(ArrayList theCdr )
        {
            string theTime;
            int controlNbr = 0;

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            foreach (BworksCdr bCdr in theCdr)
            {

                try
                {
                    StringBuilder cmdStr = new StringBuilder("INSERT INTO BworksCdr ");
                    cmdStr.Append("(recordId,serviceProvider,type,userNumber,groupNumber,");
                    cmdStr.Append("direction,callingNumber,callingPresentationIndicator,calledNumber,");
                    cmdStr.Append("startTime,userTimeZone, answerIndicator,");
                    cmdStr.Append("answerTime,releaseTime,terminationCause,networkType,carrierIndentificationCode,");
                    cmdStr.Append("dialedDigits,callCategory,networkCallType,networkTranslatedNumber,");
                    cmdStr.Append("networkTranslatedGroup, releasingParty, route, networkCallId, codec,");
                    cmdStr.Append("accessDeviceAddress, groupId, chargeIndicator, conferenceId, userId) VALUES(");

                    cmdStr.Append("'" + bCdr.recordId + "'");
                    cmdStr.Append(",'" + bCdr.serviceProvider + "'"); // service provider field = userId
                    cmdStr.Append(",'" + bCdr.type + "'");
                    cmdStr.Append(",'" + bCdr.userNumber + "'");
                    cmdStr.Append(",'" + bCdr.groupNumber + "'");

                    cmdStr.Append(",'" + bCdr.direction + "'");
                    cmdStr.Append(",'" + bCdr.callingNumber + "'");
                    cmdStr.Append(",'" + bCdr.callingPresentationIndicator + "'");
                    cmdStr.Append(",'" + bCdr.calledNumber + "'");

                    // convert the START time
                    theTime = bCdr.startTime;
                    string timeFormat = FormatTheTime(theTime);
                    cmdStr.Append(timeFormat);
                    cmdStr.Append(",'" + bCdr.userTimeZone + "'");
                    cmdStr.Append(",'" + bCdr.answerIndicator + "'");

                    // convert the ANSWER time
                    theTime = bCdr.answerTime;
                     timeFormat = FormatTheTime(theTime);
                    cmdStr.Append(timeFormat);

                    // convert the RELEASE time
                    theTime = bCdr.releaseTime;
                     timeFormat = FormatTheTime(theTime);
                    cmdStr.Append(timeFormat);
                    cmdStr.Append(",'" + bCdr.terminationCause + "'");
                    cmdStr.Append(",'" + bCdr.networkType + "'");
                    cmdStr.Append(",'" + bCdr.carrierIdentificationCode + "'");

                    cmdStr.Append(",'" + bCdr.dialedDigits + "'");
                    cmdStr.Append(",'" + bCdr.callCategory + "'");
                    cmdStr.Append(",'" + bCdr.networkCallType + "'");
                    cmdStr.Append(",'" + bCdr.networkTranslatedNumber + "'");

                    //("networkTranslatedGroup, releasingParty, route, networkCallId, codec,");
                    //("accessDeviceAddress) VALUES(");
                    cmdStr.Append(",'" + bCdr.networkTranslatedGroup + "'");
                    cmdStr.Append(",'" + bCdr.releasingParty + "'");
                    cmdStr.Append(",'" + bCdr.route + "'");
                    cmdStr.Append(",'" + bCdr.networkCallId + "'");
                    cmdStr.Append(",'" + bCdr.codec + "'");

                    // accessDeviceAddress, groupId, chargeIndicator)
                    cmdStr.Append(",'" + bCdr.accessDeviceAddress + "'"); 
                    cmdStr.Append(",'" + bCdr.group + "'");
                    cmdStr.Append(",'" + bCdr.chargeIndicator + "'");
                    cmdStr.Append(",'" + bCdr.conferenceId + "'");
                    cmdStr.Append(",'" + bCdr.userId + "')");

                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                    controlNbr++;

                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "ECaught:CDR#" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3000);
                }
            }//for each CDR

            CloseDataConn(ref dataConnection);
 
            return true;
        }

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
                EventLog.WriteEntry(m_eventLogName, "CDRHandler Service FAILED trying to get a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void OpenDataConn( ref SqlConnection dataConnection ) 
    
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
                EventLog.WriteEntry(m_eventLogName, "CDRHandler Service FAILED trying to close a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void CloseDataConn( ref SqlConnection dataConnection )
    
        /// <summary>
        /// private method to parse the cdr
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private ArrayList ParseTheCdr(string fileName)
        {
            // make the array size (number of cdrs per file ) configurable
            System.Collections.ArrayList theControls = new System.Collections.ArrayList(1000);

        
            // example file formats
            //146148451ce6120071008204847.1711-070000,,Start
            //146158451ce6120071008204858.8531-070000,Javelin_sp,Normal,+14256051047,,Originating,+14256051047,Public,2142498,20071008204858.853,1-070000,Yes,20071008204910.746,20071008205017.699,016,VoIP,,2142498,,,14252142498,,remote,172.27.58.50:5060,BW134858906081007-1715305251@172.27.57.10,G729/8000,,,,,,Pacific_Financial_Advisors_gp,,,,,,,,,,y,,,66330:0,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,4256051047@tmp.strata8.net,,,,,,,,,,,,,,,,,,,,,,,,,,,,20071008205001.004,Success,66330:1,Transfer Consult,,,,,16.157,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
            char[] sep = new char[] { ',' };
            char[] trim = new char[] { '"' };
            int lineNumber = 1;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            // new record
                            BworksCdr aRec = new BworksCdr();

                            // parse the line
                            string[] controls = line.Split(sep);
                            if (controls.GetLength(0) < 100)
                            {
                                // we have a non-data line -- header or footer... so skip it
                                continue;
                            }

                            aRec.recordId = controls[0].Trim(trim);
                            aRec.serviceProvider = controls[1].Trim(trim);
                            aRec.type = controls[2].Trim(trim);
                            aRec.userNumber = controls[3].Trim(trim);
                            aRec.groupNumber = controls[4].Trim(trim);
                            aRec.direction = controls[5].Trim(trim);
                            aRec.callingNumber = controls[6].Trim(trim);
                            aRec.callingPresentationIndicator = controls[7].Trim(trim);
                            aRec.calledNumber = controls[8].Trim(trim);
                            aRec.startTime = controls[9].Trim(trim);
                            aRec.userTimeZone = controls[10].Trim(trim);
                            aRec.answerIndicator = controls[11].Trim(trim);
                            aRec.answerTime = controls[12].Trim(trim);
                            aRec.releaseTime = controls[13].Trim(trim);
                            aRec.terminationCause = controls[14].Trim(trim);
                            aRec.networkType = controls[15].Trim(trim);
                            aRec.carrierIdentificationCode = controls[16].Trim(trim);
                            aRec.dialedDigits = controls[17].Trim(trim);
                            aRec.callCategory = controls[18].Trim(trim);
                            aRec.networkCallType = controls[19].Trim(trim);
                            aRec.networkTranslatedNumber = controls[20].Trim(trim);
                            aRec.networkTranslatedGroup = controls[21].Trim(trim);
                            aRec.releasingParty = controls[22].Trim(trim);
                            aRec.route = controls[23].Trim(trim);
                            aRec.networkCallId = controls[24].Trim(trim);
                            aRec.codec = controls[25].Trim(trim);
                            aRec.group = controls[31].Trim(trim);
                            aRec.chargeIndicator = controls[41].Trim(trim);
                            aRec.conferenceId = controls[51].Trim(trim);
                            aRec.userId = controls[120].Trim(trim);

                            // cache the record
                            theControls.Add(aRec);                      

                            lineNumber++;
                        }
                        catch (System.Exception ex)
                        {
                            string errorMsg = "Error in File>" + fileName + " Line>" + lineNumber;
                            if (line != null)
                            {// add the line information if available
                                errorMsg += "Line>" + line;
                            }
                            LogFileError(errorMsg + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                        }
                    }
                }
            }// try
            catch (Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "Error in ParseJobControlFile-- error is " + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch

            return theControls;
        }// private void ParseJobControlFile()

        public void LogFileError(string msg)
        {
            try
            {
                FileStream file = new FileStream(m_LogFile, FileMode.Append, FileAccess.Write);

                // Create a new stream to write to the file
                StreamWriter sw = new StreamWriter(file);
                //sw.Write("----------\r\n");
                // Write a string to the file
                sw.Write(msg + "\r\n");
                //sw.Write("----------\r\n");
                // Close StreamWriter
                sw.Close();
                // Close file
                file.Close();
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "Error Ocurred While Writing to Error Log File>" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
            }
        }// public void LogFileError(string msg)
        
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
                LogFileError(DateTime.Now.ToShortDateString() + ":" + DateTime.Now.ToLongTimeString() + "-NFORMATIONAL:moved file: " + fileName + " to location: " + m_filemovePath + newFileName + "\r\n");
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                LogFileError("Could not move file: " + fileName + " to location: " + m_filemovePath + newFileName + "error>" + ex.ToString() + "\r\n");
            }

        }// MoveTheFile()

    }
}
