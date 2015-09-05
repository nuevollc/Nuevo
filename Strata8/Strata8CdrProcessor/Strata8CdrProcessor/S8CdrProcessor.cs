using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using System.Threading;
using System.Diagnostics;

// using CdrDbSvcReference;
using Strata8CdrProcessor.CdrDbSvcReference;


namespace Strata8.CDR.Reporting
{

    public class GroupReport
    {
        public string groupId = String.Empty;
        public string totalOutboundCalls = "0";
        public string totalInboundCalls = "0";
        public string totalCallTime = "0";
        public string averageCallTime = "0";
        public string totalCalls = "0";
        public string groupName = String.Empty;
    }

    public class S8CdrProcessor
    {
        // processing thread
        private Thread m_procThread = null;
        // note that this needs to correlate to the CDR db update interval
        private Int32 m_ReportInterval = 5 * 60 * 1000; // 5 mins
        private string m_eventLogName = "Strata8CallReportSvc"; // default value

        // monthly report time
        // private DateTime m_referenceTime = new DateTime(DateTime.Now.Year, 11, 1, 4, 0, 0);

        private List<string> m_groupList = new List<string>();
        private List<string> m_groupNameList = new List<string>();
        private List<string> m_timeToRunList = new List<string>();
        private string m_smtpserver = null;
        private string m_tab = "\t";

        // default values for now, these are in the config file, see below
        private string m_bccEmailList = "rhernandez@strata8.com";
        private string m_reportEmailList = String.Empty;

        int m_reportStartTime = 0;
        int m_reportEndTime = 0;


        public S8CdrProcessor()
        {
            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogname"];            // get the event log name

            m_reportStartTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportStartTime"]);
            m_reportEndTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportEndTime"]);

            // get some basic config info
            // get the email list 
            m_bccEmailList = System.Configuration.ConfigurationManager.AppSettings["ReportBccMailList"];            // get the email list 

            m_reportEmailList = System.Configuration.ConfigurationManager.AppSettings["ReportMailList"];

            // get the SMTP Server
            m_smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];

            // get the time to sleep between failed file open operations
            m_ReportInterval = 1000 * Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportIntervalInSecs"]);

            // get the times that the processor is running at
            // this loads the m_timeToRunList
            this.GetTimesToRun();

            // load up our userlist
            GetGroups();
        }


        /// <summary>
        /// public method to get the CDR for a user (DID)
        /// For each user (DID), the CDRs are processed to calculate the following report at specified time
        /// intervals (8am, 10am, 1pm, 3pm, and 5pm ) :
        /// 
        /// Total Outbound
        /// Total Inbound
        /// Total Call Duration
        /// Average Call Duration
        /// 
        /// </summary>
        public void CreateStrata8CallReport()
        {
            // our base reference time that we calculate from, 6am every day
            DateTime referenceTime = new DateTime(DateTime.Now.Year, DateTime.Today.Month, DateTime.Today.Day, 4, 0, 0);

            List<GroupReport> userReportList = new List<GroupReport>();

            // get our web service interface

            // CdrDbSvcSoapClient d = new CdrDbSvcSoapClient();
            Strata8CdrProcessor.CdrDbSvcReference.CdrDbSvcSoapClient d = new CdrDbSvcSoapClient();
            
            // get the time that we are running at
            string timeString = DateTime.Now.ToString();

            // calculate the time interval starting at 6am every day
            // we generate a cumulative report starting at 6am every day with reports running at
            // 8am, 11am, 2pm and 5pm.

            // our reference time that we calculate from 
            DateTime timeNow = DateTime.Now;
            TimeSpan timeDelta = timeNow.Subtract(referenceTime);  // time from 4am to now 

            DateTime reportFromTime = DateTime.Now.AddHours(8 - timeDelta.TotalHours);// add time zone to our time: 8 hours
            int indx = 0;

            foreach (string groupId in this.m_groupList )
            {
                // 
                DataSet ds = d.GetAllCdrsForGroup(groupId, reportFromTime);
                // For each User the following calculations are performed :  userId : field 3
                // 
                // Total Inbound Calls : total number of inbound calls determined from the "direction" field 5 (terminating)
                // Total Outbound Calls : total number of outbound calls determined from the direction field 5 (originating)
                // Total Call Time : the total of inbound/outbound call times calculated from each of the CDRs for this user
                //    Call Time : answerTime - releaseTime ( field 13 - field 12 )
                // Average Call Time : Call Time / Total Number of Calls
                // 

                GroupReport r = new GroupReport();
                r.groupId = groupId;
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
                            //if (myTable.Columns[5].Equals("terminating"))
                            totalOut++;
                        else
                            totalIn++;
                        // get the call duration for this call
                        DateTime d1 = (DateTime)myRow.ItemArray[10];
                        DateTime d2 = (DateTime)myRow.ItemArray[14];
                        TimeSpan callDuration = d2.Subtract(d1);

                        // calculate total call duration for this user
                        totalCallDuration = totalCallDuration + callDuration;

                    }
                }
                r.totalInboundCalls = totalIn.ToString();
                r.totalOutboundCalls = totalOut.ToString();
                r.totalCallTime = totalCallDuration.ToString();
                if ((totalIn + totalOut) != 0)
                {
                    double avg = totalCallDuration.TotalMinutes / (totalIn + totalOut);
                    r.averageCallTime = String.Format("{0:##.###}", avg);
                }

