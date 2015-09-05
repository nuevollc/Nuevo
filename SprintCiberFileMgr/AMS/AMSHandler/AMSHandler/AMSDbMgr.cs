using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;
using TruMobility.Utils.Logging;

namespace TruMobility.Sprint.AMS
{
    public class AMSDbMgr
    {

        private static string _connectionString = String.Empty;

        /// <summary>
        /// class to handle the database parameters related to the CIBER file and 
        /// the batch sequence numbers.  Every batch submitted needs to have the next batch
        /// sequence number for Syniverse to process these.  The batch file also needs to be
        /// incremented to make error handling/tracking easier for Strata8 to resolve
        /// </summary>
        /// 
        public AMSDbMgr()
        {
            _connectionString = ConfigurationManager.AppSettings["WirelessCDRSQLConnectString"];
        }

        private void LogError(string msg)
        {
            LogFileMgr.Instance.WriteToLogFile(msg);

        }// public void LogFileError(string msg)

        public void AddAmsRecord(AMSRecord ams )
        {
            //FileWriter.Instance.WriteToLogFile("-NFORMATIONAL::MAFDbMgr::InsertMafRecord()Entering");

            StringBuilder commandStr = new StringBuilder("INSERT INTO SprintAmsRecord ");
            commandStr.Append(" ( AccountStatus, CallingStationId, IpAddress, UserName, AccountSessionId, CorrelationId, SessionCont,");
            commandStr.Append("  HAIPAddress, NASIPAddress, PCFIPAddress, BSID, UserZone, FMUX, RMUX, ServiceOption, Ftype, Rtype, Fsize, FRC, RRC, IPTech, CompFlag,");
            commandStr.Append("  ReasonInd, G3pp2FSize, IPQOS, AirQOS, AcctOutput, AcctInput, BadFrameCount, EventTimestamp, ActiveTime, NumActive, SDBInputOctet, SDBOutputOctet,");
            commandStr.Append("  SDBNumInput, SDBNumOutput, TotalBytesRecvd, SIPInboundCount, SIPOutboundCount, PDSNVendorId, AAAId, EAMSId, EAMSPreProcessTime,");
            commandStr.Append("  EAMSPostProcessTime, GMTOffset, DeltaAcctOutputOctets, DeltaAcctInputOctets,");
            commandStr.Append(" DeltaActiveTime ) VALUES(");

            try
            {
                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    // add try catch block around setting up the command            
                    StringBuilder sb = new StringBuilder();
                    //DateTime dt = new DateTime(DateTime.Now.Year, Convert.ToInt16(ams.CallDate.Substring(2, 2)),
                    //   Convert.ToInt16(ams.CallDate.Substring(4, 2)), Convert.ToInt16(ams.AirConnectTime.Substring(0, 2)),
                   //    Convert.ToInt16(ams.AirConnectTime.Substring(2, 2)), Convert.ToInt16(ams.AirConnectTime.Substring(4, 2)));

                    // for now going to log the air connect time to find out why not converting time right (AM time, 00:00:00 AM )    
                    //FileWriter.Instance.WriteToLogFile("MAFDbMgr::InsertMafRecord():CallDate:AirConnectTime:AirConnectTime(0,2):" + ams.CallDate + " : " +
                    //  ams.AirConnectTime + " : "  + ams.AirConnectTime.Substring(0, 2) + " : " + " DateTime::" + dt.ToString());

                    sb.Append("'"  + ams.AccountStatus + "'");
                    sb.Append(",'" + ams.CallingStationId + "'");
                    sb.Append(",'" + ams.IpAddress + "'");
                    sb.Append(",'" + ams.UserName + "'");
                    sb.Append(",'" + ams.AccountSessionId + "'");
                    sb.Append(",'" + ams.CorrelationId + "'");
                    sb.Append(",'" + ams.SessionCont + "'");
                    sb.Append(",'" + ams.HaIpAddress + "'");
                    sb.Append(",'" + ams.NasIpAddress + "'");
                    sb.Append(",'" + ams.PcfIpAddress + "'");
                    sb.Append(",'" + ams.BSID + "'");
                    sb.Append(",'" + ams.UserZone + "'");
                    sb.Append(",'" + ams.FMUX + "'");
                    sb.Append(",'" + ams.RMUX + "'"); 
                    sb.Append(",'" + ams.ServiceOption + "'");
                    sb.Append(",'" + ams.Ftype + "'");
                    sb.Append(",'" + ams.Rtype + "'");
                    sb.Append(",'" + ams.Fsize + "'");
                    sb.Append(",'" + ams.Frc + "'");
                    sb.Append(",'" + ams.Rrc + "'");
                    sb.Append(",'" + ams.IpTech + "'");
                    sb.Append(",'" + ams.CompFlag + "'");
                    sb.Append(",'" + ams.ReasonInd + "'");
                    sb.Append(",'" + ams.G3pp2Fsize + "'");
                    sb.Append(",'" + ams.IpQos + "'");
                    sb.Append(",'" + ams.AirQos + "'");
                    sb.Append(",'" + ams.AcctOutput + "'");
                    sb.Append(",'" + ams.AcctInput + "'");
                    sb.Append(",'" + ams.BadFrameCount + "'");
                    sb.Append(",'" + ams.EventTimeStamp + "'");
                    sb.Append(",'" + ams.ActiveTime + "'");
                    sb.Append(",'" + ams.NumActive + "'");
                    sb.Append(",'" + ams.SdbInputOctet + "'");
                    sb.Append(",'" + ams.SdbOutputOctet + "'");
                    sb.Append(",'" + ams.SdbNumInput + "'");
                    sb.Append(",'" + ams.SdbNumOutput + "'");
                    sb.Append(",'" + ams.TotalBytesReceived + "'");
                    sb.Append(",'" + ams.SipInboundCount + "'");
                    sb.Append(",'" + ams.SipOutboundCount + "'");
                    sb.Append(",'" + ams.PdsnVendorId + "'");
                    sb.Append(",'" + ams.AaaId + "'");
                    sb.Append(",'" + ams.EamsId + "'");
                    sb.Append(",'" + ams.EamsPreProcessTime + "'");
                    sb.Append(",'" + ams.EamsPostProcessTime + "'");
                    sb.Append(",'" + ams.GmtOffset + "'");
                    sb.Append(",'" + ams.DeltaAcctOutputOctets + "'");
                    sb.Append(",'" + ams.DeltaAcctInputOctets + "'");

                    sb.Append(",'" + ams.DeltaActiveTime + "'" + ")");

                    // write the CDR to the database
                    string tc = commandStr.ToString() + sb.ToString();
                    try
                    {
                        dataConnection.Open();
                        SqlCommand sqlCommand = new SqlCommand(tc, dataConnection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogError("AMSDbMgr::InsertAmsRecord():ECaught:" + ex.Message + ex.StackTrace);
                    }

                }//using


            }//try
            catch (Exception ex)
            {
                LogError("AMSDbMgr::InsertAmsRecord():ECaught:" + ex.Message + ex.StackTrace);
            }

            // FileWriter.Instance.WriteToLogFile("-NFORMATIONAL::MAFDbMgr::InsertMafRecord()Exiting");

        }

