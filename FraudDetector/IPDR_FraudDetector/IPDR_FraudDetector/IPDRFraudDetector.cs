using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Diagnostics;

using TruMobility.Utils.Logging;


namespace TruMobility.Services.Fraud
{
    /// <summary>
    /// IPDR Fraud detector for Sprint IPDR data
    /// Currently doing a simple check for data usage over an hour
    /// </summary>
    public class IPDRFraudDetector
    {
        // processing thread
        private Thread m_procThread = null;

        // note that this needs to correlate to the CDR db update interval
        private Int32 m_FraudDetectInterval = 30 * 60 * 1000; // 30 mins

        private string m_emailList = string.Empty;
        private string m_smtpserver = null;
        private double m_cdrdelay = -15;
         
        private int m_DataLimit = 50;

        // our db interface
        private FraudDbMgr _db = null;

        /// <summary>
        /// The fraud detector class is 
        /// </summary>
        public IPDRFraudDetector()
        {
            // TODO: Add constructor logic here

            // get the time to sleep between failed file open operations
            m_FraudDetectInterval = 1000 * Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FraudCheckIntervalInSecs"]);

            // get the event log name
            m_emailList = System.Configuration.ConfigurationManager.AppSettings["ToEmailList"];

            // get the SMTP Server
            m_smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];

            // get the IPDR CDR delay interval
            m_cdrdelay = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["CDRDelayIntervalInMins"]);

            // get the IPDR data limit per hour
            m_DataLimit = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["TotalDataLimitInKBPerHour"]);

            // db interface
            _db = new FraudDbMgr();

        }

        /// <summary>
        /// method to apply the rules to detect fraud
        /// </summary>
        public void CheckData( Hashtable dataUsage )
        {
            string msg = string.Empty;

            // Get our users
            List<string> rUsers = _db.GetUsers(); // list of users

            // only one row with the sidbid
            foreach (string user in rUsers )
            {
                if (dataUsage.ContainsKey(user))
                {
                    AMSData d = (AMSData)dataUsage[user];
                    int total = d.DataIn + d.DataOut;
                    if (total > this.m_DataLimit)
                    {
                        SendCallReportNotification(user, total);
                    }
                }

            }

        }//CheckData

        /// <summary>
        /// method to send out a CDR report notification
        /// </summary>
        /// <param name="numInternationalCalls"></param>
        /// <param name="totalCalls"></param>
        public void SendCallReportNotification( string rUser, int rDataUsage)
        {
            try
            {
                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = this.m_emailList;
                string Subject = "User:" + rUser + ", " + "HasDataUsage:" + rDataUsage.ToString();
                string From = "IPDRUsageDetector@TruMobility.com";

                StringBuilder Body = new StringBuilder("User:" + rUser + "\r\n" +
                    "***TotalDataLimitExceeded: #DataUsage: " + rDataUsage.ToString());
                Body.Append("\r\n" );

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage(From, To, Subject, Body.ToString());
                sclient.Send(emsg);
            }
            catch (System.Net.Mail.SmtpException sme)
            {// exception handling
                LogMsg("FraudDetector::SendNotification():ECaught:" + sme.Message );
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void ProcessFraudDetectionThread()
        { 
            try
            {
                while (true)
                {
                    try
                    {

                        // get all calls for this interval to log
                        Hashtable dataUsage = this.GetDataUsage();

                        // apply fraud rules and take action if fraud detected
                        this.CheckData(dataUsage);


                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                       LogMsg( "FraudDetector::ProcessFraudDetectionThread():Fraud Service is stopping" );
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                       LogMsg( "FraudDetector::ProcessFraudDetectionThread():ECaught:" + ex.Message );
                    };

                    System.Threading.Thread.Sleep(m_FraudDetectInterval);
                }// while(true)
            }
            catch (System.Threading.ThreadAbortException tax)
            {
               LogMsg("FraudDetector::ProcessFraudDetectionThread():ECaught:" + tax.Message );
            }
            catch (System.Exception ex)
            {
                LogMsg( "FraudDetector::ProcessFraudDetectionThread():Exception Is:" + ex.Message );
            }// catch

        }


        private void LogMsg( string msg )
        {
            LogFileMgr.Instance.WriteToLogFile(msg);

        }


        private Hashtable GetDataUsage()
        {
            Hashtable h = _db.GetDataUsage();
            return h;
        }

        /// <summary>
        /// main thread to handle fraud detection
        /// </summary>
        public void StartProcessing()
        {

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
