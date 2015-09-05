using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;
using System.Net.Mail;
using TruMobility.Utils.Logging;
using System.Configuration;
using System.Collections;

namespace TruMobility.Reporting.CDR
{
    public class ReportFormatter
    {            
        // get the SMTP Server           
        private string _smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];          
        private string _bccEmailList = "rhernandez@trumobility.com";          
        private static string _from = "DailyCallReport@trumobility.com";
                   
        // parameters around the excel report                               
        private string _excelCallReportToEmailList = System.Configuration.ConfigurationManager.AppSettings["ExcelToEmailList"];
        private bool m_generateExternalCallReport = false;
               
        /// <summary>
        /// method to send out a CDR report notification via HTML
        /// this method includes some private parameters not included in the External email notification method
        /// </summary>
        public void SendGroupReportViaHtml(List<CallReport> gr, string title, string toList )
        {
            try
            {
                SmtpClient sclient = new SmtpClient( _smtpserver );
                MailAddress Bcc = new MailAddress(_bccEmailList);
                string To = toList;

                // add 8 hours to our time zone, pull from the db
                TimeSpan eightHours = new TimeSpan(8, 0, 0);

                // get one report to get the times
                CallReport r1 = gr[0];

                DateTime st = r1.StartTime.Subtract(eightHours);
                DateTime et = r1.EndTime.Subtract(eightHours);
                StringBuilder sbb = new StringBuilder("Call Stats From : " + r1.StartTime.ToString("g"));
                sbb.Append(" To : " + r1.EndTime.ToString("g") + "Report Created at: " + r1.ReportTime.ToString("g") );
                string Subject = sbb.ToString();

                StringBuilder bod = new StringBuilder(@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                bod.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
                bod.Append(@"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"" /></head>");
                bod.Append(@"<body>");
                bod.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""1"">");

                bod.Append(@"<tr bgcolor=""EEEEEE"">"); ;
                bod.Append(@"<td colspan=""9"" align=""center""><img src=""http://trumobility.com/wp-content/themes/trumobility/images/logo.gif"" alt=""Kendall"" border=""0"" /></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<td colspan=""9"" align=""center""><font face=""verdana"" size=""2""><strong>Call Statistics - " + title + "</strong></font></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" colspan=""9"" align=""center""><font face=""verdana"" size=""1"" color=""Blue"">" + sbb.ToString() + "</font>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Name</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Group</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>ServiceProvider</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Number</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Ext.</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Outbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Inbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Avg <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Calls</b></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr>");
                bod.Append(@"<td colspan=""9""><hr></td>");
                bod.Append(@"</tr> ");

                foreach ( CallReport cr in gr )
                {
                // sort by totaloutbound calls, if only doing internal, then it is a daily report, not cumulative
                if ( m_generateExternalCallReport )
                {
                    cr.UserCallReportList.Sort(UserCallReport.CompareByTotalCalls);
                    cr.UserCallReportList.Reverse();
                }
                else
                { // sorted by cumulative
                    cr.UserCallReportList.Sort(UserCallReport.CompareByTotalCalls);
                    cr.UserCallReportList.Reverse();
                }

                foreach (UserCallReport userReport in cr.UserCallReportList)
                {
                    bod.Append(@"<tr> ");
                    //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserName + "</font></td>");
                   // bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserId + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.Group + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.ServiceProvider + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserNumber + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserExtension + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalOutboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalInboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.AverageCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalCalls + "</font></td>");
                    bod.Append(@"</tr>");

                }
                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""4"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Totals</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalOutboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalInboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalCallTime.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalCalls.ToString() + "</strong></font></td>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCumulativeCalls.ToString() + "</strong></font></td>");
                bod.Append(@"</tr>");

                }// for each group

                bod.Append(@"<td colspan=""9"" align=""center""><font face=""verdana"" size=""1"" ");
                bod.Append(@"<br>");
                bod.Append(@"<font color=""Red""><b>NOTE : </b></font>");
                bod.Append(@"These numbers reflect traffic on the TruMobility Network only.</font></td>");
                bod.Append(@"<br>");
                bod.Append(@"</table>");

                // end of BODY and our HTML format 
                bod.Append(@"</body></html>");

                SendNotification( toList, Subject, bod.ToString() );

            }
            catch (System.Exception se)
            {// exception handling 
                LogFileMgr.Instance.WriteToLogFile("ReportFormatter::SendGroupReportViaHtml():ECaught:" + se.Message);
            }

        }

        /// <summary>
        /// method to send out a CDR report notification via HTML
        /// this method includes some private parameters not included in the External email notification method
        /// </summary>
        public void SendGroupReportSummaryViaHtml(List<CallReport> gr, string title, string toList, Hashtable spReport)
        {
            try
            {
                // add 8 hours to our time zone, pull from the db
                TimeSpan eightHours = new TimeSpan(8, 0, 0);

                // get one report to get the times
                CallReport r1 = gr[0];

                DateTime st = r1.StartTime.Subtract(eightHours);
                DateTime et = r1.EndTime.Subtract(eightHours);

                StringBuilder sbb = new StringBuilder("Call Stats From : " + st.ToString("g"));
                sbb.Append(" To : " + et.ToString("g") + " Report Created at: " + r1.ReportTime.ToString("g"));
                string Subject = sbb.ToString();

                StringBuilder bod = new StringBuilder(@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                bod.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
                bod.Append(@"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"" /></head>");
                bod.Append(@"<body>");
                bod.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""1"">");

                bod.Append(@"<tr bgcolor=""EEEEEE"">"); ;
                bod.Append(@"<td colspan=""8"" align=""center""><img src=""http://trumobility.com/wp-content/themes/trumobility/images/logo.gif"" alt=""Kendall"" border=""0"" /></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<td colspan=""8"" align=""center""><font face=""verdana"" size=""2""><strong>Call Statistics - " + title + "</strong></font></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" colspan=""8"" align=""center""><font face=""verdana"" size=""1"" color=""Blue"">" + sbb.ToString() + "</font>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Name</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Group</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>ServiceProvider</b></font></td>"); 
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Outbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Inbound</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> International</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Avg <br> Call Time</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>Total Calls</b></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<tr>");
                bod.Append(@"<td colspan=""8""><hr></td>");
                bod.Append(@"</tr> ");

                Hashtable sp_cumulative = new Hashtable();
                
                // for the service providers do a summary followed by the details
                //
                foreach (DictionaryEntry de in spReport)
                {
                    CumulativeReport r = (CumulativeReport)de.Value; 
                    bod.Append(@"<tr> ");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""> ServiceProvider Summary </font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + de.Key.ToString() + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + r.TotalOutboundCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + r.TotalInboundCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + r.TotalInternationalCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + r.TotalCallTime.ToString("c") + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong> </strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + r.TotalCalls.ToString() + "</strong></font></td>");
                    //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCumulativeCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"</tr>");

                }

                // add space
                bod.Append(@"<tr>");
                bod.Append(@"<td colspan=""8""><hr></td>");
                bod.Append(@"</tr> ");

                foreach (CallReport cr in gr)
                {
                    // sort by totaloutbound calls, if only doing internal, then it is a daily report, not cumulative
                    if (m_generateExternalCallReport)
                    {
                        cr.UserCallReportList.Sort(UserCallReport.CompareByTotalCalls);
                        cr.UserCallReportList.Reverse();
                    }
                    else
                    {
                        cr.UserCallReportList.Sort(UserCallReport.CompareByTotalCalls);
                        cr.UserCallReportList.Reverse();
                    }

                    bod.Append(@"<tr> ");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + cr.GroupId + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + cr.ServiceProvider + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalOutboundCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalInboundCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalInternationalCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalCallTime.ToString("c") + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.AverageCallTime + "</strong></font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalCalls.ToString() + "</strong></font></td>");
                    //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCumulativeCalls.ToString() + "</strong></font></td>");
                    bod.Append(@"</tr>");


                }// for each group

                bod.Append(@"<td colspan=""8"" align=""center""><font face=""verdana"" size=""1"" ");
                bod.Append(@"<br>");
                bod.Append(@"<font color=""Red""><b>NOTE : </b></font>");
                bod.Append(@"These numbers reflect traffic on the TruMobility Network only.</font></td>");
                bod.Append(@"<br>");
                bod.Append(@"</table>");

                // end of BODY and our HTML format 
                bod.Append(@"</body></html>");

                SendNotification( toList, Subject, bod.ToString() );

            }
            catch (System.Exception se)
            {// exception handling

                LogFileMgr.Instance.WriteToLogFile("ReportFormatter::SendGroupReportViaHtml():ECaught:" + se.Message);
            }

        }

        private void SendNotification(string to, string subject, string body)
        {

            try
            {

                SmtpClient sclient = new SmtpClient(this._smtpserver);
                MailAddress Bcc = new MailAddress(this._bccEmailList);

                // create and send the SMTP message
                System.Net.Mail.MailMessage emsg = new MailMessage(_from, to, subject, body); //sb.ToString());

                ContentType mimeType = new System.Net.Mime.ContentType("text/html");
                // Add the alternate body to the message.

                AlternateView alternate = AlternateView.CreateAlternateViewFromString(body, mimeType);
                emsg.AlternateViews.Add(alternate);
                emsg.Bcc.Add(Bcc);
                sclient.Send(emsg);
            }
            catch (System.Net.Mail.SmtpException sme)
            {// exception handling

                LogFileMgr.Instance.WriteToLogFile("ReportFormatter::SendNotification():ECaught:" + sme.Message);
            }
            catch ( SystemException se )
            {// exception handling
                LogFileMgr.Instance.WriteToLogFile("ReportFormatter::SendNotification():ECaught:" + se.Message);
            }
            
        }

        /// <summary>
        /// method to generate the excel call report format for each user
        /// </summary>
        /// <param name="reportList"></param>
        /// <param name="theReportTime"></param>
        /// <param name="referenceTime"></param>
        private void GenerateExcelCallReport(List<UserCallReport> reportList, string toList , string fileName)
        {
            string comma = ",";

            DateTime date = DateTime.Now;
            string rFileName = fileName + date.Year.ToString() + date.Month.ToString()
                + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() +
                ".csv";
            // create the subject 

            string Subject = "DailyCallReport";

            StringBuilder sb = new StringBuilder("UserName, UserId, TotalCallsOut, TotalCallsIn, TotalCallTime, AvgCallTime, TotalCalls, TotalCumulativeCalls" + "\r\n");

            foreach (UserCallReport userReport in reportList)
            {
                sb.Append(userReport.UserName + comma);
                sb.Append(userReport.UserId + comma);
                sb.Append(userReport.TotalOutboundCalls + comma);
                sb.Append(userReport.TotalInboundCalls + comma);
                sb.Append(userReport.TotalCallTime + comma + userReport.AverageCallTime + comma);
                sb.Append(userReport.TotalCalls + comma + userReport.TotalCumulativeCalls + "\r\n"); // add the new line
            }

            LogFileMgr.Instance.WriteToFile(rFileName, sb.ToString());

            // send the file in an email
            string To = toList;
            string From = _from;
            MailAddress Bcc = new MailAddress(this._bccEmailList);
            // Create a message and set up the recipients.
            MailMessage message = new MailMessage(
               From,
               _excelCallReportToEmailList,
               Subject,
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
            SmtpClient client = new SmtpClient(this._smtpserver);
            // Add credentials if the SMTP server requires them.
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (System.Net.Mail.SmtpException sme)
            {
                LogFileMgr.Instance.WriteToLogFile("CdrProcessor::SendNotification():ECaught:" + sme.Message);
            }
            // Display 

        }

        /// <summary>
        /// method to send out a CDR report notification via HTML
        /// this method includes some private parameters not included in the External email notification method
        /// </summary>
        private void SendCallReportNotificationHtml(CallReport cr, string title, string toList)
        {
            try
            { 
                string To = toList;
                StringBuilder sbb = new StringBuilder("Call Stats From : " + cr.StartTime.ToString("g"));
                sbb.Append(" To : " + cr.EndTime.ToString("g") + "Report Created at: " + cr.ReportTime.ToString("g"));
                string Subject = sbb.ToString();
                MailAddress Bcc = new MailAddress(_bccEmailList);

                StringBuilder bod = new StringBuilder(@" <!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">");
                bod.Append(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
                bod.Append(@"<head><meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"" /></head>");
                bod.Append(@"<body>");
                bod.Append(@"<table cellspacing=""0"" cellpadding=""2"" border=""1"">");

                bod.Append(@"<tr bgcolor=""EEEEEE"">"); ;
                bod.Append(@"<td colspan=""8"" align=""center""><img src=""http://trumobility.com/wp-content/themes/trumobility/images/logo.gif"" alt=""Kendall"" border=""0"" /></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr bgcolor=""EEEEEE"">");
                bod.Append(@"<td colspan=""8"" align=""center""><font face=""verdana"" size=""2""><strong>Call Statistics - " + title + "</strong></font></td>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                bod.Append(@"<td bgcolor=""CCFFCC"" colspan=""8"" align=""center""><font face=""verdana"" size=""1"" color=""Blue"">" + sbb.ToString() + "</font>");
                bod.Append(@"</tr>");
                bod.Append(@"<tr>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Name</b></font></td>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User ID</b></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><b>User Number</b></font></td>");
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
                    cr.UserCallReportList.Sort(UserCallReport.CompareByTotalCalls);
                    cr.UserCallReportList.Reverse();
                }
                else
                {
                    cr.UserCallReportList.Sort(UserCallReport.CompareByTotalCalls);
                    cr.UserCallReportList.Reverse();
                }

                foreach (UserCallReport userReport in cr.UserCallReportList)
                {
                    bod.Append(@"<tr> ");
                    //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserName + "</font></td>");
                    // bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserId + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserNumber + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.UserExtension + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalOutboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalInboundCalls + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.AverageCallTime + "</font></td>");
                    bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1"">" + userReport.TotalCalls + "</font></td>");
                    bod.Append(@"</tr>");

                }
                bod.Append(@"<tr> ");
                bod.Append(@"<td colspan=""3"" bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>Totals</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalOutboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalInboundCalls.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalCallTime.ToString() + "</strong></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""></font></td>");
                bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + cr.TotalsReport.TotalCalls.ToString() + "</strong></font></td>");
                //bod.Append(@"<td bgcolor=""CCFFCC"" align=""center""><font face=""verdana"" size=""1""><strong>" + reportTotals.totalCumulativeCalls.ToString() + "</strong></font></td>");
                bod.Append(@"</tr>");

                bod.Append(@"<td colspan=""8"" align=""center""><font face=""verdana"" size=""1"" ");
                bod.Append(@"<br>");
                bod.Append(@"<font color=""Red""><b>NOTE : </b></font>");
                bod.Append(@"These numbers reflect traffic on the TruMobility Network only.</font></td>");
                bod.Append(@"<br>");
                bod.Append(@"</table>");

                // end of BODY and our HTML format 
                bod.Append(@"</body></html>");

                SendNotification(toList, Subject, bod.ToString());

            }
            catch (SystemException se)
            {// exception handling
                LogFileMgr.Instance.WriteToLogFile("CdrProcessor::SendNotification():ECaught:" + se.Message);
            }

        }

        /// <summary>
        /// method to generate the excel call report format for each group
        /// </summary>
        /// <param name="reportList"></param>configurable parameter in the config file
        /// <param name="theReportTime"></param>
        /// <param name="referenceTime"></param>
        public void GenerateExcelCallReportForGroups(List<CallReport> reportList, string fileName)
        {
            string comma = ",";

            DateTime date = DateTime.Now;
            string rFileName = fileName + date.Year.ToString() + date.Month.ToString()
                + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() +
                ".csv";

            // create the subject 
            // add 8 hours to our time zone, pull from the db
            TimeSpan eightHours = new TimeSpan(8, 0, 0);

            // get one report to get the times
            CallReport r1 = reportList[0];

            DateTime st = r1.StartTime.Subtract(eightHours);
            DateTime et = r1.EndTime.Subtract(eightHours);

            StringBuilder sbb = new StringBuilder("Call Stats From : " + st.ToString("g"));
            sbb.Append(" To : " + et.ToString("g") + " Report Created at: " + r1.ReportTime.ToString("g"));
            string Subject = sbb.ToString();

            StringBuilder sb = new StringBuilder("ServiceProvider, GroupId, TotalCallsOut, TotalCallsIn, TotalInternationalCalls,TotalCallTime, AvgCallTime, TotalCalls" + "\r\n");

            foreach (CallReport cr in reportList)
            {                    
                sb.Append( cr.ServiceProvider + comma );
                sb.Append( cr.GroupId + comma );
                sb.Append( cr.TotalsReport.TotalOutboundCalls.ToString() + comma );
                sb.Append( cr.TotalsReport.TotalInboundCalls.ToString() + comma );
                sb.Append(cr.TotalsReport.TotalInternationalCalls.ToString() + comma + cr.TotalsReport.TotalCallTime.ToString("c")+ comma + cr.TotalsReport.AverageCallTime + comma);
                sb.Append(cr.TotalsReport.TotalCalls.ToString()  + "\r\n"); // add the new line
            }

            LogFileMgr.Instance.WriteToFile(rFileName, sb.ToString());

            // send the file in an email
            string From = _from;
            MailAddress Bcc = new MailAddress(this._bccEmailList);
            // Create a message and set up the recipients.
            MailMessage message = new MailMessage(
               From,
               _excelCallReportToEmailList,
               Subject,
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
            SmtpClient client = new SmtpClient(this._smtpserver);
            // Add credentials if the SMTP server requires them.
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (System.Net.Mail.SmtpException sme)
            {
                LogFileMgr.Instance.WriteToLogFile("CdrProcessor::SendNotification():ECaught:" + sme.Message);
            }
            // Display 

        }

    }

}
