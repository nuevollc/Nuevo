using System;
using System.Collections.Generic;
using System.Text; 
using System.Data.SqlClient;
using System.Data;
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
        public DataSet GetCdrsForPhoneNumber(string userNumber, DateTime startDate, DateTime endDate)
        {
            StringBuilder cmdStr = new StringBuilder("SELECT * FROM BworksCdr where startTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            cmdStr.Append("' AND startTime < '" + endDate.ToString() + "' AND userNumber like '%" + userNumber.Trim() + "' AND answerIndicator='Yes' ORDER BY startTime DESC");

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
            cmdStr.Append("' AND startTime < '" + endDate.ToString() + "' AND userNumber like '%" + userNumber.Trim() + "'  ORDER BY startTime DESC");

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
                mySet = GetData(cmdStr.ToString());
            }
            catch (System.Exception ex)
            {
                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetCdrsToCalledNumber():FailedTryingToGetCDRs");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return mySet;

        }  //GetCdrsForPhoneNumber

        public List<string> GetGroupIds( DateTime startDate, DateTime endDate )
        {
            List<string> groupId = new List<string>();

            StringBuilder cmdStr = new StringBuilder("SELECT DISTINCT groupId, serviceProvider FROM BworksCdr where startTime > '");
            string rCmdString = startDate.ToString() + "' AND startTime < '" + endDate.ToString() + "' AND answerIndicator='Yes' ORDER BY serviceProvider";
            cmdStr.Append(rCmdString);

            DataSet mySet = new DataSet();

            try
            {
                mySet = GetData(cmdStr.ToString());

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in mySet.Tables)
                {
                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        String g = myRow.ItemArray[0].ToString().Trim();
                        if ( !g.Equals("780086") )
                            groupId.Add(g);
                    }
                }

            }
            catch (System.Exception e)
            {
                LogFileMgr.Instance.WriteToLogFile("CiberDbMgr::GetGroupIds():FailedTryingToGetTheGroupIdsInTheDB");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }
 
            return groupId;
        }

        public List<Subscriber> GetUsersInGroup(string groupId, DateTime startDate, DateTime endDate)
        {

            List<Subscriber> subList = new List<Subscriber>();  // list of users

            StringBuilder cmdStr = new StringBuilder("SELECT DISTINCT userNumber, groupId, serviceProvider FROM BworksCdr where startTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            string order = "' AND startTime < '" + endDate.ToString() + "' AND groupId = '" + groupId + "' AND answerIndicator='Yes' ORDER BY userNumber DESC";
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

        public List<string> GetServiceProviderIds(DateTime startDate, DateTime endDate)
        {
            List<string> spIds = new List<string>();

            StringBuilder cmdStr = new StringBuilder("SELECT DISTINCT serviceProvider FROM BworksCdr where startTime > '");
            string rCmdString = startDate.ToString() + "' AND startTime < '" + endDate.ToString() + "' AND answerIndicator='Yes' ORDER BY serviceProvider";
            cmdStr.Append(rCmdString);

            DataSet mySet = new DataSet();

            try
            {
                mySet = GetData(cmdStr.ToString());

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in mySet.Tables)
                {
                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        String g = myRow.ItemArray[0].ToString().Trim();
                        spIds.Add(g);
                    }
                }

            }
            catch (System.Exception e)
            {
                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetServiceProviderIds():GetServiceProviderIds");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            return spIds;
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

        public string GetExtension(string userNumber)
        {

            string ext = String.Empty;
            string num = String.Empty;

            if (userNumber.Trim().Contains("+1"))
                num = userNumber.Trim().Substring(2, 10);
            else if (userNumber.Trim().Contains("1"))
                num = userNumber.Trim().Substring(1, 10);
            else
                num = userNumber.Trim() ;
               
            StringBuilder cmdStr = new StringBuilder("SELECT EXTENSION FROM Extensions where USERNUMBER = '" + num + "'" );

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
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("CdrDbProcessor::GetExtension():FailedTryingToGetTheExtension");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return ext;

        }


    }// class

} // namespace

