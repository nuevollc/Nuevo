using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Threading;

using System.Data.SqlClient;
using System.Data;
using Strata8.Wireless.Cdr.Rating;

namespace Strata8.Wireless.Cdr
{

    /// <summary>
    ///  class to process the OMC CDR file
    /// 
    /// </summary>
    public class OmcCdrHandler
    {
        private OmcCdrDb m_db = null;

        private string m_eventLogName;
        private int m_ProcessThreadIntervalInMSecs;
        private string m_filemovePath;

        // queue which will contain all filenames messaged from the file watcher
        private Queue m_fileNameQ;
        // processing thread
        private Thread m_procThread;

        /// <summary>
        /// ctor
        /// </summary>
        public OmcCdrHandler()
        {
            m_db = new OmcCdrDb();

            // the the path where to move processed files
            m_filemovePath = ConfigurationManager.AppSettings["OmcCdrHandlerMoveFolder"];
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
                EventLog.WriteEntry(m_eventLogName, "OmcCdrHanlder::ProcessFileWatcher():ECaught trying to enqueue a filename::" + ex.ToString(), EventLogEntryType.Error, 3014);
            }

        }//private void ProcessFileWatcher(object sender, FileSystemEventArgs e)

        /// <summary>
        /// this method parses the OMC CDR files that are in the XML format
        /// the entire file is parsed so that each CDR is parsed and the OmcCdr object is loaded.
        /// The OmcCdr is then passed to create CIBER records if it is one of our roaming partners.
        /// </summary>
        public List<OmcCdr> ProcessOmcCdrFile(string fileName)
        {

            XmlTextReader reader = null;
            int cdrIndx = 0;
            int cdrCnt = 0;
            List<OmcCdr> cdrList = new List<OmcCdr>();

            // make sure we have not processed this file before
            string parsedFileName = this.ParseFileName(fileName);
            if (!this.CheckDbBeforeProcessingFile(parsedFileName))
            {

                try
                {
                    // Load the reader with the data file and ignore 
                    // all white space nodes.  this needs the full path name
                    reader = new XmlTextReader(fileName);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    OmcCdr cdr = null;

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                {
                                    switch (reader.Name)
                                    {

                                        case ("CDRecord"):
                                            {
                                                if (cdrIndx == 0)
                                                {
                                                    cdr = new OmcCdr();
                                                }
                                                cdrIndx++;
                                            }
                                            break;
                                        case ("version"):
                                            reader.Read();
                                            cdr.version = reader.Value;
                                            break;
                                        case ("seq_num"):
                                            reader.Read();
                                            cdr.seq_num = reader.Value;
                                            break;
                                        case ("record_type"):
                                            reader.Read();
                                            cdr.type = reader.Value;
                                            break;
                                        case ("a_party_num"):
                                            reader.Read();
                                            cdr.a_party_num = reader.Value;
                                            break;
                                        case ("b_party_num"):
                                            reader.Read();
                                            cdr.b_party_num = reader.Value;
                                            break;
                                        case ("a_party_type"):
                                            reader.Read();
                                            cdr.a_party_type = reader.Value;
                                            break;
                                        case ("b_party_type"):
                                            reader.Read();
                                            cdr.b_party_type = reader.Value;
                                            break;
                                        case ("a_party_digits"):
                                            reader.Read();
                                            cdr.a_party_digits = reader.Value;
                                            break;
                                        case ("b_party_digits"):
                                            reader.Read();
                                            cdr.b_party_digits = reader.Value;
                                            break;
                                        case ("a_party_trunk"):
                                            reader.Read();
                                            cdr.a_party_trunk = reader.Value;
                                            break;
                                        case ("b_party_trunk"):
                                            reader.Read();
                                            cdr.b_party_trunk = reader.Value;
                                            break;
                                        case ("a_party_trkgrp"):
                                            reader.Read();
                                            cdr.a_party_trkgrp = reader.Value;
                                            break;
                                        case ("b_party_trkgrp"):
                                            reader.Read();
                                            cdr.b_party_trkgrp = reader.Value;
                                            break;
                                        case ("seize"):
                                            reader.Read();
                                            cdr.seize = reader.Value;
                                            break;
                                        case ("answer"):
                                            reader.Read();
                                            cdr.answer = reader.Value;
                                            break;
                                        case ("disc"):
                                            reader.Read();
                                            cdr.disc = reader.Value;
                                            break;
                                        case ("disc_code"):
                                            reader.Read();
                                            cdr.disc_code = reader.Value;
                                            break;
                                        case ("disc_reason"):
                                            reader.Read();
                                            cdr.disc_reason = reader.Value;
                                            break;
                                        case ("msc_id"):
                                            reader.Read();
                                            cdr.msc_id = reader.Value;
                                            break;
                                        case ("orig_esn"):
                                            reader.Read();
                                            cdr.orig_esn = reader.Value;
                                            break;
                                        case ("term_esn"):
                                            reader.Read();
                                            cdr.TerminatingEsn = reader.Value;
                                            break;
                                        case ("cell_id"):
                                            reader.Read();
                                            cdr.cell_id = reader.Value;
                                            break;
                                        case ("b_cell_id"):
                                            reader.Read();
                                            cdr.b_cell_id = reader.Value;
                                            break;
                                        case ("a_feature_bits"):
                                            reader.Read();
                                            cdr.a_feature_bits = reader.Value;
                                            break;
                                        case ("b_feature_bits"):
                                            reader.Read();
                                            cdr.b_feature_bits = reader.Value;
                                            break;
                                        case ("o_msisdn"):
                                            reader.Read();
                                            cdr.o_msisdn = reader.Value;
                                            break;
                                        case ("t_msisdn"):
                                            reader.Read();
                                            cdr.t_msisdn = reader.Value;
                                            break;
                                        case ("o_exchange"):
                                            reader.Read();
                                            cdr.o_exchange = reader.Value;
                                            break;
                                        case ("t_exchange"):
                                            reader.Read();
                                            cdr.t_exchange = reader.Value;
                                            break;
                                        case ("o_market_id"):
                                            reader.Read();
                                            cdr.o_market_id = reader.Value;
                                            break;
                                        case ("o_swno"):
                                            reader.Read();
                                            cdr.o_swno = reader.Value;
                                            break;
                                        case ("o_bin"):
                                            reader.Read();
                                            cdr.o_bin = reader.Value;
                                            break;
                                        case ("t_market_id"):
                                            reader.Read();
                                            cdr.t_market_id = reader.Value;
                                            break;
                                        case ("t_swno"):
                                            reader.Read();
                                            cdr.t_swno = reader.Value;
                                            break;
                                        case ("t_bin"):
                                            reader.Read();
                                            cdr.t_bin = reader.Value;
                                            break;
                                        case ("o_billdgts"):
                                            reader.Read();
                                            cdr.o_billdgts = reader.Value;
                                            break;
                                        case ("t_billdgts"):
                                            reader.Read();
                                            cdr.t_billdgts = reader.Value;
                                            break;
                                        case ("o_serviceid"):
                                            reader.Read();
                                            cdr.o_serviceid = reader.Value;
                                            break;
                                        case ("t_serviceid"):
                                            reader.Read();
                                            cdr.t_serviceid = reader.Value;
                                            break;
                                        case ("crg_charge_info"):
                                            reader.Read();
                                            cdr.crg_charge_info = reader.Value;
                                            break;
                                        case ("ocpn"):
                                            reader.Read();
                                            cdr.Ocpn = reader.Value;
                                            break;
                                        case ("icprn"):
                                            reader.Read();
                                            cdr.Icprn = reader.Value;
                                            break;
                                    }
                                }
                                break;
                            case XmlNodeType.Text:
                                break;
                            case XmlNodeType.CDATA:
                                break;
                            case XmlNodeType.ProcessingInstruction:
                                break;
                            case XmlNodeType.Comment:
                                break;
                            case XmlNodeType.XmlDeclaration:
                                break;
                            case XmlNodeType.Document:
                                break;
                            case XmlNodeType.DocumentType:
                                break;
                            case XmlNodeType.EntityReference:
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.Equals("CDRecord"))
                                {
                                    // this is the end of the CDR record we are processing
                                    cdrIndx = 0;

                                    // increment the total number of CDRs we are processing for our Record98
                                    cdrCnt++;

                                    // add to the list and then store entire list at once instead of one by one
                                    cdrList.Add(cdr);
                                }

                                break;
                        }//switch

                    }// while loop

                }//try
                catch (SystemException ex)
                {
                    WriteToLogFile("OmcCdrHandler::ProcessOmcCdrFile():ECaught:CDR#" + cdrCnt + " \r\n" + ex.Message + ex.StackTrace);
                }


                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }

            return cdrList;

        }// ProcessOmcCdrFile

        private void UpdateFileInfo(string fileName)
        { 
            // update our file info to reflect that we have also
            // stored the CDRs in the database
            m_db.UpdateDateOmcCdrsStored(fileName);
        
        }// UpdateFileInfo

        private bool StoreCdrsInDb(List<OmcCdr> cdrList)
        {
            bool stored = true;
            m_db.StoreCdrInDb( cdrList );
            return stored;
        }

        private void WriteToLogFile(string msg)
        {
            OmcCdrFileWriter.Instance.WriteToLogFile(msg);

        }// public void WriteToFile(string msg)

        private bool CheckDbBeforeProcessingFile(string fileName)
        {
            OmcCdrFileInfo info = m_db.GetOmcCdrInfo(fileName);
            if (info.StoredInDb == 1)
                return true;
            return false;

        }//CheckDbBeforeDownloadingFile

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        private void UpdateProcessedFileNameInDb( string fileName )
        {
            m_db.UpdateDateOmcCdrsStored( fileName );

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
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["OmcCdrHandlerEventLogName"];
            // set up event logging
            if (!EventLog.SourceExists(m_eventLogName))
            {
                EventLog.CreateEventSource(m_eventLogName, "Application");
            }
            string watchFolder = System.String.Empty;

            // get the time to sleep between failed file open operations
            m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["OmcCdrHandlerProcessThreadIntervalInSecs"]);

            try
            {
                // create the Watcher for file type in the watchfolder; config params
                watchFolder = ConfigurationManager.AppSettings["OmcCdrHandlerWatchFolder"];
                string fileType = ConfigurationManager.AppSettings["OmcCdrHandlerWatchFileType"];
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

                EventLog.WriteEntry(m_eventLogName, "OmcCdrHandler::StartProcessing():WatchFolder:"+watchFolder + " FileType:"+fileType, EventLogEntryType.Information, 2000);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcCdrHandler::StartProcessing():Error setting up watch folder>" + watchFolder + " Error>" + ex.ToString(), EventLogEntryType.Error, 2000);
            }
        }// public void StartProcessing()

        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        private void ProcessJobControlFileThread()
        {
            List<OmcCdr> theCdrs;
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
                            theCdrs = this.ProcessOmcCdrFile(fName);

                            // store the cdrs in the database
                            bool dbUpdate = this.StoreCdrsInDb(theCdrs);

                            // housekeeping, update our database that we have processed this file
                            UpdateFileInfo(parsedFileName);

                            // move the file with the full path name 
                            this.MoveTheFile(fName);
                        }

                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        EventLog.WriteEntry(m_eventLogName, "OmcCdrHandler::ProcessJobControlFile():Service is stopping", EventLogEntryType.Information, 2001);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        EventLog.WriteEntry(m_eventLogName, "OmcCdrHandler::ProcessJobControlFile():ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);
                }// while(true)
            }
            catch (System.Threading.ThreadAbortException)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcCdrHandler::ProcessJobControlFile():ECaught: thread is shutting down", EventLogEntryType.Information, 2001);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcCdrHandler::ProcessJobControlFile():ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);
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
                WriteToLogFile(DateTime.Now.ToShortDateString() + ":" + DateTime.Now.ToLongTimeString() + "-NFORMATIONAL:moved file: " + fileName + " to location: " + m_filemovePath + newFileName + "\r\n");
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                WriteToLogFile("Could not move file: " + fileName + " to location: " + m_filemovePath + newFileName + "error>" + ex.ToString() + "\r\n");
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

    }//class


}//namespace