        public void AddAmsFile(string fname)
        {
            
            StringBuilder commandStr = new StringBuilder("INSERT INTO AMSFilesDownloaded ");
            commandStr.Append(" ( fileName, dateStoredInDb ");
            commandStr.Append(" ) VALUES(");

            try
            {
                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    // add try catch block around setting up the command            
                    StringBuilder sb = new StringBuilder();

                    sb.Append("'" + fname + "' , '" + DateTime.Now.ToString() + "')");

                    // write the CDR to the database
                    string tc = commandStr.ToString() + sb.ToString();
                    try
                    {
                        dataConnection.Open();
                        SqlCommand sqlCommand = new SqlCommand(tc, dataConnection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogError("AMSDbMgr::AddAmsFile():ECaught:" + ex.Message + ex.StackTrace);
                    }

                }//using


            }//try
            catch (Exception ex)
            {
                LogError("AMSDbMgr::AddAmsFile():ECaught:" + ex.Message + ex.StackTrace);
            } 

        }

        public void UpdateFileStatus(string fname)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    StringBuilder cmdStr = new StringBuilder("UPDATE AMSFilesDownloaded SET dateStoredInDb = '");

                    cmdStr.Append(DateTime.Now.ToString() + "'" + " WHERE fileName = '");
                    cmdStr.Append(fname + "'");

                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), connection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (System.Exception e)
            {
                LogError("AmsDbMgr::UpdateFileStatus():FailedTryingToUpdateTheFileNameInTheDB:" + fname);
                LogError("ECaught:" + e.Message + e.StackTrace);
            }

        }// UpdateFileStatus()

        public bool CheckIfFileDownloaded(string fileName)
        {
            bool uploaded = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    StringBuilder cmdStr = new StringBuilder("Select * from AMSFilesDownloaded WHERE fileName = '");
                    cmdStr.Append(fileName + "'");

                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr.ToString(), connection);
                    myDaptor.TableMappings.Add("AMSFilesDownloaded", "Files");

                    DataSet files = new DataSet("Files");
                    myDaptor.Fill(files, "AMSFilesDownloaded");

                    bool done = CheckData(files);
                    if (done)
                        uploaded = true;
                }
            }
            catch (System.Exception e)
            {
                LogError("AMSDbMgr::CheckIfFileDownloaded():FailedTryingToCheckTheFileNameInTheDB:" + fileName);
                LogError("ECaught:" + e.Message + e.StackTrace);
            }

            return uploaded;

        }

        private bool CheckData(DataSet dataSet)
        {

            DataTableCollection dtc = dataSet.Tables;
            DataTable dt = dtc[0];
            if (dt.Rows.Count.Equals(0))
                return false;
            else
                return true;

        } //checkData

    }//class


}//namespace

