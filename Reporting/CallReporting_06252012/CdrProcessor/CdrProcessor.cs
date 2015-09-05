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

namespace TruMobility.Reporting.CDR
{
    public class CdrProcessor
    {
        // processing thread
        private Thread m_procThread = null;
        // note that this needs to correlate to the CDR db update interval
        private Int32 m_ReportInterval = 5 * 60 * 1000; // 5 mins
        private string m_eventLogName = "CallReportSvc"; // default value

        // default values for now, these are in the config file, see below
        private string m_reportInternalEmailList = String.Empty;
        private string m_reportExternalEmailList = String.Empty;

        // our db service
        private CdrDbProcessor m_db = null;
        private CdrReportProcessor m_processor = null;
        private TimeKeeper m_keeper = null;
        private ReportFormatter _reporter = null;

        public CdrProcessor()
        {
            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogFileName"];            // get the event log name
             
            // email distribution list
            m_reportExternalEmailList = System.Configuration.ConfigurationManager.AppSettings["EMailList"];

            // get the time to sleep between failed file open operations
            m_ReportInterval = 1000 * Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportIntervalInSecs"]);

            // get our db interface
            m_db = new CdrDbProcessor();
            m_keeper = new TimeKeeper();
            m_processor = new CdrReportProcessor();
            _reporter = new ReportFormatter();

        }

        /// <summary>
        /// processor thread that runs every interval to create the report
        /// </summary>
        private void ProcessCdrThread()
        {
            //int reportInterval = 0;
            TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            TimeSpan eightHours = new TimeSpan(0, 8, 0, 0);

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
                            List<CallReport> groupCallReport = m_processor.CreateGroupCallReport(rReportTime.Add(eightHours), rEndTime.Add(eightHours) );

                            // 
                            Hashtable spReport = m_processor.CreateSPReport(groupCallReport);

                            // create group call report
                            //SendGroupReportViaHtml( groupCallReport, "TruMobility", m_reportExternalEmailList  );
                            _reporter.SendGroupReportSummaryViaHtml(groupCallReport, "TruMobility", m_reportExternalEmailList, spReport );

                            //_reporter.GenerateExcelCallReportForGroups(groupCallReport,"DailyCallReport_");
                           
                        }

                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        LogFileMgr.Instance.WriteToLogFile("CdrProcessor::ProcessCdrThread():CdrProcessorSvc is stopping");
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        LogFileMgr.Instance.WriteToLogFile("CdrProcessor::ProcessCdrThread():ECaught:" + ex.Message);
                    }

                    System.Threading.Thread.Sleep( m_ReportInterval );

                }// while(true)
            }
            catch (System.Threading.ThreadAbortException tax)
            {
                LogFileMgr.Instance.WriteToLogFile( "CdrProcessor::ProcessCdrThread():ECaught:" + tax.Message );
            }
            catch (System.Exception ex)
            {
                LogFileMgr.Instance.WriteToLogFile("CdrProcessor::ProcessCdrThread():ECaught:" + ex.Message);
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
                LogFileMgr.Instance.WriteToLogFile("CdrProcessor::StartProcessing():ECaught:" + se.Message); 

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
            TimeSpan eightHours = new TimeSpan(0, 8, 0, 0);
            TimeSpan days = new TimeSpan(1, 0, 0, 0);

            // generate from our cumulative configurable param start date
            // this is the default for the one day report (daily report)
            //DateTime rReportTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).Subtract(days);
           // DateTime eTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            //      
           // for (int i = 9; i < 12;i++ )
           // {
                DateTime rReportTime = new DateTime(DateTime.Now.Year - 1, 7, 1, 0, 0, 0);
                DateTime eTime = new DateTime(DateTime.Now.Year-1, 8, 1, 0, 0, 0);
                // create group call report ,,,             // now compensate for the user time zone
                List<CallReport> groupCallReport = m_processor.CreateGroupCallReport(rReportTime.Add(eightHours), eTime.Add(eightHours));

                Hashtable spReport = m_processor.CreateSPReport(groupCallReport);

                _reporter.SendGroupReportSummaryViaHtml(groupCallReport, "TruMobility", m_reportExternalEmailList, spReport);
                //_reporter.GenerateExcelCallReportForGroups(groupCallReport, "DailyCallReport");
           // }
        }

    }//class

}//namespace
