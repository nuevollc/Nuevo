
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Diagnostics;
using System.Net.Mime;
using System.IO;

using Strata8.Wireless;
using Strata8.Wireless.Cdr;
using Strata8.Wireless.Db;
using Strata8.Wireless.Data;
using Strata8.Wireless.Utils;

namespace Strata8.Wireless.Cdr.Reporting
{

    public class WCallReport
    {
        public string seqNumber = String.Empty;
        public string siteName = String.Empty;
        public string recordType = String.Empty;
        public string disconnectMessage = String.Empty;
        public string disconnectCode = String.Empty;
        public string aPartyNumber = String.Empty;
        public string bPartyNumber = String.Empty;
        public string aPartyType = String.Empty;
        public string bPartyType = String.Empty;
        public string aPartyDigits = String.Empty;
        public string bPartyDigits = String.Empty;
        public DateTime seize;
        public DateTime answer;
        public DateTime disconnect;
        public TimeSpan duration;

    }

    public class WTotalsReport
    {
        public int totalMoCallAttempts = 0;
        public int totalMoCalls = 0;
        public int totalMtCallAttempts = 0;
        public int totalMtCalls = 0;

        public int totalMtSmsAttempts = 0;
        public int totalMtSms = 0;
        public int totalMoSmsAttempts = 0;
        public int totalMoSms = 0;
        public TimeSpan totalMoTime = new TimeSpan();
        public TimeSpan totalMtTime = new TimeSpan();
        public TimeSpan totalCallTime = new TimeSpan();
        public int totalCallAttempts = 0;
        public int totalSmsAttempts = 0;
        public int totalSms = 0;
        public int totalCalls = 0;
    }   

    public class WirelessCdrProcessor
    {
        // processing thread
        private Thread m_procThread = null;
        // note that this needs to correlate to the CDR db update interval
        private Int32 m_ReportInterval = 5 * 60 * 1000; // 5 mins
        private string m_eventLogName = "OWReportSvc"; // default value

        private Db.CiberDbMgr m_cbrDbMgr = new Db.CiberDbMgr();
        private TechDataSheetProcessor m_tds = new TechDataSheetProcessor();

        private List<string> m_groupList = new List<string>();
        private List<string> m_groupNameList = new List<string>();
        private List<string> m_timeToRunList = new List<string>();
        private string m_smtpserver = null;

        // default values for now, these are in the config file, see below
        private string m_bccEmailList = "rhernandez@strata8.com";
        private string m_reportEmailList = String.Empty;
        private string m_dailyReportEmailList = String.Empty;
        private string m_csvReportEmailList = String.Empty;
        private string m_reportOutputDirectory = String.Empty;
        private static string m_LogFile = String.Empty;

        // ftp server params
        private string m_remoteFTPServer = String.Empty;
        private string m_remoteFTPServerUsername = String.Empty;
        private string m_remoteFTPServerPassword = String.Empty;

        private int m_dailyRunTime = 0;

        private int m_reportStartTime = 0;
        private int m_reportEndTime = 0;

        private int m_reportStartMonth = 0;
        private int m_reportStartDay = 0;
        private bool m_useStartMonthDay = false;


        public WirelessCdrProcessor()
        {
            // get the output directory for the CDR report for onwaves
            m_reportOutputDirectory = System.Configuration.ConfigurationManager.AppSettings["ReportOutputDirectory"];

            m_reportStartTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportStartTime"]);
            m_reportEndTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportEndTime"]);

            m_reportStartMonth = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportStartMonth"]);
            m_reportStartDay = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportStartDay"]);
            m_useStartMonthDay = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseStartMonthDay"]);

            // get some basic config info
            // get the email list 
            m_bccEmailList = System.Configuration.ConfigurationManager.AppSettings["ReportBccMailList"];            // get the email list 

            // 24 hour report
            m_dailyReportEmailList = System.Configuration.ConfigurationManager.AppSettings["DailyReportMailList"];

            // cumulative report throughtout the day
            m_reportEmailList = System.Configuration.ConfigurationManager.AppSettings["ReportMailList"];

            // csv report email list - 24 hour report
            m_csvReportEmailList = System.Configuration.ConfigurationManager.AppSettings["CSVReportMailList"];

            // get the SMTP Server
            m_smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];            // get the SMTP Server
            m_LogFile = System.Configuration.ConfigurationManager.AppSettings["LogFileName"];

