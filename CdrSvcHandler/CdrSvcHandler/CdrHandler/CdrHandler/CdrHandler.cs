
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
using TruMobility.Utils.Logging;

namespace Strata8.Telephony.MiddleTier.Services.CDR
{

    /// <summary>
    /// CdrHandler : Responsible for the Cdr processing.
    /// </summary>
    public class CdrHandler
    {
        private const int PROCESS_SLEEP_PERIOD = 1000;
        private string m_filemovePath;
        private int m_ProcessThreadIntervalInMSecs;    
        private string m_ServiceProviderToGetCdrsFor;
        private string m_getServiceProviderCdrs = "TRUE";
        private List<string> m_serviceProviders = new List<string>();
        private List<FtpSiteInfo> m_ftpSites = new List<FtpSiteInfo>();
 
        // directory where to put the service provider cdrs
        private string m_ServiceProviderDirectory = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderDirectory"];
        private string m_connection = ConfigurationManager.AppSettings["BWorksCdr_SQLConnectString"];

        // ftp site info for each site
        private string m_ServiceProviderFTPSite;
        private string m_ServiceProviderFTPUsername;
        private string m_ServiceProviderFTPPassword;

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

        private void LogIt( string msg )
        {
            LogFileMgr.Instance.WriteToLogFile(msg);
        }

        public void StartProcessing()
        {
            // set up event logging
            //if (!EventLog.SourceExists(m_eventLogName))
            //{
            //    EventLog.CreateEventSource(m_eventLogName, "Application");
            //}
            // the the path where to move processed files
            m_filemovePath = ConfigurationManager.AppSettings["MoveFolder"];
            string watchFolder = System.String.Empty;

            // get the time to sleep between failed file open operations
            m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["ProcessThreadIntervalInSecs"]);

            // turn on the option to get service provider CDRs
            m_getServiceProviderCdrs = ConfigurationManager.AppSettings["GetServiceProviderCdrs"];
            
            // get the names of the service providers to get cdrs for
            m_ServiceProviderToGetCdrsFor = ConfigurationManager.AppSettings["ServiceProviderToGetCdrsFor"];
            m_serviceProviders = this.ParseList(m_ServiceProviderToGetCdrsFor);

