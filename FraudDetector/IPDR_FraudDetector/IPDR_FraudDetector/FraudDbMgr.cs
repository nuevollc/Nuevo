using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using TruMobility.Utils.Logging;

namespace TruMobility.Services.Fraud
{
    class FraudDbMgr
    {
        private static string m_connectionString = ConfigurationManager.AppSettings["SprintMVNODataSQLConnectString"];
        private static int m_timeDelay = Convert.ToInt32(ConfigurationManager.AppSettings["HourDelay"]); 

        /// <summary>
        /// method to get a list of unique users in the Sprint data
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsers()
        {
             
            List<string> rUsers = new List<string>();  // list of users

            StringBuilder cmdStr = new StringBuilder("SELECT MDN FROM Users");
            string order = " ORDER BY MDN DESC";
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
                        rUsers.Add((String)myRow.ItemArray[0]);
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetUsers():FailedTryingToGetTheListOfUsers");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return rUsers;

        }

        /// <summary>
        /// "Method to get data usage for all users
        /// </summary>
        /// <param name="subsystemName"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public Hashtable GetDataUsage()
        {

            // get the local time
            DateTime timeNow = DateTime.Now;
            // go back 2 hours for now
            TimeSpan tminus = new TimeSpan(m_timeDelay, 0, 0);
            DateTime dt = timeNow.Subtract(tminus);

            string theDate = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString();

            Hashtable data = new Hashtable();
            StringBuilder cmdStr = new StringBuilder(" SELECT  userName, SUM(DeltaAcctInputOctets) AS DataInTotal, ");
            cmdStr.Append(" SUM(DeltaAcctOutputOctets) AS DataOutTotal FROM SprintAMSRecord");
            cmdStr.Append(" WHERE (AccountStatus = '2')  AND EventTimestamp > '" + theDate + "'");
            cmdStr.Append(" GROUP BY userName ORDER by userName");

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
                        AMSData d = new AMSData();
                        d.User = (String)myRow.ItemArray[0];
                        d.DataIn = (int)myRow.ItemArray[1];
                        d.DataOut = (int)myRow.ItemArray[2];

                        data.Add(d.User, d);
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogMsg("FraudDbManager::GetDataUsage():FailedTryingToGetTheDataUsage");
                LogMsg("ECaught:" + ex.Message );
            }

            return data;

        }  //GetSMSUsage

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
                using (SqlConnection connection = new SqlConnection(FraudDbMgr.m_connectionString))
                {
                    connection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr, FraudDbMgr.m_connectionString);
                    myDaptor.Fill(ds);
                }

            }

            catch (System.Exception ex)
            {

                LogMsg("DbProcessor::GetData():FailedCommand" + cmdStr);
                LogMsg("ECaught:" + ex.Message);
            }

            return ds;

        }
        public void LogMsg( string msg )
        {
            LogFileMgr.Instance.WriteToLogFile( msg );
        }

    }
}