            // get the time to sleep between failed file open operations
            m_ReportInterval = 1000 * Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportIntervalInSecs"]);
            
            m_dailyRunTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportTimeToRunAtIn24HourTimeReference"]);

            // get the FTP Server to upload CSV report to daily or at the m_dailyRunTime
            m_remoteFTPServer = System.Configuration.ConfigurationManager.AppSettings["RemoteFTPServer"]; 
            m_remoteFTPServerUsername = System.Configuration.ConfigurationManager.AppSettings["RemoteFTPServerUsername"];
            m_remoteFTPServerPassword = System.Configuration.ConfigurationManager.AppSettings["RemoteFTPServerPassword"];

            try
            {
                // set up event logging
                if (!EventLog.SourceExists(m_eventLogName))
                {
                    // EventLog.CreateEventSource(m_eventLogName, "Application");
                }
            }
            catch( System.Security.SecurityException se)
            {
                WirelessCdrProcessor.WriteToLogFile( "WirelessCdrProcessor::Ctor():UnableToCreateEventLogFile");
            }
            // get the times that the processor is running at
            // this loads the m_timeToRunList
            this.GetTimesToRun();

        }

        /// <summary>
        /// public method to generate a call report from 00 hours to midnight 
        /// 
        /// </summary>
        public List<OmcCdr> CreateDailyCallReport(DateTime reportFromTime, DateTime reportToTime, string emailList )
        {
            // our base reference time that we calculate from, this will be from midnight yesterday
            //DateTime reportFromTime = new DateTime(DateTime.Now.Year, 05, 28, 0, 0, 0);
            //DateTime reportFromTime = new DateTime(DateTime.Now.Year, 02, 01, 0, 0, 0);
            // our reference time that we calculate from is midnight today
            //DateTime reportToTime = new DateTime(DateTime.Now.Year, 03, 01, 0, 0, 0);
            //DateTime toTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, 24, 0, 0);

            List<OmcCdr> verizonVoiceList = new List<OmcCdr>();
            List<OmcCdr> sprintVoiceList = new List<OmcCdr>();
            List<OmcCdr> sprintSmsList = new List<OmcCdr>();
            List<OmcCdr> verizonSmsList = new List<OmcCdr>();
            List<OmcCdr> verizonVoiceSmsList = new List<OmcCdr>();

            WTotalsReport totalsReport = new WTotalsReport();
            WTotalsReport sprintTotalsReport = new WTotalsReport();


            // get our db interface
            WirelessReportDbMgr db = new WirelessReportDbMgr();

            // add 8 hours to compensate for our to time since we are PST which is UTC - 8.
            TimeSpan t = new TimeSpan(8, 0, 0);

            // since CDRs are in UTC format, we add 8 to our time
            DateTime rft = reportFromTime.Add(t);
            DateTime tt = reportToTime.Add(t);

            DataSet ds = db.GetOmcCdrs(rft, tt);
            // For each User the following calculations are performed :  userId : field 3
            // 
            // Total Inbound Calls : total number of inbound calls determined from the "direction" field 5 (terminating)
            // Total Outbound Calls : total number of outbound calls determined from the direction field 5 (originating)
            // Total Call Time : the total of inbound/outbound call times calculated from each of the CDRs for this user
            //    Call Time : answerTime - releaseTime ( field 13 - field 12 )
            // Average Call Time : Call Time / Total Number of Calls
            // 
            // for each CDR we update our report values
            foreach (DataTable myTable in ds.Tables)
            {
                // process each CDR
                foreach (DataRow myRow in myTable.Rows)
                {
                    OmcCdr cdr = null;

                    try
                    {
                        // if we have a voice cdr record
                        if (myRow.ItemArray[2].ToString().Equals("1"))
                        {
                            cdr = this.LoadCdr(myRow);

                            if (cdr.A_Party_Type.Equals("1") || cdr.A_Party_Type.Equals("0"))
                            {
                                //MO
                                cdr.OriginatingMsisdn = cdr.A_Party_Num;
                                string sidBid = m_tds.GetSubscriberSidBid(cdr.OriginatingMsisdn);

                                // check to see if a VZW user
                                if (sidBid.Equals(TechDataEnums.SIDBID_NOT_FOUND.ToString()))
                                {
                                    sprintVoiceList.Add(cdr);

                                    if (ValidCall(cdr))
                                    {
                                        TimeSpan callDuration = cdr.Disconnect.Subtract(cdr.Seize);
                                        // calculate total call duration for this user
                                        sprintTotalsReport.totalMoTime = sprintTotalsReport.totalMoTime + callDuration;
                                        // total good calls = totalMoCalls + totalMtCalls;
                                        // total attemtpted calls = totalMoCallAttempts + totalMtCallAttempts;
                                        sprintTotalsReport.totalMoCalls++;
                                        sprintTotalsReport.totalMoCallAttempts++;
                                    }
                                    else
                                    {
                                        sprintTotalsReport.totalMoCallAttempts++;
                                    }
                                }
                                else
                                {
                                    //add the cdr to our list to generate the report
                                    verizonVoiceList.Add(cdr);
                                    verizonVoiceSmsList.Add(cdr);

                                    if (ValidCall(cdr))
                                    {
                                        TimeSpan callDuration = cdr.Disconnect.Subtract(cdr.Seize);
                                        // calculate total call duration for this user
                                        totalsReport.totalMoTime = totalsReport.totalMoTime + callDuration;
                                        // total good calls = totalMoCalls + totalMtCalls;
                                        // total attemtpted calls = totalMoCallAttempts + totalMtCallAttempts;
                                        totalsReport.totalMoCalls++;
                                        totalsReport.totalMoCallAttempts++;
                                    }
                                    else
                                    {
                                        totalsReport.totalMoCallAttempts++;
                                    }
                                }
                            } //MO case
                            else
                            {
                                // MT
                                cdr.OriginatingMsisdn = cdr.B_Party_Num;
                                string sidBid = m_tds.GetSubscriberSidBid(cdr.OriginatingMsisdn);

                                // check to see if a VZW user
                                if (sidBid.Equals(TechDataEnums.SIDBID_NOT_FOUND.ToString()))
                                {
                                    sprintVoiceList.Add(cdr);

                                    if (ValidCall(cdr))
                                    {
                                        TimeSpan callDuration = cdr.Disconnect.Subtract(cdr.Seize);
                                        // calculate total call duration for this user
                                        sprintTotalsReport.totalMtTime = sprintTotalsReport.totalMtTime + callDuration;
                                        // total good calls = totalMoCalls + totalMtCalls;
                                        // total attemtpted calls = totalMoCallAttempts + totalMtCallAttempts;
                                        sprintTotalsReport.totalMtCalls++;
                                        sprintTotalsReport.totalMtCallAttempts++;
                                    }
                                    else
                                    {
                                        sprintTotalsReport.totalMtCallAttempts++;
                                    }

                                }
                                else
                                {
                                    //add the cdr to our list to generate the report
                                    verizonVoiceList.Add(cdr);
                                    verizonVoiceSmsList.Add(cdr);

                                    if (ValidCall(cdr))
                                    {
                                        TimeSpan callDuration = cdr.Disconnect.Subtract(cdr.Seize);
                                        // calculate total call duration for this user
                                        totalsReport.totalMtTime = totalsReport.totalMtTime + callDuration;
                                        // total good calls = totalMoCalls + totalMtCalls;
                                        // total attemtpted calls = totalMoCallAttempts + totalMtCallAttempts;
                                        totalsReport.totalMtCalls++;
                                        totalsReport.totalMtCallAttempts++;
                                    }
                                    else
                                    {
                                        totalsReport.totalMtCallAttempts++;
                                    }
                                }
                            }// MT case

                        }// VoiceCall

                        else if (myRow.ItemArray[2].ToString().Equals("2"))
                        { // SMS record

                            cdr = this.LoadCdr(myRow);

                            if (cdr.A_Party_Type.Equals("1"))
                            {
                                //MO
                                cdr.OriginatingMsisdn = cdr.A_Party_Num;
                                string sidBid = m_tds.GetSubscriberSidBid(cdr.OriginatingMsisdn);

                                // check to see if a VZW user
                                if (sidBid.Equals(TechDataEnums.SIDBID_NOT_FOUND.ToString()))
                                {
                                    sprintSmsList.Add(cdr);
                                    if (ValidCall(cdr))
                                    {
                                        sprintTotalsReport.totalMoSms++;
                                        sprintTotalsReport.totalMoSmsAttempts++;
                                    }
                                    else
                                    {
                                        sprintTotalsReport.totalMoSmsAttempts++;
                                    }
                                }
                                else
                                {
                                    //add the cdr to our list to generate the report
                                    verizonSmsList.Add(cdr);
                                    verizonVoiceSmsList.Add(cdr);

                                    if (ValidCall(cdr))
                                    {
                                        totalsReport.totalMoSms++;
                                        totalsReport.totalMoSmsAttempts++;
                                    }
                                    else
                                    {
                                        totalsReport.totalMoSmsAttempts++;
                                    }
                                }
                            }
                            else
                            {
                                // MT
                                cdr.OriginatingMsisdn = cdr.B_Party_Num;
                                string sidBid = m_tds.GetSubscriberSidBid(cdr.OriginatingMsisdn);

                                // check to see if a VZW user
                                if (sidBid.Equals(TechDataEnums.SIDBID_NOT_FOUND.ToString()))
                                {
                                    sprintSmsList.Add(cdr);

                                    if (ValidCall(cdr))
                                    {
                                        sprintTotalsReport.totalMtSms++;
                                        sprintTotalsReport.totalMtSmsAttempts++;
                                    }
                                    else
                                    {
                                        sprintTotalsReport.totalMtSmsAttempts++;
                                    }
                                }
                                else
                                {
                                    //add the cdr to our list to generate the report
                                    verizonSmsList.Add(cdr);
                                    verizonVoiceSmsList.Add(cdr);

                                    if (ValidCall(cdr))
                                    {
                                        totalsReport.totalMtSms++;
                                        totalsReport.totalMtSmsAttempts++;
                                    }
                                    else
                                    {
                                        totalsReport.totalMtSmsAttempts++;
                                    }
                                }
                            }

                        }// SMS

                    }

                    catch (SystemException ex)
                    {
                        WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::CreateCallReport():ECaught:" + ex.Message + ex.StackTrace);
                    }

                }
            }

            // good calls
            totalsReport.totalCalls = totalsReport.totalMoCalls + totalsReport.totalMtCalls;
            sprintTotalsReport.totalCalls = sprintTotalsReport.totalMoCalls + sprintTotalsReport.totalMtCalls;

            // call attempts
            totalsReport.totalCallAttempts = totalsReport.totalMoCallAttempts + totalsReport.totalMtCallAttempts;
            sprintTotalsReport.totalCallAttempts = sprintTotalsReport.totalMoCallAttempts + sprintTotalsReport.totalMtCallAttempts;

            // total call times
            totalsReport.totalCallTime = totalsReport.totalMoTime + totalsReport.totalMtTime;
            sprintTotalsReport.totalCallTime = sprintTotalsReport.totalMoTime + sprintTotalsReport.totalMtTime;
            // sms 
            totalsReport.totalSms = totalsReport.totalMoSms + totalsReport.totalMtSms;
            sprintTotalsReport.totalSms = sprintTotalsReport.totalMoSms + sprintTotalsReport.totalMtSms;

            //sms attempts
            totalsReport.totalSmsAttempts = totalsReport.totalMtSmsAttempts + totalsReport.totalMoSmsAttempts;
            sprintTotalsReport.totalSmsAttempts = sprintTotalsReport.totalMtSmsAttempts + sprintTotalsReport.totalMoSmsAttempts;

            // email notification of the call report
            SendCallReportNotificationHtml(verizonVoiceList, sprintVoiceList, verizonSmsList, sprintSmsList,
                totalsReport, sprintTotalsReport, tt, rft, emailList);

            return verizonVoiceSmsList;

        } //CreateDailyCallReport

