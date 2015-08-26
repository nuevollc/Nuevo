using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Net.Mail;
using System.Threading;
using System.Diagnostics;
using System.Net.Mime;
using System.IO;

using TruMobility.Utils.Logging;

namespace TruMobility.Reporting.CDR.Groups
{
    public class GroupReportProcessor
    {
        private static string _group = System.Configuration.ConfigurationManager.AppSettings["Group"];

        // processing thread
        private Thread m_procThread = null;
        // note that this needs to correlate to the CDR db update interval
        private Int32 m_ReportInterval = 5 * 60 * 1000; // 5 mins
        private string m_eventLogName = "CallReportSvc"; // default value

        // parameters for the excel call report
        private string m_emailList = String.Empty;
        private string m_bccemailList = String.Empty;
        private string m_group = String.Empty;

        // our db service
        private CdrDbProcessor m_db = null;
        private GroupReportDataProcessor m_processor = null;
        private TimeKeeper m_keeper = null;
        private ReportFormatter _reportFormatter = null;

        public GroupReportProcessor()
        {
            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogFileName"];            // get the event log name

            // email to list
            m_emailList = System.Configuration.ConfigurationManager.AppSettings["EMailList"]; 

            // get the time to sleep between failed file open operations
            m_ReportInterval = 1000 * Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportIntervalInSecs"]);

            // get our db interface
            m_db = new CdrDbProcessor();
            m_keeper = new TimeKeeper();
            m_processor = new GroupReportDataProcessor();
            _reportFormatter = new ReportFormatter();

        }

        /// <summary>
        /// processor thread that runs every interval to create the report
        /// </summary>
        private void ProcessCdrThread()
        {
            TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            TimeSpan eightHours = new TimeSpan(0, 8, 0, 0);
            TimeSpan fourHours = new TimeSpan(0, 4, 0, 0);

            try
            {
                while (true)
                {
                    try
                    {

                        // check if our 24 hour interval has passed
                        if (m_keeper.DayPassed())
                        {
                            // call report from midnight 
                            // generate from our cumulative configurable param start date
                            DateTime rReportTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).Subtract(ts);
                            DateTime rEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

                            // create group call report
                            // right now we are assuming all users are on the timezone of -8 hours 
                            CallReport groupCallReport = m_processor.CreateGroupCallReport(_group, rReportTime.Add(fourHours), rEndTime.Add(fourHours));

                            // create group call report
                            _reportFormatter.GenerateExcelUserCallReport(groupCallReport.UserCallReportList, m_emailList );
                        }

                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        LogFileMgr.Instance.WriteToLogFile("GroupReportProcessor::ProcessCdrThread():CdrProcessorSvc is stopping");
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        LogFileMgr.Instance.WriteToLogFile("GroupReportProcessor::ProcessCdrThread():ECaught:" + ex.Message);
                    }

                    System.Threading.Thread.Sleep( m_ReportInterval );

                }// while(true)
            }
            catch (System.Threading.ThreadAbortException tax)
            {
                LogFileMgr.Instance.WriteToLogFile("GroupReportProcessor::ProcessCdrThread():ECaught:" + tax.Message);
            }
            catch (System.Exception ex)
            {
                LogFileMgr.Instance.WriteToLogFile("GroupReportProcessor::ProcessCdrThread():ECaught:" + ex.Message);
            }// catch

            finally
            {

            }// finally

        }
         
        /// <summary>
        /// main thread to handle fraud detection
        /// </summary>
        public void StartProcessing()
        {
            // set up event logging
            try
            {
                if (!EventLog.SourceExists(m_eventLogName))
                {
                    EventLog.CreateEventSource(m_eventLogName, "Application");
                }
            }
            catch (SystemException se)
            {
                LogFileMgr.Instance.WriteToLogFile("GroupReportProcessor::StartProcessing():ECaught:" + se.Message); 

            }

            // launch the job control processing thread monitoring the queue
            m_procThread = new Thread(new ThreadStart(ProcessCdrThread));
            m_procThread.Start();
        }

        /// <summary>
        /// main thread to handle stopping the fraud detector object
        /// </summary>
        public void StopProcessing()
        {
            if (null != m_procThread)
            {
                // stop the watch thread
                m_procThread.Abort();
            }
        }


        /// <summary>
        /// method to test the call report generation
        /// and email 
        /// </summary>
        public void TestCallReport()
        {
        
            string emailList = System.Configuration.ConfigurationManager.AppSettings["EMailList"];
            TimeSpan eightHours = new TimeSpan(0, 8, 0, 0);
            TimeSpan days = new TimeSpan(1, 0, 0, 0); 

            // generate from our cumulative configurable param start date
            // this is the default for the one day report (daily report)
            DateTime rReportTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).Subtract(days);
            DateTime eTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            // create group call report ,,, compensate for the user time zone that we are in
            CallReport groupCallReport = m_processor.CreateGroupCallReport(_group, rReportTime.Add(eightHours), eTime.Add(eightHours));

            _reportFormatter.GenerateExcelUserCallReport( groupCallReport.UserCallReportList, emailList );

        }

    }//class

}//namespace
