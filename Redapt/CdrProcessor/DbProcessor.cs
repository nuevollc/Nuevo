using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Web;
//using System.Web.Services;
//using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Diagnostics;
using System.Configuration;


namespace Strata8.CDR.Reporting
{
    public class DbProcessor
    {

        private string m_EventLogName = null;
        private string m_LogFile = null;
        
        private string m_connectionString = ConfigurationManager.AppSettings["SQLConnectString"];

        public DbProcessor()
        {
            try
            {
                // method that initialized params from config file

                // get the event log name
                m_EventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogName"];
                // set up event logging
                if (!EventLog.SourceExists(m_EventLogName))
                {
                    EventLog.CreateEventSource(m_EventLogName, "Application");
                }

                m_LogFile = ConfigurationManager.AppSettings["LogFile"];
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_EventLogName, "Exception Caught:" + ex.Message, EventLogEntryType.Error, 2001);
                return;
            }

            EventLog.WriteEntry(m_EventLogName, "DbProcessor()::ServiceConfigurationIsComplete", EventLogEntryType.Information, 3000);

        }

        /// <summary>
        /// "Method to get CDRs from a Subsystem starting from startdate to NOW")
        /// </summary>
        /// <param name="subsystemName"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public DataSet GetCdrsForPhoneNumber(string userNumber, DateTime startDate)
        {
            StringBuilder cmdStr = new StringBuilder("SELECT * FROM BworksCdr where answerTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            cmdStr.Append("' AND userNumber like '%" + userNumber.Trim() + "' ORDER BY startTime DESC");

            DataSet mySet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr.ToString(), connection);
                    myDaptor.Fill(mySet);
                }

                
            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_EventLogName, "DbProcessor::GetCdrsForPhoneNumber():ECaught:" + ex.Message, EventLogEntryType.Error, 3000);

            }

            return mySet;

        }  //GetCdrsForPhoneNumber

        public DataSet GetCdrsToCalledNumber(string userNumber, string calledNumber, DateTime startDate)
        {
            StringBuilder cmdStr = new StringBuilder("SELECT * FROM BworksCdr where answerTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            cmdStr.Append("' AND userNumber like '%" + userNumber.Trim() + "' AND ");
            cmdStr.Append(" calledNumber like '%" + calledNumber.Trim() + "' ORDER BY startTime DESC");

            DataSet mySet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr.ToString(), connection);
                    myDaptor.Fill(mySet);
                }


            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_EventLogName, "DbProcessor::GetCdrsToCalledNumber():ECaught:" + ex.Message, EventLogEntryType.Error, 3000);

            }

            return mySet;

        }  //GetCdrsForPhoneNumber
    }// class

} // namespace