        private bool ValidCall( OmcCdr cdr )
        {
            if ((cdr.DisconnectCode.Equals("201")) || (cdr.DisconnectCode.Equals("202")) || (cdr.DisconnectCode.Equals("203")) || (cdr.DisconnectCode.Equals("204")))
                return true;

            else
                return false;

        }

        private OmcCdr LoadCdr(DataRow myRow)
        {

            OmcCdr cdr = new OmcCdr();
            cdr.SequenceNumber = myRow.ItemArray[0].ToString();
            cdr.Type = myRow.ItemArray[2].ToString();
            cdr.A_Party_Num = myRow.ItemArray[3].ToString();
            cdr.B_Party_Num = myRow.ItemArray[4].ToString();
            cdr.A_Party_Type = myRow.ItemArray[5].ToString();
            cdr.B_Party_Type = myRow.ItemArray[6].ToString();
            cdr.A_Party_Digits = myRow.ItemArray[7].ToString();
            cdr.B_Party_Digits = myRow.ItemArray[8].ToString();
            cdr.SeizeTime = myRow.ItemArray[13].ToString();
            cdr.AnswerTime = myRow.ItemArray[14].ToString();
            cdr.DisconnectTime = myRow.ItemArray[15].ToString();
            cdr.DisconnectCode = myRow.ItemArray[16].ToString();
            cdr.CellId = myRow.ItemArray[21].ToString();
            cdr.B_CellId = myRow.ItemArray[22].ToString();

            // DateTime formats
            cdr.Seize = (DateTime)myRow.ItemArray[13];
            cdr.Answer = (DateTime)myRow.ItemArray[14];
            cdr.Disconnect = (DateTime)myRow.ItemArray[15];
            return cdr;
        }

