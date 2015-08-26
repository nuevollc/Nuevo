using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Diagnostics;
using System.Configuration;

using TruMobility.Utils.Logging;

namespace TruMobility.Reporting.CDR
{
    public class CdrDbProcessor
    {

        private string m_connectionString = ConfigurationManager.AppSettings["BWorksCdr_SQLConnectString"];

        public CdrDbProcessor()
        {
            try
            {
                // method that initialized params from config file

                //// get the event log name
                // m_EventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogName"];
                //// set up event logging
                //if (!EventLog.SourceExists(m_EventLogName))
                //{
                //    EventLog.CreateEventSource(m_EventLogName, "Application");
                //}
                 
            }
            catch (System.Exception ex)
            {
                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::CdrDbProcessor():FailedTryingToInitializeEventLog");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            //LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::CdrDbProcessor():ServiceConfigurationIsComplete"); 
        }

        public List<string> GetUsers( DateTime startDate )
        {

            List<string> phoneList = new List<string>();  // list of users

            StringBuilder cmdStr = new StringBuilder("SELECT DISTINCT userNumber FROM BworksCdr where startTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            string order = "' ORDER BY userNumber DESC";
            cmdStr.Append(order);

            DataSet ds = new DataSet();

            try
            {
                ds = GetData(cmdStr.ToString());

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    //r.count = myTable.Rows.Count.ToString();

                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        phoneList.Add((String)myRow.ItemArray[0]);
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetUsers():FailedTryingToGetTheListOfUsers");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return phoneList;

        }
       
        /// <summary>
        /// "Method to get CDRs from a Subsystem starting from startdate to NOW")
        /// </summary>
        /// <param name="subsystemName"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public DataSet GetAllCdrsForPhoneNumber(string userNumber, DateTime startDate, DateTime endDate)
        {
            StringBuilder cmdStr = new StringBuilder("SELECT * FROM BworksCdr where startTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            cmdStr.Append("' AND startTime < '" + endDate.ToString() + "' AND userNumber like '%" + userNumber.Trim() + "'  AND answerIndicator = 'Yes' ");
            cmdStr.Append("  ORDER BY startTime DESC");

            DataSet mySet = new DataSet();

            try
            {
                mySet = GetData(cmdStr.ToString());

            }
            catch (System.Exception ex)
            {
                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetCdrsForPhoneNumber():FailedTryingToGetCDRs");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return mySet;

        }  //GetCdrsForPhoneNumber

        public List<Subscriber> GetUsersInGroup(string dept, DateTime startDate, DateTime endDate)
        {

            List<Subscriber> subList = new List<Subscriber>();  // list of users

            StringBuilder cmdStr = new StringBuilder("SELECT DISTINCT userNumber, department, serviceProvider FROM BworksCdr where startTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            string order = "' AND startTime < '" + endDate.ToString() + "' AND department = '" + dept + "' AND answerIndicator='Yes' ORDER BY userNumber DESC";
            cmdStr.Append(order);

            DataSet ds = new DataSet();

            try
            {
                ds = GetData(cmdStr.ToString());

                // process all the users in the group
                foreach (DataTable myTable in ds.Tables)
                {

                    // one row per user
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        Subscriber s = new Subscriber();
                        s.PhoneNumber = (String)myRow.ItemArray[0];
                        s.Group = (String)myRow.ItemArray[1];
                        s.ServiceProvider = (String)myRow.ItemArray[2];
                        subList.Add(s);
                    }

                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetUsers():FailedTryingToGetTheListOfUsers");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return subList;

        }

        /// <summary>
        /// method to execute the sql query
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <returns></returns>
        private DataSet GetData(string cmdStr)
        {

            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr, m_connectionString);
                    myDaptor.Fill(ds);
                }

            }

            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetData():FailedCommand" + cmdStr);
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return ds;

        }


        /// <summary>
        /// public method that returns the first and last name 
        /// of the user.
        /// </summary>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        public string GetName(  string userNumber )
        {
            string ext = String.Empty;
            string num = String.Empty;
            string firstName = String.Empty;
            string lastName = String.Empty;

            if (userNumber.Trim().Contains("+1"))
                num = userNumber.Trim().Substring(2, 10);
            else if (userNumber.Trim().Contains("1"))
                num = userNumber.Trim().Substring(1, 10);
            else
                num = userNumber.Trim() ;
               
            StringBuilder cmdStr = new StringBuilder("SELECT EXTENSION, firstName, lastName FROM Extensions where USERNUMBER = '" + num + "'" );

            DataSet ds = new DataSet();

            try
            {
                ds = GetData(cmdStr.ToString());

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    //r.count = myTable.Rows.Count.ToString();

                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        ext = ((String)myRow.ItemArray[0]);
                        firstName = ((String)myRow.ItemArray[1]);
                        lastName = ((String)myRow.ItemArray[2]);

                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetExtension():FailedTryingToGetTheExtension");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return (firstName + lastName);

        }


    }// class

} // namespace

