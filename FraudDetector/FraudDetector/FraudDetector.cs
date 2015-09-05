using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
//using System.Web.Mail;
using System.Net.Mail;


namespace TruMobility.Services.Fraud
{
    /// <summary>
    /// Fraud Detector object that is used to identify unusual call patterns.
    /// This first iteration is used to calculate the total number of calls along
    /// with the total number of International calls during a configurable interval.
    /// </summary>
    public class FraudDetector
    {


        // processing thread
        private Thread m_procThread = null;

        // note that this needs to correlate to the CDR db update interval
        private Int32 m_FraudDetectInterval = 5*60*1000; // 5 mins
        private string m_eventLogName = "FraudDetectorSvc"; // default value

        private string m_emailList = string.Empty;
        private string m_smtpserver = null;
        private double m_cdrdelay = -15;

        private int m_internationalCallLimit = 50;
        private int m_CallLimit = 50;


        // our db interface
        private DbInterface db = null;

        /// <summary>
        /// The fraud detector class is 
        /// </summary>
        public FraudDetector()
        {
            // TODO: Add constructor logic here

            // get the time to sleep between failed file open operations
            m_FraudDetectInterval = 1000*Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FraudCheckIntervalInSecs"]);

            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogname"];

            // get the event log name
            m_emailList = System.Configuration.ConfigurationManager.AppSettings["ToEmailList"];

            // get the SMTP Server
            m_smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];

            // get the CDR delay interval
            m_cdrdelay = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["CDRDelayIntervalInMins"]);

            // get the CDR delay interval
            m_internationalCallLimit = Convert.ToInt16( System.Configuration.ConfigurationManager.AppSettings["InternationalCallLimit"]);
            // get the CDR delay interval
            m_CallLimit = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["TotalCallLimit"]);
            // db interface
            db = new DbInterface();

        }

        /// <summary>
        /// method to get the international cdr records
        /// </summary>
        public int GetInternationalCalls()
        {
            // add code here to get the CDRs
            int numCalls = db.CountInternationalCalls( this.m_cdrdelay );
            return numCalls;
        }

        /// <summary>
        /// method to get all the calls for this interval
        /// </summary>
        public int GetTotalCalls()
        {
            // add code here to get the CDRs
            int numCalls = db.CountAllCalls(this.m_cdrdelay);
            return numCalls;
        }

        /// <summary>
        /// method to apply the rules to detect fraud
        /// </summary>
        public string CheckCalls(int totalCalls, int intlCalls )
        {
            string msg = string.Empty;

            if (totalCalls > this.m_CallLimit)
                msg = "***TotalCallLimitExceeded: ";
            else if (intlCalls > this.m_internationalCallLimit)
                msg = "***IntlCallLimitExceeded: ";
            else
                return msg;

            return msg;

        }

        /// <summary>
        /// method to send out a CDR report notification
        /// </summary>
        /// <param name="numInternationalCalls"></param>
        /// <param name="totalCalls"></param>
        public void SendCallReportNotification( int numInternationalCalls, int totalCalls, string errorMsg )
        {
            try
            {
                SmtpClient sclient = new SmtpClient( this.m_smtpserver );

                string To = this.m_emailList;
                string Subject = errorMsg + "#IC:"+numInternationalCalls.ToString() + ", "+"#TC:"+totalCalls.ToString();
                string From = "FraudDetector@TruMobility.com";

                StringBuilder Body = new StringBuilder( "#International Calls:" + numInternationalCalls.ToString()+ "\r\n" +
                    "#Total Calls: " + totalCalls.ToString() );
                Body.Append("\r\n" + errorMsg);

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage(From, To, Subject, Body.ToString() );
                sclient.Send(emsg);
            }
            catch( System.Net.Mail.SmtpException sme )
            {// exception handling
                EventLog.WriteEntry(m_eventLogName, "FraudDetector::SendNotification():ECaught:" + sme.ToString(), EventLogEntryType.Error, 3000);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessFraudDetectionThread()
        {
            SqlDataAdapter dataDapter = null;
            DataSet dataSet = new DataSet();
            try
            {
                while (true)
                {
                    try
                    {
                        // get the cdr dataset every 20 mins
                        int numberOfInternationalCalls = this.GetInternationalCalls();

                        // get all calls for this interval to log
                        // add code later to work on the same dataset already in memory
                        // TODO::
                        int numberOfTotalCalls = this.GetTotalCalls();

                        // apply fraud rules and take action if fraud detected
                        string msg = CheckCalls( numberOfTotalCalls, numberOfInternationalCalls);

                        // we always send out the call report notification
                        // for now manual fraud detection, send an email out
                        // ** changed to only send out if call limits exceeded
                        if ( !msg.Equals( string.Empty ) )
                            this.SendCallReportNotification(numberOfInternationalCalls, numberOfTotalCalls, msg);

                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        EventLog.WriteEntry(m_eventLogName, "FraudDetector::ProcessFraudDetectionThread():Fraud Service is stopping", EventLogEntryType.Information, 2001);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        EventLog.WriteEntry(m_eventLogName, "FraudDetector::ProcessFraudDetectionThread():ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);
                    }

                    EventLog.WriteEntry(m_eventLogName, "FraudDetector::ProcessFraudDetectionThread():FraudServiceIntervalComplete:Waiting: "+this.m_FraudDetectInterval.ToString()+"msecs ",
                        EventLogEntryType.Information, 2001);

                    System.Threading.Thread.Sleep( m_FraudDetectInterval );
                }// while(true)
            }
            catch (System.Threading.ThreadAbortException tax)
            {
                EventLog.WriteEntry(m_eventLogName, "FraudDetector::ProcessFraudDetectionThread():ECaught:" + tax.ToString(), EventLogEntryType.Error, 2001);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "FraudDetector::ProcessFraudDetectionThread():Exception Is:" + ex.ToString(), EventLogEntryType.Error, 2001);
            }// catch

            finally
            {
                // clean up
                if (dataDapter != null)
                {
                    dataDapter = null;
                }

            }// finally

        }



        /// <summary>
        /// main thread to handle fraud detection
        /// </summary>
        public void StartProcessing()
        {     
            // set up event logging
            if (!EventLog.SourceExists(m_eventLogName))
            {
                EventLog.CreateEventSource(m_eventLogName, "Application");
            }
 
            // launch the job control processing thread monitoring the queue
            m_procThread = new Thread(new ThreadStart(ProcessFraudDetectionThread));
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


    }
}
