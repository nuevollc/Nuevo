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

namespace Strata8.Wireless.Cdr
{

    /// <summary>
    ///  class to process the OMC CDR file
    /// 
    /// </summary>
    public class FileHandler
    {
        private string m_eventLogName;
        private int m_ProcessThreadIntervalInMSecs;

        private int m_dailyRunTime = 0;

        private FileReadWriteMgr m_frwMgr = new FileReadWriteMgr();

        //File
        // processing thread
        private Thread m_procThread;

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
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["ReadWriteManagerEvtFile"];

            m_dailyRunTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportTimeToRunAtIn24HourTimeReference"]);

            // set up event logging
            if (!EventLog.SourceExists(m_eventLogName))
            {
                EventLog.CreateEventSource(m_eventLogName, "Application");
            }
            string watchFolder = System.String.Empty;

            // get the time to sleep between failed file open operations
            m_ProcessThreadIntervalInMSecs = 1000 * Convert.ToInt32(ConfigurationManager.AppSettings["ProcessThreadIntervalInSecs"]);

            try
            {
                // launch the processing thread sleep and process the files
                m_procThread = new Thread(new ThreadStart(FileThreadProcessor));
                m_procThread.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "FileHandler::StartProcessing():Error setting up watch folder>" + watchFolder + " Error>" + ex.ToString(), EventLogEntryType.Error, 2000);
            }
        }// public void StartProcessing()

        /// <summary>
        /// FileThreadProcessor:Method that sleeps for the configured interval
        /// and then calls the file mgr object to read the directory and create a file for ciber processing to handle.
        /// </summary>
        private void FileThreadProcessor()
        {
            try
            {
                // our base reference time that we calculate from, this will be from midnight yesterday
                DateTime rRunTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, m_dailyRunTime, 0, 0);

                while (true)
                {
                    try
                    {
                        // our daily run interval
                        DateTime tNow = DateTime.Now;
                        TimeSpan t = tNow.Subtract(rRunTime);
                        if (t.TotalHours > 24)
                        {
                            m_frwMgr.ProcessFiles();

                            // setup to run at the same time tomorrow
                            rRunTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_dailyRunTime, 0, 0);
                        }


                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        EventLog.WriteEntry(m_eventLogName, "FileHandler::FileThreadProcessor():Service is stopping", EventLogEntryType.Information, 2001);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        EventLog.WriteEntry(m_eventLogName, "FileHandler::FileThreadProcessor():ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);

                    }

                    System.Threading.Thread.Sleep(m_ProcessThreadIntervalInMSecs);

                }// while(true)
            }
            catch (System.Threading.ThreadAbortException)
            {
                EventLog.WriteEntry(m_eventLogName, "FileHandler::FileThreadProcessor():ECaught: thread is shutting down", EventLogEntryType.Information, 2001);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "FileHandler::FileThreadProcessor():ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);
            }// catch
            finally
            {
                // clean up

            }// finally

        }// private void ProcessJobControlFile()

    }//class


}//namespace