                r.groupName = m_groupNameList[indx];
                indx++;

                // store the userReport in the list
                userReportList.Add(r);

            }// get next user

            SendStrata8CallReport(userReportList, timeNow, referenceTime);

        } //CreateStrata8CallReport


        /// <summary>
        /// method to send out a CDR report notification
        /// </summary>
        private void SendStrata8CallReport(List<GroupReport> reportList, DateTime theReportTime, DateTime referenceTime)
        {
            try
            {
                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = this.m_reportEmailList;
                StringBuilder sbb = new StringBuilder("Strata8 Group Call Report From " + referenceTime.ToString("g"));
                sbb.Append(" To : " + theReportTime.ToString("g"));
                string Subject = sbb.ToString();
                string From = "Strata8GroupCallReport@strata8.com";
                MailAddress Bcc = new MailAddress(this.m_bccEmailList);

                // create the Header for the report and the body in the loop
                StringBuilder sb = new StringBuilder();
                StringBuilder body = new StringBuilder();
                sb.Append("UserName" + m_tab + m_tab);
                sb.Append("UserId" + m_tab);
                sb.Append("TotalOubound" + m_tab);
                sb.Append("TotalInbound" + m_tab);
                sb.Append("TotalCallTime" + m_tab);
                sb.Append("AvgCallTime" + m_tab);
                sb.Append("TotalCalls" + "\r\n\r\n");

                foreach (GroupReport userReport in reportList)
                {
                    if (userReport.groupName.Length < 12)
                        body.Append(userReport.groupName + m_tab + m_tab);
                    else
                        body.Append(userReport.groupName + m_tab);
                    body.Append(userReport.groupId + m_tab);
                    body.Append(m_tab + userReport.totalOutboundCalls + m_tab + m_tab);
                    body.Append(userReport.totalInboundCalls + m_tab + m_tab + m_tab);
                    body.Append(userReport.totalCallTime + m_tab + m_tab);
                    body.Append(userReport.averageCallTime + m_tab + m_tab + m_tab);
                    body.Append(userReport.totalCalls + m_tab);
                    body.Append("\r\n");

                }

                // create the total email body structure
                sb.Append(body);

                string temp = "rh@strata8.com";

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage(From, temp, Subject, sb.ToString());
                emsg.Bcc.Add(Bcc);
                sclient.Send(emsg);

            }
            catch (System.Net.Mail.SmtpException sme)
            {// exception handling
                //EventLog.WriteEntry(m_eventLogName, "Strata8CdrProcessor::SendNotification():ECaught:" + sme.ToString(), EventLogEntryType.Error, 3000);
            }

        }


        /// <summary>
        /// method to get and parse the list of user id's to process
        /// </summary>
        private void GetGroups()
        {
            string groupListString = System.Configuration.ConfigurationManager.AppSettings["GroupList"];
            ParseGroupList(groupListString);

            //string userNameListString = System.Configuration.ConfigurationManager.AppSettings["SalesUserNameList"];
            //ParseUserNameList(userNameListString);
        }//GetStrata8SalesUsers

        /// <summary>
        /// method to parse and load the userid's being processes
        /// </summary>
        /// <param name="userList"></param>

        private void ParseGroupList( string groupList )
        {
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = groupList.Trim().Split(delimiter);

            foreach (string s in split)
            {
                // remove the host list and use the hash table
                // in the future for all state processing
                this.m_groupList.Add(s.Trim());

            }
            return;

        }//ParseUserList

        /// <summary>
        /// method to parse and load the userid's being processes
        /// </summary>
        /// <param name="userList"></param>

        private void ParseUserNameList(string groupList)
        {
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = groupList.Trim().Split(delimiter);

            foreach (string s in split)
            {
                // remove the host list and use the hash table
                // in the future for all state processing
                this.m_groupNameList.Add(s.Trim());

            }
            return;

        }//ParseUserList

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
                m_timeToRunList.Add(s.Trim());

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
                        if (DateTime.Now.Hour + 1 > m_reportStartTime & DateTime.Now.Hour < m_reportEndTime + 1)
                        {
                            reportInterval = this.m_ReportInterval;
                            //run the report
                            this.CreateStrata8CallReport();
                        }
                        else
                        {
                            // convert to milliseconds : 60 mins * 60 secs * X hours 
                            reportInterval = 1000 * 3600 * 1;  // check every hour for now was: (24 - m_reportEndTime) + m_reportStartTime;
                        }
                    }//try
                    catch (System.Threading.ThreadAbortException)
                    {
                        EventLog.WriteEntry(m_eventLogName, "Strata8CdrProcessor::ProcessStrata8CdrThread():Fraud Service is stopping", EventLogEntryType.Information, 2001);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        EventLog.WriteEntry(m_eventLogName, "Strata8CdrProcessor::ProcessStrata8CdrThread():ECaught:" + ex.ToString(), EventLogEntryType.Error, 3000);
                    }

                    System.Threading.Thread.Sleep(reportInterval);

                }// while(true)
            }
            catch (System.Threading.ThreadAbortException tax)
            {
                EventLog.WriteEntry(m_eventLogName, "Strata8CdrProcessor::ProcessStrata8CdrThread():ECaught:" + tax.ToString(), EventLogEntryType.Error, 2001);
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "Strata8CdrProcessor::ProcessStrata8CdrThread():Exception Is:" + ex.ToString(), EventLogEntryType.Error, 2001);
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


    }//class

}//namespace