        /// <summary>
        /// method to send out a CDR report notification
        /// </summary>
        private void SendCallReportNotificationHtml(List<OmcCdr> vzwVoiceList, List<OmcCdr> sprintVoiceList, List<OmcCdr> vzwSmsList, List<OmcCdr> sprintSmsList, 
            WTotalsReport reportTotals, WTotalsReport sprintTotals, DateTime reportEndTime, DateTime reportStartTime, string emailList)
        {
            try
            {
                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = emailList;
                StringBuilder header = new StringBuilder("Call Statistics \n");
                StringBuilder sbb = new StringBuilder("From " + reportStartTime.ToString("g"));
                sbb.Append(" To : " + reportEndTime.ToString("g"));
                string Subject = sbb.ToString();
                string From = "OWCallReport@enterprisepcs.com";
                MailAddress Bcc = new MailAddress(this.m_bccEmailList);

                StringBuilder bod = new StringBuilder(@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                bod.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
                bod.Append(@"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"" /></head>");
                bod.Append(@"<body>");
                bod.Append(@"<table align=""center"" cellspacing=""0"" cellpadding=""2"" border=""1"">");

                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<br>");
                bod.Append(@"<td colspan=""17"" align=""center""><font face=""verdana"" size=""3""><strong>" + header.ToString() + "</strong></font>");
                bod.Append(@"<br><br><font color=""Blue"" face=""verdana"" size=""2""><strong>" + sbb.ToString() + "</strong></font></td>");
                bod.Append(@"</tr>");

                bod.Append(AddSpace());

                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<td colspan=""17"" align=""center""><font face=""verdana"" size=""2""><strong>VZW Traffic</strong></font></td>");
                bod.Append(@"</tr>");

                bod.Append(AddSpace());

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""17"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>VZW Summary Report</strong></font></td>");
                bod.Append(@"</tr> ");

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Total Attempts</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Total Complete</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MO Attempts</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MO Complete</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MT Attempts</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MT Complete</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Calls</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalCallAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalCalls.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMoCallAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMoCalls.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMtCallAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMtCalls.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>SMS</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalSmsAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalSms.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMoSmsAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMoSms.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMtSmsAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMtSms.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Call Time</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalCallTime.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMoTime.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + reportTotals.totalMtTime.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(AddSpace());

                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>SiteName</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Cell ID</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>SeqNum </b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>RecordType</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>DiscMsg</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>DiscCode</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>APartyNum(MIN)</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>BPartyNum</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>APartyType</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>BPartyType</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>APartyDigits</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>BPartyDigits</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>SeizeTime</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>AnswerTime</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>DiscTime</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Call SetUp</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Call Duration</b></font></td>");

                // verizon voice traffic first
                string rData = FormatData(vzwVoiceList);
                bod.Append(rData);
                bod.Append(AddSpace());

                // vzw sms data
                string smsData = FormatData(vzwSmsList);
                bod.Append(smsData);
                bod.Append(AddSpace());


                // SPRINT traffic , headers and all
                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<td colspan=""17"" align=""center""><font face=""verdana"" size=""2""><strong>SPRINT Traffic</strong></font></td>");
                bod.Append(@"</tr>");

                bod.Append(AddSpace());

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""17"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>SPRINT Summary Report</strong></font></td>");
                bod.Append(@"</tr> ");
                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Total Attempts</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Total Complete</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MO Attempts</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MO Complete</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MT Attempts</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>MT Complete</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Calls</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalCallAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalCalls.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMoCallAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMoCalls.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMtCallAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMtCalls.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>SMS</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalSmsAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalSms.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMoSmsAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMoSms.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMtSmsAttempts.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMtSms.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Call Time</strong></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                 bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalCallTime.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMoTime.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + sprintTotals.totalMtTime.ToString() + "</font></td>");
                bod.Append(@"<td colspan=""2"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"</tr>");

