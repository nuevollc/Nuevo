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
        private static string _smtpserver = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
        private static string _bccEmailList = System.Configuration.ConfigurationManager.AppSettings["BCCEMailList"];
        private static string _from = System.Configuration.ConfigurationManager.AppSettings["FromEmail"];
        private static string _subject = System.Configuration.ConfigurationManager.AppSettings["ExcelSubjectEmail"];
        private static string _fileName = System.Configuration.ConfigurationManager.AppSettings["ExcelFileName"];  

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
            // create a summary for each user
            ExcelMgr.CreateCSV(reportList, rFileName);

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

    }

}
