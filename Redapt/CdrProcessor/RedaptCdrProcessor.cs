using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using System.Threading;
using System.Diagnostics;
using System.Net.Mime;
using System.IO;

// using CdrDbSvcReference;
using CdrProcessor.CdrDbSvcSoap;


namespace Strata8.CDR.Reporting
{

    public class RedaptReport
    {
        public string userId = String.Empty;
        public string totalOutboundCalls = "0";
        public string totalInboundCalls = "0";
        public string totalCallTime = "0";
        public string totalInboundCallTime = "0";
        public string totalOutboundCallTime = "0";
        public string averageCallTime = "0";
        public string totalCalls = "0";
        public string totalCumulativeCalls = "0";
        public string userName = String.Empty;
        public string userExtension = String.Empty;

        public static int CompareByCumulativeCalls(RedaptReport x, RedaptReport y)
        {

            if (x.totalCumulativeCalls == null)
            {
                if (y.totalCumulativeCalls == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y.totalCumulativeCalls == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // lengths of the two strings.
                    //
                    int intx = Convert.ToInt32(x.totalCumulativeCalls);
                    int inty = Convert.ToInt32(y.totalCumulativeCalls);

                    int retval = intx.CompareTo(inty);

                    if (retval != 0)
                    {
                        // If the strings are not of equal,
                        // the longer string is greater.
                        //
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length,
                        // sort them with ordinary string comparison.
                        // done above
                        return retval;
                    }
                }
            }
        }


        public static int CompareByTotalCalls(RedaptReport x, RedaptReport y)
        {

            if (x.totalCalls == null)
            {
                if (y.totalCalls == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y.totalCalls == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // lengths of the two strings.
                    //
                    int intx = Convert.ToInt32(x.totalCalls);
                    int inty = Convert.ToInt32(y.totalCalls);

                    int retval = intx.CompareTo(inty);

                    if (retval != 0)
                    {
                        // If the strings are not of equal,
                        // the longer string is greater.
                        //
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length,
                        // sort them with ordinary string comparison.
                        // done above
                        return retval;
                    }
                }
            }
        }

    
    }// redapt report

    public class TotalsReport
    {
        public int totalOutboundCalls = 0;
        public int totalInboundCalls = 0;
        public TimeSpan totalCallTime = new TimeSpan();
        public TimeSpan totalInboundCallTime = new TimeSpan();
        public TimeSpan totalOutboundCallTime = new TimeSpan();
        public int totalCalls = 0;
        public int totalCumulativeCalls = 0;
    }

    public class RedaptCdrProcessor
    {
        // processing thread
        private Thread m_procThread = null;
        // note that this needs to correlate to the CDR db update interval
        private Int32 m_ReportInterval = 5 * 60 * 1000; // 5 mins
        private string m_eventLogName = "RedaptCallReportSvc"; // default value

        private List<string> m_userList = new List<string>();
        private List<string> m_userNameList = new List<string>();
        private List<string> m_userExtensionList = new List<string>();

        private List<string> m_timeToRunList = new List<string>();
        private string m_smtpserver = null;
        private string m_tab = "\t";

        // default values for now, these are in the config file, see below
        private string m_bccEmailList = "rhernandez@enterprisepcs.com";
        private string m_reportInternalEmailList = String.Empty;
        private string m_reportExternalEmailList = String.Empty;


        // parameters to generate internal/external call reports
        private bool m_generateInternalCallReport = false;
        private bool m_generateExternalCallReport = false;
        
        // parameters for the excel call report
        private bool m_generateExcelCallReport = false;
        private string m_callReportFileName = String.Empty;
        private string m_excelCallReportSubject = String.Empty;
        private string m_excelCallReportToEmailList = String.Empty;

        // cumulative params for the report
        private int m_cumulativeReportDay = 1;
        private int m_cumulativeReportMonth = 1;

        int m_reportStartTime = 0;
        int m_reportEndTime = 0;

        // our db service
        private DbProcessor m_db = null;
        private TimeKeeper m_keeper = null;

        public RedaptCdrProcessor()
        {
            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogname"];            // get the event log name

            m_reportStartTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportStartTime"]); 
            m_reportEndTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportEndTime"]);

            // get some basic config info
            // get the email list 
            m_bccEmailList = System.Configuration.ConfigurationManager.AppSettings["ReportBccMailList"];            // get the email list 

            // get the call report 
            m_generateInternalCallReport = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["GenerateInternalReportTrueFalse"]);
            m_generateExternalCallReport = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["GenerateExternalReportTrueFalse"]);

            // email distribution list
            m_reportExternalEmailList = System.Configuration.ConfigurationManager.AppSettings["ExternalReportMailList"];
            m_reportInternalEmailList = System.Configuration.ConfigurationManager.AppSettings["InternalReportMailList"];

