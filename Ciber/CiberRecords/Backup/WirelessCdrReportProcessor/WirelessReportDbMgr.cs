using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;

using Strata8.Wireless.Utils;

namespace Strata8.Wireless.Cdr.Reporting
{
    public class WirelessReportDbMgr
    {
        public DataSet GetOmcCdrs(DateTime startTime, DateTime endTime)
        {
            DataSet mySet = new DataSet();
            StringBuilder cmdStr = new StringBuilder("SELECT * from OmcCdr where");

            // line added for Brad McManus for his pico with cellid = 17
            cmdStr.Append(" (aPartyDigits <> '#777') AND (cellid = '17') AND ( seizeTime > '");
            cmdStr.Append(startTime.ToString() + "') AND ( DISCTIME < '" + endTime.ToString() + "')");

            //cmdStr.Append(" (aPartyDigits <> '#777') AND (cellid like '16%') AND ( seizeTime > '");
            //cmdStr.Append(startTime.ToString() + "') AND ( DISCTIME < '" + endTime.ToString() + "')");
            //cmdStr.Append(" OR (aPartyDigits <> '#777') AND (cellid like '171%') AND ( seizeTime > '");
            //cmdStr.Append(startTime.ToString() + "') AND ( DISCTIME < '" + endTime.ToString() + "')");

            //cmdStr.Append(" OR (aPartyDigits <> '#777') AND (bcellid like '16%') AND ( seizeTime > '");
            //cmdStr.Append(startTime.ToString() + "') AND ( DISCTIME < '" + endTime.ToString() + "')");
            //cmdStr.Append(" OR (aPartyDigits <> '#777') AND (bcellid like '171%') AND ( seizeTime > '");
            //cmdStr.Append(startTime.ToString() + "') AND ( DISCTIME < '" + endTime.ToString() + "')");

            cmdStr.Append(" ORDER BY SEQNUMBER DESC");


            SqlDataAdapter rDapter = null;
            SqlConnection dataConnection = null;

            try
            {
                // make the connection
                OpenDataConn(ref dataConnection);

                // execute and fill
                rDapter = new SqlDataAdapter(cmdStr.ToString(), dataConnection);
                rDapter.Fill(mySet);

            }
            catch (System.Exception e)
            {
                this.LogError("WirelessReportDbMgr::GetOmcCdrs():" + e.Message);
            }

            CloseDataConn(ref dataConnection);

            return mySet;

        }//CheckDbBeforeDownloadingFile

        private void LogError(string msg)
        {
            WirelessCdrProcessor.WriteToLogFile(msg);

        }// public void LogFileError(string msg)

        private void OpenDataConn(ref SqlConnection dataConnection)
        {
            try
            {
                dataConnection = new SqlConnection();
                dataConnection.ConnectionString = ConfigurationManager.AppSettings["WirelessCdrSQLConnectString"];
                dataConnection.Open();

            }// try 
            catch (Exception e)
            {
                this.LogError("WirelessReportDbMgr::OpenDataConn():" + e.Message);
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
                this.LogError("WirelessReportDbMgr::CloseDataConn():" + e.Message );
            }// catch

        }// public void CloseDataConn( ref SqlConnection dataConnection )
    }
}
