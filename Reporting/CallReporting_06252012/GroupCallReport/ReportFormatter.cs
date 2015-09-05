using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mime;
using System.Net.Mail;
using TruMobility.Utils.Logging;

namespace TruMobility.Reporting.CDR.Groups
{
    public class ReportFormatter
    {            
        // get the SMTP Server           
        private static string _smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
        private static string _bccEmailList = System.Configuration.ConfigurationManager.AppSettings["BCCEMailList"];  
        private static string _from =  System.Configuration.ConfigurationManager.AppSettings["FromEmail"];
        private static string _subject = System.Configuration.ConfigurationManager.AppSettings["ExcelSubjectEmail"];
        private static string _fileName = System.Configuration.ConfigurationManager.AppSettings["ExcelFileName"];  
                                 
        private void SendNotification(string to, string subject, string body)
        {
            try
            {
                SmtpClient sclient = new SmtpClient(ReportFormatter._smtpserver);
                MailAddress Bcc = new MailAddress(ReportFormatter._bccEmailList);

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
        public void GenerateExcelUserCallReport( List<UserCallReport> reportList, string toList )
        {

            DateTime date = DateTime.Now;

            // csv format
            string rFileName = _fileName + date.Year.ToString() + date.Month.ToString()
                + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() +
                ".csv";
            CreateExcelDoc.CreateCSV(reportList, rFileName); 

            // xcel format
            //string rFileName = _fileName + date.Year.ToString() + date.Month.ToString() + date.Day.ToString()
            //    + date.Hour.ToString() + date.Minute.ToString() + ".xlsx";
            //CreateExcelDoc.CreateCallSummaryDoc(reportList, rFileName);

            // send the file in an email
            string To = toList;
            string From = _from;
            MailAddress Bcc = new MailAddress( ReportFormatter._bccEmailList ) ;
            // Create a message and set up the recipients.
            MailMessage message = new MailMessage(
               From,
               To,
               _subject,
               "See the attached group daily call report spreadsheet.");

            // Create  the file attachment for this e-mail message.
            Attachment data = new Attachment(rFileName, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.not needed hombre
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(rFileName);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(rFileName);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(rFileName);
            // Add the file attachment to this e-mail message.
            message.Attachments.Add(data);

            // add some Bcc
            message.Bcc.Add(Bcc);

            //Send the message.
            SmtpClient client = new SmtpClient(ReportFormatter._smtpserver);
            // Add credentials if the SMTP server requires them.
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (System.Net.Mail.SmtpException sme)
            {
                LogFileMgr.Instance.WriteToLogFile("ReportFormatter::SendNotification():ECaught:" + sme.Message);
            }
            // Display 

        }

        /// <summary>
        /// method to generate the excel call report format for each user
        /// </summary>
        /// <param name="reportList"></param>
        /// <param name="theReportTime"></param>
        /// <param name="referenceTime"></param>
        public void GenerateCsvUserCallReport(List<UserCallReport> reportList, string toList)
        {

            DateTime date = DateTime.Now;

            // csv format
            string rFileName = _fileName + date.Year.ToString() + date.Month.ToString()
                + date.Day.ToString() + date.Hour.ToString() + date.Minute.ToString() +
                ".csv";
            CreateExcelDoc.CreateUserCSVReport(reportList, rFileName);

            // send the file in an email
            string To = toList;
            string From = _from;
            MailAddress Bcc = new MailAddress(ReportFormatter._bccEmailList);
            // Create a message and set up the recipients.
            MailMessage message = new MailMessage(
               From,
               To,
               _subject,
               "See the attached User Daily Call Report spreadsheet.");

            // Create  the file attachment for this e-mail message.
            Attachment data = new Attachment(rFileName, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.not needed hombre
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(rFileName);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(rFileName);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(rFileName);
            // Add the file attachment to this e-mail message.
            message.Attachments.Add(data);

            // add some Bcc
            message.Bcc.Add(Bcc);

            //Send the message.
            SmtpClient client = new SmtpClient(ReportFormatter._smtpserver);
            // Add credentials if the SMTP server requires them.
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (System.Net.Mail.SmtpException sme)
            {
                LogFileMgr.Instance.WriteToLogFile("ReportFormatter::SendNotification():ECaught:" + sme.Message);
            }
            // Display 

        }


        /// <summary>
        /// method to generate the excel call report format for each group
        /// </summary>
        /// <param name="reportList"></param>configurable parameter in the config file
        /// <param name="theReportTime"></param>
        /// <param name="referenceTime"></param>
        public void GenerateExcelCallReportForGroups(List<CallReport> reportList, string toList)
        {
            string comma = ",";

            DateTime date = DateTime.Now;
            string rFileName = ReportFormatter._fileName + date.Year.ToString() + date.Month.ToString()
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
            MailAddress Bcc = new MailAddress(ReportFormatter._bccEmailList);
            // Create a message and set up the recipients.
            MailMessage message = new MailMessage(
               From,
               toList,
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
            SmtpClient client = new SmtpClient(ReportFormatter._smtpserver);
            // Add credentials if the SMTP server requires them.
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (System.Net.Mail.SmtpException sme)
            {
                LogFileMgr.Instance.WriteToLogFile("ReportFormatter::SendNotification():ECaught:" + sme.Message);
            }
            // Display 

        }

    }

}
