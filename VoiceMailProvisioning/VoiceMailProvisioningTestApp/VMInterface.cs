using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.SqlClient;
//using System.Data.Sql;
//using System.Data;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using MySql.Data;
using MySql.Data.MySqlClient;

using System.Collections;
namespace VoiceMailProvisioningTestApp
{

    class VMInterface
    {

        private string _mailboxPassword = string.Empty;
        private string _domain = "voicemail.strata8.net";
        private string _logFile = string.Empty;

        public VMInterface()
        {

            _mailboxPassword = ConfigurationManager.AppSettings["MailBoxPassword"];
            // get the error log filename
            _logFile = ConfigurationManager.AppSettings["LogFile"];
        }

        public bool ProvisionVoiceMailUser(string phoneNumber)
        {
            bool allGood = true;
            DateTime dateCreated = DateTime.Now;
            string dbTime = dateCreated.ToLongDateString() + " " + dateCreated.ToLongTimeString();

            // make the connection
            MySql.Data.MySqlClient.MySqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

                try
                {
                    string did = phoneNumber.Trim();

                    System.Text.StringBuilder cmdStr = new StringBuilder("INSERT INTO mailbox ");
                    cmdStr.Append("(username,password,name,maildir, quota, domain, created, modified, active ) VALUES ( ");

                    cmdStr.Append("'" + phoneNumber + "@" + _domain + "'");
                    cmdStr.Append(",'" + _mailboxPassword + "'");
                    cmdStr.Append(",'" + "TestFirst" + " " + "TestLast" + "'");
                    cmdStr.Append(",'" + @"voicemail.strata8.net/" + phoneNumber + "/" + "'");
                    cmdStr.Append(",'" + "10240000" + "'");
                    cmdStr.Append(",'" + _domain + "'");
                    // cmdStr.Append(",'" + dbTime + "'");
                    // cmdStr.Append(",'" + dbTime + "'");
                    cmdStr.Append(",'" + "0000-00-00 00:00:00" + "'"); 
                    cmdStr.Append(",'" + "0000-00-00 00:00:00" + "'");
                    cmdStr.Append(",'" + 1 + "'");

                    // close the statement
                    cmdStr.Append(")");

                    MySqlCommand sqlCommand = new MySqlCommand(cmdStr.ToString(), dataConnection);
                    sqlCommand.CommandType = System.Data.CommandType.Text;                    
                    sqlCommand.ExecuteNonQuery();

                    // close the connection
                    CloseDataConn(ref dataConnection);

                    // add the alias table; if not good update status and return
                    if (!AddAlias(phoneNumber))
                    {
                        allGood = false;
                    }
                    else
                    {
                        // update the provisioning database 
                        // ****
                        // ***** add interface to provisioning database here
                        // ****
                    }

                }
                catch (System.Exception ex)
                {
                    allGood = false;
                    LogFileError("ProvisionVoiceMailUser:" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);

                    //EventLog.WriteEntry(m_eventLogName, "ECaught:DID#" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3000);
                }

            return allGood;

        }// ProvisionVoiceMailUser

        public bool AddAlias(string phoneNumber)
        {
            bool allGood = true;

            // make the connection
            MySql.Data.MySqlClient.MySqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                string did = phoneNumber.Trim();

                System.Text.StringBuilder cmdStr = new StringBuilder("INSERT INTO alias ");
                cmdStr.Append("(address,goto,domain,created, modified, active ) VALUES ( ");

                cmdStr.Append("'" + phoneNumber + "@" + _domain + "'");
                cmdStr.Append("," + phoneNumber + "@" + _domain + "'"); 
                cmdStr.Append(",'" + _domain + "'");
                // cmdStr.Append(",'" + dbTime + "'");
                // cmdStr.Append(",'" + dbTime + "'");
                cmdStr.Append(",'" + "0000-00-00 00:00:00" + "'");
                cmdStr.Append(",'" + "0000-00-00 00:00:00" + "'");
                cmdStr.Append(",'" + 1 + "'");

                // close the statement
                cmdStr.Append(")");

                MySqlCommand sqlCommand = new MySqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception ex)
            {
                allGood = false;
                LogFileError("AddAlias:" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);

                //EventLog.WriteEntry(m_eventLogName, "ECaught:DID#" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3000);
            }

            // close the connection
            CloseDataConn(ref dataConnection);

            return allGood;

        }// addAlias

        /// <summary>
        /// method to open the database connection
        /// </summary>
        /// <param name="dataConnection"></param>
        private void OpenDataConn(ref MySqlConnection dataConnection)
        {
            try
            {
                dataConnection = new MySqlConnection();
                dataConnection.ConnectionString = ConfigurationManager.AppSettings["SQLConnectString"];
                dataConnection.Open();

            }// try 
            catch (Exception e)
            {
                //EventLog.WriteEntry(m_eventLogName, "FtpHandlerSvc::OpenDataConnection():FailedEstablishingDBConnection:" + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }

        /// <summary>
        /// method used to close the database connection
        /// </summary>
        /// <param name="dataConnection"></param>
        private void CloseDataConn(ref MySqlConnection dataConnection)
        {
            try
            {
                if (dataConnection != null)
                {
                    dataConnection.Close();
                    dataConnection = null;
                }

            }// try 
            catch (Exception e)
            {
                // EventLog.WriteEntry(m_eventLogName, "FtpHandlerSvc::OpenDataConnection():FailedTryingToCloseTheDBConnection:" + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void CloseDataConn( ref SqlConnection dataConnection )   


        private void LogFileError(string msg)
        {
            try
            {
                FileStream file = new FileStream(_logFile, FileMode.Append, FileAccess.Write);

                // Create a new stream to write to the file
                StreamWriter sw = new StreamWriter(file);
                //sw.Write("----------\r\n");
                // Write a string to the file
                sw.Write(msg + "\r\n");
                //sw.Write("----------\r\n");
                // Close StreamWriter
                sw.Close();
                // Close file
                file.Close();
            }
            catch (System.Exception ex)
            {
                //EventLog.WriteEntry(m_eventLogName, "Error Ocurred While Writing to Error Log File>" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
            }
        }// public void LogFileError(string msg)
    
    }//class

}//namespace