            // get the SMTP Server
            m_smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];

            // get the time to sleep between failed file open operations
            m_ReportInterval = 1000 * Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportIntervalInSecs"]);

            // parameters around the excel report 
            m_generateExcelCallReport = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["GenerateExcelReportTrueFalse"]);
            m_callReportFileName = System.Configuration.ConfigurationManager.AppSettings["ExcelReportFileName"];
            m_excelCallReportSubject = System.Configuration.ConfigurationManager.AppSettings["ExcelSubjectEmail"];
            m_excelCallReportToEmailList = System.Configuration.ConfigurationManager.AppSettings["ExcelToEmailList"];

            // cumulative time params
            m_cumulativeReportMonth = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportCumulativeMonth"]);
            m_cumulativeReportDay= Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings["ReportCumulativeDay"] ); 

            // get the times that the processor is running at
            // this loads the m_timeToRunList
            this.GetTimesToRun();

            // load up our userlist
            GetUserList();

            // get our db interface
            m_db = new DbProcessor();
            m_keeper = new TimeKeeper();

        }

        /// <summary>
        /// public method to create a call report
        /// the default interval starts daily at midnight up until time now
        /// the following parameters are calculated for each user in the list
        /// 
        /// Total Outbound
        /// Total Inbound
        /// Total Call Duration
        /// Average Call Duration
        /// 
        /// </summary>
        public void CreateIntervalCallReport( int reportMonthStart, int reportDayStart, int reportStartHour)
        {        
            // our base reference time that we calculate from midnight every day
            DateTime referenceTime = new DateTime(DateTime.Now.Year, reportMonthStart, reportDayStart, reportStartHour, 0, 0);

            List<RedaptReport> userReportList = new List<RedaptReport>();

            // get our web service interface
          
            //CdrProcessor.CdrDbSvcSoap.CdrDbSvcSoapClient d = new CdrDbSvcSoapClient();        
           
            // get the time that we are running at
            string timeString = DateTime.Now.ToString();

            // initiate our group report object with running totals
            TotalsReport gr = new TotalsReport();
            // calculate the time interval starting at 6am every day
            // we generate a cumulative report starting at 6am every day with reports running at
            // 8am, 11am, 2pm and 5pm.

            // our reference time that we calculate from 
            DateTime timeNow = DateTime.Now;
            TimeSpan timeDelta = timeNow.Subtract( referenceTime );  // time from midnight to now 
            
            DateTime reportFromTime = DateTime.Now.AddHours( 8 - timeDelta.TotalHours );// add time zone to our time: 8 hours
            int indx = 0;

            foreach (string userId in m_userList)
            {
                // ***
                // *** Redapt Report uses this API
                DataSet ds = m_db.GetCdrsForPhoneNumber(userId, referenceTime); //reportFromTime);
                //DataSet ds = m_soapClient.GetAllCdrsForPhoneNumber( userId, reportFromTime);

                // For each User the following calculations are performed :  userId : field 3
                // 
                // Total Inbound Calls : total number of inbound calls determined from the "direction" field 5 (terminating)
                // Total Outbound Calls : total number of outbound calls determined from the direction field 5 (originating)
                // Total Call Time : the total of inbound/outbound call times calculated from each of the CDRs for this user
                //    Call Time : answerTime - releaseTime ( field 13 - field 12 )
                // Average Call Time : Call Time / Total Number of Calls
                // 

                RedaptReport r = new RedaptReport();
                r.userId = userId;
                int totalIn = 0;
                int totalOut = 0;
                TimeSpan totalCallDuration = new TimeSpan();
               
                // for each CDR we update our report values
                foreach (DataTable myTable in ds.Tables)
                {
                    r.totalCalls = myTable.Rows.Count.ToString();

                    foreach (DataRow myRow in myTable.Rows)
                    {
                        //foreach (DataColumn myColumn in myTable.Columns)
                        //{
                        //    Console.WriteLine(myRow[myColumn]);
                        //}
                        if (myRow.ItemArray[6].Equals("Originating"))
                            totalOut++;
                        else if (myRow.ItemArray[6].Equals("Terminating"))
                            totalIn++;
                        // get the call duration for this call
                        DateTime d1 = (DateTime) myRow.ItemArray[10];
                        DateTime d2 = (DateTime)myRow.ItemArray[14];
                        TimeSpan callDuration = d2.Subtract(d1);

                        // calculate total call duration for this user
                        totalCallDuration = totalCallDuration + callDuration;

                    }
                }
                r.totalInboundCalls = totalIn.ToString();
                r.totalOutboundCalls = totalOut.ToString();
                r.totalCallTime = totalCallDuration.ToString();
                if ( (totalIn+totalOut) != 0 )
                {
                    double avg = totalCallDuration.TotalMinutes / (totalIn + totalOut);
                    r.averageCallTime = String.Format("{0:##.###}", avg);
                }

                r.userName = m_userNameList[indx];
                r.userExtension = m_userExtensionList[indx];
                indx++;

                // store the userReport in the list
                userReportList.Add(r);

                // maintain the group totals here
                gr.totalCalls = gr.totalCalls + Convert.ToInt32(r.totalCalls);
                gr.totalCallTime = gr.totalCallTime + totalCallDuration;
                gr.totalInboundCalls = gr.totalInboundCalls + Convert.ToInt32(r.totalInboundCalls);
                gr.totalOutboundCalls = gr.totalOutboundCalls + Convert.ToInt32(r.totalOutboundCalls);    
        
            }// get next user

            // get the cumulative report from the configured date
            List<RedaptReport> cumulativeReportList = GetCumulativeCallData(ref userReportList, ref gr);

            if (m_generateInternalCallReport)
                // generate the HTML notification call report format
                SendInternalCallReportNotificationHtml(userReportList, gr, timeNow, referenceTime, cumulativeReportList);

            if (m_generateExternalCallReport)
                // generate the HTML notification call report format, this report is intended for 
                // outside of the Engineering group/company
                SendExternalCallReportNotificationHtml(userReportList, gr, timeNow, referenceTime, cumulativeReportList);

            // generate the Excel call report
            if ( m_generateExcelCallReport )
                GenerateExcelCallReport(userReportList, gr, timeNow, referenceTime, cumulativeReportList );

            // **
            // SendCallReportNotificationHtml(userReportList, endTime , referenceTime);
        
        } //CreateRedaptCallReport

        /// <summary>
        /// method to get CDR data
        /// </summary>
        /// <param name="x"></param>
        private List<RedaptReport> GetCumulativeCallData(ref List<RedaptReport> rUserList, ref TotalsReport totals)
        {
            // our base reference time that we calculate from, 4am on the configured day
            DateTime referenceTime = new DateTime( DateTime.Now.Year, m_cumulativeReportMonth, m_cumulativeReportDay );

            // CdrDbSvcSoapClient d = new CdrDbSvcSoapClient();
            // CdrProcessor.CdrDbSvcSoap.CdrDbSvcSoapClient d = new CdrDbSvcSoapClient();

            // get the time that we are running at
            string timeString = DateTime.Now.ToString();

            List<RedaptReport> cumulativeReportList = new List<RedaptReport>();

            // initiate our group report object with running totals
            TotalsReport gr = new TotalsReport();

            foreach (RedaptReport u in rUserList)
            {
                // ***
                // *** Redapt Report uses this API
                // DataSet ds = d.GetBillableCdrsForPhoneNumber(userId, reportFromTime);
                DataSet ds = m_db.GetCdrsForPhoneNumber(u.userId, referenceTime);

                // For each User the following calculations are performed :  userId : field 3
                // 
                // Total Inbound Calls : total number of inbound calls determined from the "direction" field 5 (terminating)
                // Total Outbound Calls : total number of outbound calls determined from the direction field 5 (originating)
                // Total Call Time : the total of inbound/outbound call times calculated from each of the CDRs for this user
                //    Call Time : answerTime - releaseTime ( field 13 - field 12 )
                // Average Call Time : Call Time / Total Number of Calls
                // 

                RedaptReport cumulativeUserReport = new RedaptReport();
                cumulativeUserReport.userId = u.userId;
                int totalIn = 0;
                int totalOut = 0;
                TimeSpan totalCallDuration = new TimeSpan();

                // for each CDR we update our report values
                foreach (DataTable myTable in ds.Tables)
                {

                    foreach (DataRow myRow in myTable.Rows)
                    {
                        //foreach (DataColumn myColumn in myTable.Columns)
                        //{
                        //    Console.WriteLine(myRow[myColumn]);
                        //}
                        if (myRow.ItemArray[6].Equals("Originating"))
                            //if (myTable.Columns[5].Equals("terminating"))
                            totalOut++;
                        else if (myRow.ItemArray[6].Equals("Terminating"))
                            totalIn++;
                        // get the call duration for this call
                        DateTime d1 = (DateTime)myRow.ItemArray[10];
                        DateTime d2 = (DateTime)myRow.ItemArray[14];
                        TimeSpan callDuration = d2.Subtract(d1);

                        // calculate total call duration for this user
                        totalCallDuration = totalCallDuration + callDuration;

                    }
                }
                cumulativeUserReport.totalInboundCalls = totalIn.ToString();
                cumulativeUserReport.totalOutboundCalls = totalOut.ToString();
                cumulativeUserReport.totalCallTime = totalCallDuration.ToString();

                if ((totalIn + totalOut) != 0)
                {
                    double avg = totalCallDuration.TotalMinutes / (totalIn + totalOut);
                    cumulativeUserReport.averageCallTime = String.Format("{0:##.###}", avg);
                }

                cumulativeUserReport.userName = u.userName;

                cumulativeUserReport.totalCalls = (totalIn + totalOut).ToString();
                u.totalCumulativeCalls = (totalIn + totalOut).ToString();

                // keep a total of all calls
                totals.totalCumulativeCalls  = totals.totalCumulativeCalls + Convert.ToInt32(u.totalCumulativeCalls); 

                // store the userReport in the list
                cumulativeReportList.Add( cumulativeUserReport );

            }// get next user

            return cumulativeReportList;

        }// get cumulative call data

        /// <summary>
        /// method to generate the excel call report format
        /// </summary>
        /// <param name="reportList"></param>
        /// <param name="theReportTime"></param>
        /// <param name="referenceTime"></param>
        private void GenerateExcelCallReport(List<RedaptReport> reportList, TotalsReport tr,
            DateTime theReportTime, DateTime referenceTime, List<RedaptReport> cumulativeReport)
        {
            string comma = ",";
            DateTime date = DateTime.Now;
            string rFileName = m_callReportFileName + date.Year.ToString() + date.Month.ToString()
                + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() +
                ".csv";


            StringBuilder sb = new StringBuilder("UserName, UserId, TotalCallsOut, TotalCallsIn, TotalCallTime, AvgCallTime, TotalCalls, TotalCumulativeCalls" + "\r\n");
           
            foreach (RedaptReport userReport in reportList)
            {
                sb.Append( userReport.userName + comma );
                sb.Append( userReport.userId + comma );
                sb.Append( userReport.totalOutboundCalls  + comma );
                sb.Append( userReport.totalInboundCalls  + comma );
                sb.Append( userReport.totalCallTime  + comma + userReport.averageCallTime + comma );
                sb.Append( userReport.totalCalls + comma + userReport.totalCumulativeCalls + "\r\n" ); // add the new line
            }

            this.WriteToFile(rFileName, sb.ToString());

            // send the file in an email
            string To = this.m_reportInternalEmailList;
            string From = "CallReport@enterprisepcs.com";
            MailAddress Bcc = new MailAddress(this.m_bccEmailList);
            // Create a message and set up the recipients.
            MailMessage message = new MailMessage(
               From,
               m_excelCallReportToEmailList,
               m_excelCallReportSubject,
               "See the attached daily call report spreadsheet.");

            // Create  the file attachment for this e-mail message.
            Attachment data = new Attachment(rFileName, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.not needed hombre
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(rFileName);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(rFileName);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(rFileName);
            // Add the file attachment to this e-mail message.
            message.Attachments.Add(data);

            //Send the message.
            SmtpClient client = new SmtpClient( this.m_smtpserver );
            // Add credentials if the SMTP server requires them.
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch ( System.Net.Mail.SmtpException sme )
            {
                EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::SendNotification():ECaught:" + sme.Message, EventLogEntryType.Error, 3000);

            }
            // Display 

        }

        /// 
        /// public method to create a call report based on cnfigurable parameters
        /// 
        /// </summary>
        public void CreateCallReport()
        {
            // our base reference time that we calculate from, 6am every day
            // DateTime referenceTime = new DateTime(DateTime.Now.Year, DateTime.Today.Month, DateTime.Today.Day, 4, 0, 0);

            DateTime referenceTime = new DateTime(2009, 12, 1, 0, 0, 0);

            // *** change these two to add a date range
            // DateTime referenceTime = new DateTime(DateTime.Now.Year, 11, 1, 0, 0, 0);
            DateTime endTime = new DateTime(2009, 12, 9);

            List<RedaptReport> userReportList = new List<RedaptReport>();

            // get our web service interface

            CdrDbSvcSoapClient d = new CdrDbSvcSoapClient();

            // get the time that we are running at
            string timeString = DateTime.Now.ToString();

            // initiate our group report object with running totals
            TotalsReport gr = new TotalsReport();
            // calculate the time interval starting at 6am every day
            // we generate a cumulative report starting at 6am every day with reports running at
            // 8am, 11am, 2pm and 5pm.

            // our reference time that we calculate from 
            DateTime timeNow = DateTime.Now;
            TimeSpan timeDelta = new TimeSpan(8, 0, 0);  // time from 4am to now 

            DateTime reportFromTime = referenceTime.Add(timeDelta);// add time zone to our time: 8 hours
            int indx = 0;

            foreach (string userId in m_userList)
            {
                // ***
                // DataSet ds = d.GetBillableCdrsForPhoneNumber(userId, reportFromTime);
                DataSet ds = d.GetDateRangeCdrsForPhoneNumber(userId, reportFromTime, endTime);

                // For each User the following calculations are performed :  userId : field 3
                // 
                // Total Inbound Calls : total number of inbound calls determined from the "direction" field 5 (terminating)
                // Total Outbound Calls : total number of outbound calls determined from the direction field 5 (originating)
                // Total Call Time : the total of inbound/outbound call times calculated from each of the CDRs for this user
                //    Call Time : answerTime - releaseTime ( field 13 - field 12 )
                // Average Call Time : Call Time / Total Number of Calls
                // 

                RedaptReport r = new RedaptReport();
                r.userId = userId;
                int totalIn = 0;
                int totalOut = 0;
                TimeSpan totalCallDuration = new TimeSpan();
                TimeSpan totalInboundCallDuration = new TimeSpan();
                TimeSpan totalOutboundCallDuration = new TimeSpan();

                // for each CDR we update our report values
                foreach (DataTable myTable in ds.Tables)
                {
                    r.totalCalls = myTable.Rows.Count.ToString();

                    foreach (DataRow myRow in myTable.Rows)
                    {
                        // get the call duration for this call
                        DateTime d1 = (DateTime)myRow.ItemArray[10];
                        DateTime d2 = (DateTime)myRow.ItemArray[14];
                        TimeSpan inboundCallDuration;
                        TimeSpan outboundCallDuration;

                        if (myRow.ItemArray[6].Equals("Originating"))
                        {
                            totalOut++;
                            // get the call duration for this call
                            outboundCallDuration = d2.Subtract(d1);
                            totalOutboundCallDuration = totalOutboundCallDuration + outboundCallDuration;
                        }
                        else
                        {
                            totalIn++;
                            // get the call duration for this call
                            inboundCallDuration = d2.Subtract(d1);
                            totalInboundCallDuration = totalInboundCallDuration + inboundCallDuration;
                        }

                    }
                }
                r.totalInboundCalls = totalIn.ToString();
                r.totalOutboundCalls = totalOut.ToString();
                r.totalCallTime = totalCallDuration.ToString();
                r.totalInboundCallTime = totalInboundCallDuration.ToString();
                r.totalOutboundCallTime = totalOutboundCallDuration.ToString();

                if ((totalIn + totalOut) != 0)
                {
                    double avg = totalCallDuration.TotalMinutes / (totalIn + totalOut);
                    r.averageCallTime = String.Format("{0:##.###}", avg);
                }

                r.userName = m_userNameList[indx];
                indx++;

                // store the userReport in the list
                userReportList.Add(r);

                // maintain the group totals here
                gr.totalCalls = gr.totalCalls + Convert.ToInt32(r.totalCalls);
                gr.totalCallTime = gr.totalCallTime + totalCallDuration;
                gr.totalInboundCalls = gr.totalInboundCalls + Convert.ToInt32(r.totalInboundCalls);
                gr.totalInboundCallTime = gr.totalInboundCallTime + totalInboundCallDuration;
                gr.totalOutboundCallTime = gr.totalOutboundCallTime + totalOutboundCallDuration;
                gr.totalOutboundCalls = gr.totalOutboundCalls + Convert.ToInt32(r.totalOutboundCalls);

            }// get next user

            SendTotalCallReportNotificationHtml(userReportList, gr, reportFromTime, endTime);

        } //CreateCallReport

         /// <summary>
        /// method to send out a CDR report notification
        /// </summary>
        private void SendRedaptCallReport(List<RedaptReport> reportList, DateTime theReportTime, DateTime referenceTime )
        {
            try
            {
                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = this.m_reportInternalEmailList;                   
                StringBuilder sbb = new StringBuilder("Call Report From " + referenceTime.ToString("g") );
                sbb.Append( " To : " + theReportTime.ToString("g") );
                string Subject = sbb.ToString();
                string From = "CallReport@enterprisepcs.com";
                MailAddress Bcc = new MailAddress( this.m_bccEmailList );
               
                // create the Header for the report and the body in the loop
                StringBuilder sb = new StringBuilder();
                StringBuilder body = new StringBuilder();
                sb.Append("UserName" + m_tab + m_tab);
                sb.Append("UserId" + m_tab);
                sb.Append("TotalOubound" + m_tab);
                sb.Append("TotalInbound" + m_tab );
                sb.Append("TotalCallTime" + m_tab );
                sb.Append("AvgCallTime" + m_tab );
                sb.Append("TotalCalls"+ "\r\n\r\n");

                foreach (RedaptReport userReport in reportList)
                {
                    if ( userReport.userName.Length < 12 )
                        body.Append(userReport.userName + m_tab + m_tab);
                    else
                        body.Append(userReport.userName + m_tab);
                    body.Append(userReport.userId + m_tab);
                    body.Append(m_tab + userReport.totalOutboundCalls + m_tab + m_tab);
                    body.Append(userReport.totalInboundCalls + m_tab + m_tab + m_tab);
                    body.Append(userReport.totalCallTime + m_tab + m_tab);
                    body.Append(userReport.averageCallTime + m_tab + m_tab + m_tab);
                    body.Append(userReport.totalCalls + m_tab);
                    body.Append("\r\n");

                }

                // create the total email body structure
                sb.Append(body);

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage( From, To, Subject, sb.ToString() );
                emsg.Bcc.Add(Bcc);
                sclient.Send(emsg);

            }
            catch (System.Net.Mail.SmtpException sme)
            {// exception handling
                //EventLog.WriteEntry(m_eventLogName, "RedaptCdrProcessor::SendNotification():ECaught:" + sme.ToString(), EventLogEntryType.Error, 3000);
            }

        }
 
        /// <summary>
        /// method to send out a CDR report notification via HTML
        /// this method includes some private parameters not included in the External email notification method
        /// </summary>
        private void SendInternalCallReportNotificationHtml(List<RedaptReport> reportList, TotalsReport reportTotals,
            DateTime theReportTime, DateTime referenceTime, List<RedaptReport> cumulativeReport )
        {
            try
            {
                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = this.m_reportInternalEmailList;
                StringBuilder sbb = new StringBuilder("Call Report From " + referenceTime.ToString("g"));
                sbb.Append(" To : " + theReportTime.ToString("g"));
                string Subject = sbb.ToString();
                string From = "CallReport@enterprisepcs.com";
                MailAddress Bcc = new MailAddress(this.m_bccEmailList);

                StringBuilder bod = new StringBuilder(@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                bod.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
                bod.Append(@"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"" /></head>");
                bod.Append(@"<body>");
                bod.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""1"">");

                bod.Append(@"<tr bgcolor=""EEEEEE"">");;
                bod.Append(@"<td colspan=""8"" align=""center""><img src=""http://www.nuevocommunications.com/images/dmImage.jpg"" alt=""David Maus Toyota"" border=""0"" /></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<td colspan=""8"" align=""center""><font face=""verdana"" size=""2""><strong>Call Statistics</strong></font></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" colspan=""8"" align=""center""><font face=""verdana"" size=""1"" color=""Blue"">" + sbb.ToString() + "</font>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Name</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User ID</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Ext.</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Outbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Inbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Avg <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Calls</b></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr>");
                bod.Append(@"<td colspan=""8""><hr></td>");
                bod.Append(@"</tr> ");

                // sort by totaloutbound calls, if only doing internal, then it is a daily report, not cumulative
                if (m_generateExternalCallReport)
                {
                    reportList.Sort(RedaptReport.CompareByCumulativeCalls);
                    reportList.Reverse();
                }
                else
                {
                    reportList.Sort(RedaptReport.CompareByTotalCalls);
                    reportList.Reverse();
                }

                foreach (RedaptReport userReport in reportList)
                {
                    bod.Append(@"<tr> ");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userName + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userId + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userExtension + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalOutboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalInboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.averageCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalCalls + "</font></td>");
                    bod.Append(@"</tr>");

                }
                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Totals</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalOutboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalInboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCallTime.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCalls.ToString() + "</strong></font></td>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCumulativeCalls.ToString() + "</strong></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<td colspan=""8"" align=""center""><font face=""verdana"" size=""1"" ");
                bod.Append(@"<br>");
                bod.Append(@"<font color=""Red""><b>NOTE : </b></font>");
                bod.Append(@"These numbers reflect traffic on the EnterprisePCS Network only.</font></td>");
                bod.Append(@"<br>");
                bod.Append(@"</table>");

                // end of BODY and our HTML format 
                bod.Append(@"</body></html>");

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage(From, To, Subject, bod.ToString()); //sb.ToString());

                ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                // Add the alternate body to the message.

                AlternateView alternate = AlternateView.CreateAlternateViewFromString(bod.ToString(), mimeType);
                emsg.AlternateViews.Add(alternate);
                emsg.Bcc.Add(Bcc);
                sclient.Send(emsg);

            }
            catch (System.Net.Mail.SmtpException sme)
            {// exception handling
                EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::SendNotification():ECaught:" + sme.ToString(), EventLogEntryType.Error, 3000);
            }

        }

        /// <summary>
        /// method to send out a CDR report notification via HTML
        /// this method includes some private parameters not included in the External email notification method
        /// </summary>
        private void SendExternalCallReportNotificationHtml(List<RedaptReport> reportList, TotalsReport reportTotals,
            DateTime theReportTime, DateTime referenceTime, List<RedaptReport> cumulativeReport)
        {
            try
            {
                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = this.m_reportExternalEmailList;
                StringBuilder sbb = new StringBuilder("Call Report From " + referenceTime.ToString("g"));
                sbb.Append(" To : " + theReportTime.ToString("g"));
                string Subject = sbb.ToString();
                string From = "CallReport@enterprisepcs.com";
                MailAddress Bcc = new MailAddress(this.m_bccEmailList);

                StringBuilder bod = new StringBuilder(@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                bod.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
                bod.Append(@"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"" /></head>");
                bod.Append(@"<body>");
                bod.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""1"">");

                bod.Append(@"<tr bgcolor=""EEEEEE"">"); ;
                bod.Append(@"<td colspan=""7"" align=""center""><img src=""http://www.nuevocommunications.com/images/dmImage.jpg"" alt=""David Maus Toyota"" border=""0"" /></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<td colspan=""7"" align=""center""><font face=""verdana"" size=""2""><strong>Call Statistics</strong></font></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" colspan=""7"" align=""center""><font face=""verdana"" size=""1"" color=""Blue"">" + sbb.ToString() + "</font>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Name</b></font></td>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User ID</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Ext.</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Outbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Inbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Avg <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Calls</b></font></td>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Cumulative Calls</b></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr>");
                bod.Append(@"<td colspan=""7""><hr></td>");
                bod.Append(@"</tr> ");

                // sort by totaloutbound calls
                reportList.Sort(RedaptReport.CompareByCumulativeCalls);
                reportList.Reverse();

                foreach (RedaptReport userReport in reportList)
                {
                    bod.Append(@"<tr> ");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userName + "</font></td>");
//                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userId + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userExtension + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalOutboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalInboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.averageCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalCalls + "</font></td>");
                   // bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalCumulativeCalls + "</font></td>");
                    bod.Append(@"</tr>");

                }
                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Totals</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalOutboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalInboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCallTime.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCalls.ToString() + "</strong></font></td>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCumulativeCalls.ToString() + "</strong></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<td colspan=""7"" align=""center""><font face=""verdana"" size=""1"" ");
                bod.Append(@"<br>");
                bod.Append(@"<font color=""Red""><b>NOTE : </b></font>");
                bod.Append(@"These numbers reflect traffic on the EnterprisePCS Network only.</font></td>");
                bod.Append(@"<br>");
                bod.Append(@"</table>");

                // end of BODY and our HTML format 
                bod.Append(@"</body></html>");

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage(From, To, Subject, bod.ToString()); //sb.ToString());

                ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                // Add the alternate body to the message.

                AlternateView alternate = AlternateView.CreateAlternateViewFromString(bod.ToString(), mimeType);
                emsg.AlternateViews.Add(alternate);
                emsg.Bcc.Add(Bcc);
                sclient.Send(emsg);

            }
            catch (System.Net.Mail.SmtpException sme)
            {// exception handling
                EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::SendNotification():ECaught:" + sme.ToString(), EventLogEntryType.Error, 3000);
            }

        }

        /// <summary>
        /// method to send out a CDR report notification
        /// </summary>
        private void SendTotalCallReportNotificationHtml(List<RedaptReport> reportList, TotalsReport reportTotals, DateTime startTime, DateTime endTime)
        {
            try
            {
                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = this.m_reportInternalEmailList;
                StringBuilder sbb = new StringBuilder("Call Report From " + startTime.ToString("g"));
                sbb.Append(" To : " + endTime.ToString("g"));
                string Subject = sbb.ToString();
                string From = "CallReport@enterprisepcs.com";
                MailAddress Bcc = new MailAddress(this.m_bccEmailList);

                StringBuilder bod = new StringBuilder(@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                bod.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
                bod.Append(@"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"" /></head>");
                bod.Append(@"<body>");
                bod.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""1"">");

                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<br>");
                bod.Append(@"<td colspan=""9"" align=""center""><font face=""verdana"" size=""2""><strong>Call Statistics</strong></font></td>");
                bod.Append(@"<br>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" colspan=""7"" align=""center""><font face=""verdana"" size=""1"" color=""Blue"">" + sbb.ToString() + "</font>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Name</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>UserId</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Outbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Inbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Inbound Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Outbound Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Avg Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Calls</b></font></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td colspan=""9""><hr></td>");
                bod.Append(@"</tr> ");


                foreach (RedaptReport userReport in reportList)
                {
                    bod.Append(@"<tr> ");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userName + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.userId + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalOutboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalInboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalInboundCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalOutboundCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.averageCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.totalCalls + "</font></td>");
                    bod.Append(@"</tr>");

                }
                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Totals</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalOutboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalInboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalInboundCallTime.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalOutboundCallTime.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCallTime.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCalls.ToString() + "</strong></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<td colspan=""9"" align=""center""><font face=""verdana"" size=""1"" ");
                bod.Append(@"<br>");
                bod.Append(@"<font color=""Red""><b>NOTE : </b></font>");
                bod.Append(@"These numbers reflect traffic on the EnterprisePCS Network only.</font></td>");
                bod.Append(@"<br>");
                bod.Append(@"</table>");

                // end of BODY and our HTML format 
                bod.Append(@"</body></html>");

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage(From, To, Subject, bod.ToString()); //sb.ToString());

                ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                // Add the alternate body to the message.

                AlternateView alternate = AlternateView.CreateAlternateViewFromString(bod.ToString(), mimeType);
                emsg.AlternateViews.Add(alternate);
                emsg.Bcc.Add(Bcc);
                sclient.Send(emsg);

            }
            catch (System.Net.Mail.SmtpException sme)
            {// exception handling
                EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::SendNotification():ECaught:" + sme.ToString(), EventLogEntryType.Error, 3000);
            }

        } 
        
        /// <summary>
        /// method to get and parse the list of user id's to process
        /// </summary>
        private void GetUserList()
        {
            // get the list of userIds
            string userListString = System.Configuration.ConfigurationManager.AppSettings["SalesUserList"];
            this.m_userList = ParseList(userListString);

            // get the name of each user
            string userNameListString = System.Configuration.ConfigurationManager.AppSettings["SalesUserNameList"];
            this.m_userNameList = ParseList(userNameListString);

            // get the extension of each user
            string userExtensionListString = System.Configuration.ConfigurationManager.AppSettings["SalesUserExtensionList"];
            this.m_userExtensionList = ParseList(userExtensionListString);

        }//GetUserList

        /// <summary>
        /// method to parse and load the users being processes
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
                theList.Add( s.Trim() );

            }
            return theList;

        }//ParseList

        /// <summary>
        /// method to get and parse the list times to run the report
        /// </summary>
        private void GetTimesToRun()
        {
            string timesToRunString = System.Configuration.ConfigurationManager.AppSettings["ReportTimesToRun"];
            ParseTimeList(timesToRunString);
        }

        /// <summary>
        /// method to parse and load the userid's being processes
        /// </summary>
        /// <param name="userList"></param>
        private void ParseTimeList(string userList)
        {
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = userList.Trim().Split(delimiter);

            foreach (string s in split)
            {
                // remove the host list and use the hash table
                // in the future for all state processing
                m_timeToRunList.Add( s.Trim() );

            }
            return;

        }//ParseTimeList

        /// <summary>
        /// 
        /// </summary>
        private void ProcessCdrThread()
        {
            int reportInterval = 0;

            try
            {
                while (true)
                {
                    try
                    {
                    // report will run every reportInterval time when within the window
                    // think about making this run every reportTime interval using a timer or ...
                    // if we are in the report interval, our time is the config interval time
                        if (DateTime.Now.Hour +1 > m_reportStartTime & DateTime.Now.Hour < m_reportEndTime + 1 )
                        {
                            reportInterval = this.m_ReportInterval;
                            //run only the internal report for our faster interval
                            this.m_generateExternalCallReport = false;
                            this.CreateIntervalCallReport( DateTime.Now.Month, DateTime.Now.Day, 0);
                        }
                        else
                        {
                            // convert to milliseconds : 60 mins * 60 secs * X hours 
                            reportInterval = 1000 * 3600 * 1;  // check every hour for now was: (24 - m_reportEndTime) + m_reportStartTime;
                        }
                        // check if our 24 hour interval has passed
                        if ( m_keeper.DayPassed() )
                        {
                            // generate from our start date
                            this.m_generateExternalCallReport = true;
                            this.CreateIntervalCallReport( m_cumulativeReportMonth, m_cumulativeReportDay, 0 );                         
                        }
                        
                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        EventLog.WriteEntry(m_eventLogName, "RedaptCdrProcessor::ProcessRedaptCdrThread():Fraud Service is stopping", EventLogEntryType.Information, 2001);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        EventLog.WriteEntry(m_eventLogName, "RedaptCdrProcessor::ProcessRedaptCdrThread():ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);
                    }

                    System.Threading.Thread.Sleep(reportInterval);

                }// while(true)
            }
            catch (System.Threading.ThreadAbortException tax)
            {
                EventLog.WriteEntry(m_eventLogName, "RedaptCdrProcessor::ProcessRedaptCdrThread():ECaught:" + tax.ToString(), EventLogEntryType.Error, 2001);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "RedaptCdrProcessor::ProcessRedaptCdrThread():Exception Is:" + ex.ToString(), EventLogEntryType.Error, 2001);
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
            if (!EventLog.SourceExists(m_eventLogName))
            {
                EventLog.CreateEventSource(m_eventLogName, "Application");
            }

            // launch the job control processing thread monitoring the queue
            m_procThread = new Thread(new ThreadStart( ProcessCdrThread ));
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
 
        /// private method used to write to the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="msg"></param>
        private void WriteToFile( string fileName, string msg )
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if (!File.Exists(fileName))
            {
                try
                {
                    fs = File.Create(fileName);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::WriteToFile():ECaught:" + ex.Message, EventLogEntryType.Error, 3000);
    
                    return;
                }

            }// created new file and file stream
            else
            {
                // we just append to the file
                try
                {
                    fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::WriteToFile():ECaught:" + ex.Message, EventLogEntryType.Error, 3000);
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
                EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::WriteToFile():ECaught:" + ex.Message, EventLogEntryType.Error, 3000);
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
                EventLog.WriteEntry(m_eventLogName, "ReportCdrProcessor::WriteToFile():ECaught:" + ex.Message, EventLogEntryType.Error, 3000);
                return;
            }

        }// private void WriteToFile(string msg)

    }//class
 
}//namespace