            this.m_ftpSites = GetSiteInfo( m_serviceProviders );

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
                LogIt("CdrHandler::StartProcessing()::ExceptionCaught:" + ex.Message);
            }
        }// public void StartProcessing()

        private List<FtpSiteInfo> GetSiteInfo( List<string> sp )
        {
            List<FtpSiteInfo> si = new List<FtpSiteInfo>();

            // get the site credentials for each site
            m_ServiceProviderFTPSite = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderFTPSite"];
            m_ServiceProviderFTPUsername = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderFTPUsername"];
            m_ServiceProviderFTPPassword = System.Configuration.ConfigurationManager.AppSettings["ServiceProviderFTPPassword"];
            List<string> m_sites = this.ParseList(m_ServiceProviderFTPSite);
            List<string> m_unames = this.ParseList(m_ServiceProviderFTPUsername);
            List<string> m_pwds = this.ParseList(m_ServiceProviderFTPPassword);

            try
            {
                int i = 0;
                foreach (string s in sp)
                {
                    FtpSiteInfo f = new FtpSiteInfo();
                    f.ServiceProvider = s;
                    f.Site = m_sites[i];
                    f.Username = m_unames[i];
                    f.Password = m_pwds[i];

                    // add the site to our list
                    si.Add(f);
                    i++;

                }
            }
            catch (SystemException se)
            {
                LogIt("CdrHandler::GetSiteInfo()::ExceptionCaught:" + se.Message);
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
                LogIt("CdrHandler::ProcessFileWatcher()::ExceptionCaught:" + ex.Message);
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
                    catch (System.Threading.ThreadAbortException ta)
                    {
                        LogIt("CdrHandler::ProcessJobControlFileThread()::ExceptionCaught:" + ta.Message);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic system exception

                        LogIt("CdrHandler::ProcessJobControlFileThread()::ExceptionCaught:" + ex.Message);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);
                
                }// while(true)
            
            }
            catch (System.Threading.ThreadAbortException ta)
            {
                LogIt("CdrHandler::ProcessJobControlFileThread()::ExceptionCaught:" + ta.Message);
            }
            catch (System.Exception ex)
            {
                LogIt("CdrHandler::ProcessJobControlFileThread()::ExceptionCaught:" + ex.Message);
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
        private bool InsertCdrIntoDb(ArrayList theCdr)
        {
            string theTime;
            int controlNbr = 0;

            foreach ( BworksCdr bCdr in theCdr )
            {
                try
                {
                    // make the connection to insert the cdr into the database
                    using (SqlConnection connection = new SqlConnection(m_connection))
                    {
                        connection.Open();

                        StringBuilder cmdStr = new StringBuilder("INSERT INTO BworksCdr ");
                        cmdStr.Append("(recordId,serviceProvider,type,userNumber,groupNumber,");
                        cmdStr.Append("direction,callingNumber,callingPresentationIndicator,calledNumber,");
                        cmdStr.Append("startTime,userTimeZone, answerIndicator,");
                        cmdStr.Append("answerTime,releaseTime,terminationCause,networkType,");// carrierIndentificationCode, ");
                        cmdStr.Append("dialedDigits,callCategory,networkCallType,networkTranslatedNumber,");
                        cmdStr.Append("networkTranslatedGroup, releasingParty, route, networkCallId, codec,");
                        cmdStr.Append("department, originalCalledNumber, originalCalledReason,redirectingNumber,redirectingReason,");
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
                        // cmdStr.Append(",'" + bCdr.carrierIdentificationCode + "'");

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

                        // department, originalCalledNumber (reason), redirectingNumber (reason)
                        cmdStr.Append(",'" + bCdr.department + "'");
                        cmdStr.Append(",'" + bCdr.originalCalledNumber + "'");
                        cmdStr.Append(",'" + bCdr.originalCalledReason + "'");
                        cmdStr.Append(",'" + bCdr.redirectingNumber + "'");
                        cmdStr.Append(",'" + bCdr.redirectingReason + "'");

                        // accessDeviceAddress, groupId, chargeIndicator)
                        cmdStr.Append(",'" + bCdr.accessDeviceAddress + "'");
                        cmdStr.Append(",'" + bCdr.group + "'");
                        cmdStr.Append(",'" + bCdr.chargeIndicator + "'");
                        cmdStr.Append(",'" + bCdr.conferenceId + "'");
                        cmdStr.Append(",'" + bCdr.userId + "')");

                        SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), connection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();
                        controlNbr++;

                    }// sql connection closed

                }//try

                catch (System.Exception ex)
                {
                    LogIt("CdrHandler::InsertCdrIntoDb()::ExceptionCaught:" + ex.Message);
                }

            }//for each CDR

            return true;

        } // insertcdrintodb
    
        /// <summary>
        /// private method to parse the cdr
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private ArrayList ParseTheCdr(string fileName)
        {
            // make the array size (number of cdrs per file ) configurable
            System.Collections.ArrayList theControls = new System.Collections.ArrayList(1000);

            // our service provider handler to create a separate file to write the CDRs to
            ServiceProviderCdrHandler spHandler = new ServiceProviderCdrHandler();
            
            // set up the filenames
            string newFileName = this.ParseFileName(fileName);
            foreach (FtpSiteInfo s in m_ftpSites)
            {
                s.Filename = m_ServiceProviderDirectory + s.ServiceProvider + newFileName.Substring(2) + ".csv";
            }   
    
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

                            if (this.m_getServiceProviderCdrs.Equals("TRUE"))
                            {
                                foreach (FtpSiteInfo s in m_ftpSites )
                                {
                                    spHandler.ProcessServiceProviderCdrs(fileName, line, s );
                                }
                            }

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
                            // added to help determine call flows
                            aRec.department = controls[32].Trim(trim);
                            aRec.originalCalledNumber = controls[35].Trim(trim);
                            aRec.originalCalledReason = controls[37].Trim(trim);
                            aRec.redirectingNumber = controls[38].Trim(trim);
                            aRec.redirectingReason = controls[40].Trim(trim);
                       
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
                            LogIt("CdrHandler::ParseTheCdr()::ExceptionCaught:" + errorMsg + "::" + ex.Message);
                        }

                    }// while more lines to read             
                
                }// using streamreader to read

                // post each service provider CDR file to the ftp site
                foreach (FtpSiteInfo s in m_ftpSites)
                {
                    spHandler.PostFileToSite( s );
                } 

            }// try
            catch (Exception e)
            {
                LogIt("CdrHandler::ParseTheCdr()::ExceptionCaught:" + e.Message);
            }// catch

            return theControls;
        }// private void ParseJobControlFile()

        
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
                LogIt("CdrHandler::MoveTheFile()::File:" + fileName + "  MovedTo::" + m_filemovePath + newFileName);
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                LogIt("CdrHandler::MoveTheFile()::ExceptionCaught:" + ex.Message);
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
    }
}
