
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;

namespace TruMobility.Services.Fraud
{
    /// <summary>
    /// this needs to be moved into our CDR DB web service interface
    /// for now it's here: rlh
    /// </summary>
    class DbInterface
    {
                    
        // get the event log name
        private string   m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["EventLogname"];

        private void OpenDataConn(ref SqlConnection dataConnection)
        {
            try
            {
                dataConnection = new SqlConnection();
                dataConnection.ConnectionString = ConfigurationManager.AppSettings["BWorksCdr_SQLConnectString"];
                dataConnection.Open();

            }// try 
            catch (Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "DbInterface::OpenDataConnection():FailedEstablishingDBConnection:" + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void OpenDataConn( ref SqlConnection dataConnection ) 

        private void CloseDataConn(ref SqlConnection dataConnection)
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
                EventLog.WriteEntry(m_eventLogName, "DbInterface::OpenDataConnection():FailedTryingToCloseTheDBConnection:" + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void CloseDataConn( ref SqlConnection dataConnection )


        /// <summary>
        /// method that simply counts the number of International Calls every 15 mins
        /// </summary>
        /// <returns></returns>
        public int CountInternationalCalls( double cdrDelayInterval )
        {

            int numInternationalCalls = 0;

            StringBuilder cmdStr = new StringBuilder("SELECT * from bworkscdr where callcategory = 'internat' and starttime > ");

            DateTime univDateTime = DateTime.Now.ToUniversalTime();
            //DateTime localTime = univDateTime.ToLocalTime();


            // we subtract 30 mins from our GMT time; cdrs are all GMT
            // ignoring userTimeZone offset for now, may change later
            // and our collection interval is 15 mins + delay so we doing 30
            DateTime reportTime = univDateTime.AddMinutes( cdrDelayInterval );

            DataSet mySet = new DataSet();
            SqlDataAdapter rDapter = null;
            SqlConnection dataConnection = null;

            try
            {
                // make the connection
                OpenDataConn(ref dataConnection);

                cmdStr.Append("'" + reportTime + "'");
                // execute and fill
                rDapter = new SqlDataAdapter(cmdStr.ToString(), dataConnection);
                rDapter.Fill(mySet);

                // have one table
                DataTable theTable = mySet.Tables[0];
                DataRow[] currentRows = theTable.Select(
                    null, null, DataViewRowState.CurrentRows);

                if (currentRows.Length < 1)
                {
                    Console.WriteLine("No Current Rows Found");
                }
                else
                {
                    numInternationalCalls = currentRows.Length;
                }

            }
            catch (System.Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "DbInterface::UpdateFtpFileNameInDb():FailedTryingToUpdateTheFileNameInTheDB:" + e.ToString(), EventLogEntryType.Error, 3012);
            }

            CloseDataConn(ref dataConnection);

            return numInternationalCalls;

        }//CountInternationalCalls


        /// <summary>
        /// method that simply counts the number of ALL Calls every CDRDelayInterval mins
        /// </summary>
        /// <returns></returns>
        public int CountAllCalls(double cdrDelayInterval)
        {

            int numCalls = 0;

            StringBuilder cmdStr = new StringBuilder("SELECT * from bworkscdr where starttime > ");

            DateTime univDateTime = DateTime.Now.ToUniversalTime();
            // we subtract 30 mins from our GMT time; cdrs are all GMT
            // ignoring userTimeZone offset for now, may change later
            // and our collection interval is 15 mins + delay so we doing 30
            DateTime reportTime = univDateTime.AddMinutes(cdrDelayInterval);

            DataSet mySet = new DataSet();
            SqlDataAdapter rDapter = null;
            SqlConnection dataConnection = null;

            try
            {
                // make the connection
                OpenDataConn(ref dataConnection);

                cmdStr.Append("'" + reportTime + "'");
                // execute and fill
                rDapter = new SqlDataAdapter(cmdStr.ToString(), dataConnection);
                rDapter.Fill(mySet);

                // have one table
                DataTable theTable = mySet.Tables[0];
                DataRow[] currentRows = theTable.Select(
                    null, null, DataViewRowState.CurrentRows);

                if (currentRows.Length < 1)
                {
                    Console.WriteLine("No Current Rows Found");
                }
                else
                {
                    numCalls = currentRows.Length;
                }

            }
            catch (System.Exception e)
            {
                EventLog.WriteEntry(m_eventLogName, "DbInterface::UpdateFtpFileNameInDb():FailedTryingToUpdateTheFileNameInTheDB:" + e.ToString(), EventLogEntryType.Error, 3012);
            }

            CloseDataConn(ref dataConnection);

            return numCalls;

        }//CountAllCalls
    }
}