                bod.Append(AddSpace());

                //// verizon voice traffic first
                string sv = FormatData(sprintVoiceList);
                bod.Append(sv);

                bod.Append(AddSpace());

                // sms data
                string ssms = FormatData(vzwSmsList);
                bod.Append(ssms);

                bod.Append(@"<td colspan=""17"" align=""center""><font face=""verdana"" size=""1"" ");
                bod.Append(@"<br>");
                bod.Append(@"<font color=""Red""><b>NOTE : </b></font>");
                bod.Append(@"These numbers reflect traffic for VZW/SPRINT users only; all times are coordinated universal time (UTC).</font></td>");
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
                WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::SendReportNotification():ECaught:" + sme.Message + sme.StackTrace);
            }

        }

        /// <summary>
        /// private method to create a csv report 
        /// </summary>
        /// <param name="cdrList"></param>
        private string CreateCsvReport(List<OmcCdr> cdrList)
        {
            DateTime rDate = DateTime.Now;
            string rFileName =  m_reportOutputDirectory + "OW-MSC-CDR-" + rDate.ToString("yyyyMMddHHmmss") + ".csv";

            // report header
            StringBuilder sb = new StringBuilder();
            sb.Append("SequenceNumber, SiteName, RecordType, DisconnectMessage, DisconnectCode, APartyNumber (MIN), BPartyNum, APartyType,BPartyType");
            sb.Append(",APartyDigits,BPartyDigits,Seize, Answer,Disconnect,CallDuration");

            FileWriter.Instance.WriteToReportFile( rFileName, sb.ToString() );

            foreach (OmcCdr cdr in cdrList)
            {
                try
                {
                    FileWriter.Instance.WriteToReportFile( rFileName, cdr.ToReportStringFormat() );
                }

                catch (SystemException ex)
                {
                    WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::CreateCsvReport():ECaught:" + ex.Message + "\n" + ex.StackTrace);
                    return null;
                }
            }

            return rFileName;        
        }

        /// <summary>
        /// private method to send the CSV report as an attachment
        /// </summary>
        /// <param name="fileToSend"></param>
        private void SendCsvReport(string fileToSend)
        {
            try
            {

                SmtpClient sclient = new SmtpClient(this.m_smtpserver);

                string To = m_csvReportEmailList;
                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg =
                    new MailMessage("OWCSVCallReport@enterprisepcs.com", To, "Daily CallReport", "See the attached spreadsheet.");

                // Create  the file attachment for this e-mail message.
                Attachment data = new Attachment(fileToSend, MediaTypeNames.Application.Octet);
                // Add time stamp information for the file.
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(fileToSend);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(fileToSend);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(fileToSend);
                // Add the file attachment to this e-mail message.
                emsg.Attachments.Add(data);
                sclient.Send(emsg);
            }
            catch (System.Exception ex)
            {// generic exception
                WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::SendCSVReport():ECaught:" + ex.Message + ex.StackTrace);
            }
            return;
        }

        private string AddSpace()
        {
            StringBuilder s = new StringBuilder(@"<tr>");
            s.Append(@"<td colspan=""17""><hr></td>");
            s.Append(@"</tr> ");
            return s.ToString();
        }

        private string FormatData(List<OmcCdr> cdrList)
        {
            StringBuilder bod = new StringBuilder();

            // verizon voice traffic first
            foreach (OmcCdr c in cdrList)
            {
                string siteName = string.Empty;
                string cellId = string.Empty;
                bod.Append(@"<tr> ");
                if (c.A_Party_Type.Equals("2"))
                {
                    siteName = m_cbrDbMgr.GetSiteNameForCellId(c.B_CellId);
                    cellId = c.B_CellId;
                }
                else
                {
                    siteName = m_cbrDbMgr.GetSiteNameForCellId(c.CellId);
                    cellId = c.CellId;
                }
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + siteName + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + cellId + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.SequenceNumber + "</font></td>");
                DisconnectCodeMgr dMgr = new DisconnectCodeMgr();
                string t = dMgr.GetRecordType(c.Type);
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + t + "</font></td>");


                string dMessage = dMgr.GetDisconnectMessage(c.DisconnectCode);
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + dMessage + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.DisconnectCode + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.A_Party_Num + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.B_Party_Num + "</font></td>");
                string type = dMgr.GetPartyType(c.A_Party_Type);
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + type + "</font></td>");
                type = dMgr.GetPartyType(c.B_Party_Type);
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + type + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.A_Party_Digits + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.B_Party_Digits + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.Seize.ToString("s") + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.Answer.ToString("s") + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.Disconnect.ToString("s") + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.Answer.Subtract(c.Seize).ToString() + "</font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + c.Disconnect.Subtract(c.Seize).ToString() + "</font></td>");
                bod.Append(@"</tr>");

            }

            return bod.ToString();
        
        }// FormatData

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

        private void ParseTimeList(string timeList)
        {
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = timeList.Trim().Split(delimiter);

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
            TimeSpan ts = new TimeSpan(1,0,0,0);

            // get our desired report time to run at
             DateTime rDailyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_dailyRunTime, 0, 0).Subtract(ts);


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
                            //run the cumulative daily/hourly report
                            DateTime reportStartTime = new DateTime(DateTime.Now.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                            DateTime reportEndTime = DateTime.Now;
                            this.CreateDailyCallReport(reportStartTime, reportEndTime, this.m_reportEmailList);

                        }
                        else
                        {
                            // we run every 30 mins
                            // convert to milliseconds : 60 mins * 60 secs * X hours 
                            reportInterval = 1000 * 30 * 60;
                        }

                        // check if it is time to run the report
                        DateTime tNow = DateTime.Now;
                        TimeSpan t = tNow.Subtract(rDailyTime);
                        if (t.TotalHours > 24)
                        {
                            DateTime reportFromTime;
                            DateTime reportToTime;

                            if (this.m_useStartMonthDay)
                            {
                                reportFromTime = new DateTime(DateTime.Now.Year, this.m_reportStartMonth, this.m_reportStartDay, 0, 0, 0).Subtract(ts);
                                reportToTime = new DateTime(DateTime.Now.Year, this.m_reportStartMonth , this.m_reportStartDay, 0, 0, 0);
                            }
                            else
                            {
                                 reportFromTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).Subtract(ts);
                                 reportToTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                            }
                            List<OmcCdr> cdrList = this.CreateDailyCallReport(reportFromTime, reportToTime, this.m_dailyReportEmailList);
                            string rFile = this.CreateCsvReport(cdrList);
                            if (rFile != null)
                            {
                                //string u = @"ftp:\\cdrftp01-sttlwa.strata8.net";
                                string u = m_remoteFTPServer;
                                WirelessCdrProcessor.PostFileToSite(u, m_remoteFTPServerUsername, m_remoteFTPServerPassword, rFile);
                                this.SendCsvReport(rFile);
                            }

                            // setup to run at the same time tomorrow
                            rDailyTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_dailyRunTime, 0, 0);

                        }

                    }//try
                    catch (System.Threading.ThreadAbortException ta)
                    {
                        WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::ProcessCdrThread():ServiceIsStopping"+ta.Message);
                        return;
                    }
                    catch (System.Exception ex)
                    {// generic exception

                        WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::ProcessCdrThread():ECaught:" + ex.Message + ex.StackTrace);
                    }

                    System.Threading.Thread.Sleep( reportInterval );

                }// while(true)
            }
            catch (System.Threading.ThreadAbortException tax)
            {
                WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::ProcessCdrThread():ECaught:" + tax.Message + tax.StackTrace);
            }
            catch (System.Exception ex)
            {
                WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::ProcessCdrThread():ECaught:" + ex.Message + ex.StackTrace);
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
        /// method to upload the contents of a file from a remote URI
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static void PostFileToSite(string ftpSiteName, string user, string pass, string fileName)
        {
            try
            {
                string parsedFileName = WirelessCdrProcessor.ParseFileName(fileName);
                // contains the URI path and filename to upload to the remote server
                UriBuilder ub = new UriBuilder(ftpSiteName + "//" + parsedFileName + ".csv");
                Uri ftpUri = ub.Uri;
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                NetworkCredential nc = new NetworkCredential(user, pass);
                CredentialCache cc = new CredentialCache();
                cc.Add(ftpUri, "Basic", nc);
                request.Credentials = cc;

                // Copy the contents of the file to the request stream.
                StreamReader sourceStream = new StreamReader(fileName);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();

                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::PostFileToSite():FilePosted:" + fileName + "-" + response.StatusDescription);

                response.Close();
            }
            catch (WebException e)
            {
                WirelessCdrProcessor.WriteToLogFile("WirelessCdrProcessor::PostFileToSite():ECaught:" + e.Message + "\n" + e.StackTrace);
            }          

        }

        private static string ParseFileName(string fileName)
        {
            string newFileName = String.Empty;

            if (fileName.Contains('\\'))
            {// move the file
                int index = fileName.LastIndexOf('\\');
                newFileName = fileName.Substring(index + 1);

            }
            else
            {
                newFileName = fileName;
            }

            if (newFileName.Contains('.'))
            {
                int i = newFileName.IndexOf('.');
                newFileName = newFileName.Substring(0, i);
            }


            return newFileName;

        }

        /// <summary>
        /// public method to write to the error log file
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteToLogFile( string msg)
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if (!File.Exists(m_LogFile))
            {
                try
                {
                    fs = File.Create(m_LogFile);
                }
                catch (System.Exception ex)
                {
                    return;
                }

            }// created new file and file stream
            else
            {
                // we just append to the file
                try
                {
                    fs = new FileStream(m_LogFile, FileMode.Append, FileAccess.Write);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry("OWReportSvc", "WirelessCdrProcessor::WriteToFile():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }
            }

            // Create a new streamwriter to write to the file   
            try
            {
                sw = new StreamWriter(fs);
                sw.Write(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + msg + "\r\n");

            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry("OWReportSvc", "WirelessCdrProcessor::WriteToFile():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry("OWReportSvc", "WirelessCdrProcessor::WriteToFile():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }
        }
    }//class

}//namespace
