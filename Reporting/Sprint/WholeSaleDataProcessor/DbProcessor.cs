﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Diagnostics;
using System.Configuration;

using TruMobility.Utils.Logging;
using TruMobility.Reporting.Sprint;

namespace TruMobility.Reporting.Sprint
{
    /// <summary>
    /// class interface to the database for Sprint Wholesale data processing
    /// </summary>
    class DbProcessor
    {

        private string m_connectionString = ConfigurationManager.AppSettings["Sprint_SQLConnectString"];

        /// <summary>
        /// method to get a list of unique users in the Sprint data
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsers()
        {
             
            List<string> phoneList = new List<string>();  // list of users

            StringBuilder cmdStr = new StringBuilder("SELECT DISTINCT billAccessNumber FROM WholesaleUsage");
            string order = " ORDER BY billAccessNumber DESC";
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

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetUsers():FailedTryingToGetTheListOfUsers");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return phoneList;

        }


        /// <summary>
        /// "Method to get SMS usage for all users
        /// </summary>
        /// <param name="subsystemName"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public Hashtable GetSMSUsage()
        {
            Hashtable sms = new Hashtable(); 
            StringBuilder cmdStr = new StringBuilder(" SELECT  billAccessNumber, COUNT(*) AS SMSCount FROM WholesaleUsage");
            cmdStr.Append(" WHERE (usageTypeCD = 'M') ");
            cmdStr.Append(" GROUP BY billAccessNumber ORDER by billAccessNumber");

            DataSet ds = new DataSet();

            try
            {
                ds = GetData(cmdStr.ToString());

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    //r.count = myTable.Rows.Count.ToString();

                    // add the data for each user 
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        sms.Add( (String)myRow.ItemArray[0] ,  (int)myRow.ItemArray[1] );
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetSMSCount():FailedTryingToGetTheSMSCount");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return sms;

        }  //GetSMSUsage

        /// <summary>
        /// "Method to get the voice usage for all users
        /// </summary>
        /// <param name="subsystemName"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public Hashtable GetVoiceUsage()
        {

            Hashtable voice = new Hashtable();
            StringBuilder cmdStr = new StringBuilder(" SELECT  billAccessNumber, SUM(roundedTollElaspsedTime) AS VoiceTotal FROM WholesaleUsage");
            cmdStr.Append(" WHERE (usageTypeCD = 'V') ");
            cmdStr.Append(" GROUP BY billAccessNumber ORDER by billAccessNumber");

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
                        voice.Add((String)myRow.ItemArray[0], (int)myRow.ItemArray[1]);
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetSMSCount():FailedTryingToGetTheSMSCount");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return voice;

        }  //GetSMSUsage


        /// <summary>
        /// "Method to get data usage for all users
        /// </summary>
        /// <param name="subsystemName"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public Hashtable GetDataUsage()
        {

            Hashtable data = new Hashtable();
            StringBuilder cmdStr = new StringBuilder(" SELECT  billAccessNumber, SUM(rpdrTotalUsageQty) AS DataTotal FROM WholesaleUsage");
            cmdStr.Append(" WHERE (usageTypeCD = 'P') ");
            cmdStr.Append(" GROUP BY billAccessNumber ORDER by billAccessNumber");

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
                        data.Add((String)myRow.ItemArray[0], (int)myRow.ItemArray[1]);
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetSMSCount():FailedTryingToGetTheSMSCount");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
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

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetData():FailedCommand" + cmdStr);
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return ds;

        }

    }
}
