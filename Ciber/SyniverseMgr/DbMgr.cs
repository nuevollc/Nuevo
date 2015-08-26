using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;
using Strata8.Wireless.Utils;

namespace EPCS.SyniverseMgr.Db
{
    public class DbMgr
    {


        private static string _connectionString = String.Empty;

        /// <summary>
        /// class to handle the database parameters related to the CIBER file and 
        /// the batch sequence numbers.  Every batch submitted needs to have the next batch
        /// sequence number for Syniverse to process these.  The batch file also needs to be
        /// incremented to make error handling/tracking easier for Strata8 to resolve
        /// </summary>
        /// 
        public DbMgr()
        {
            _connectionString = ConfigurationManager.AppSettings["WirelessTdsSQLConnectString"]; 
        }

        public void LogError(string msg)
        {
            FileWriter.Instance.WriteToLogFile(msg);
            
        }// public void LogFileError(string msg)

        /// <summary>
        /// method to get the latest sequence number
        /// **NOTE:  AND if the sequence number does not exist for the home carrier, it will add one.
        /// </summary>
        /// <param name="msid">The MSID of the subscriber</param>
        /// <returns></returns>
        public int GetSequenceNumber(string homeCarrierSidBid, string servingCarrierSidBid)
        {
            int seqNumber = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append( "SELECT sequenceNumber from HomeCarrierSidBidFileInfo where homeCarrierSidBid = '" );
            sb.Append( homeCarrierSidBid + "'" );
            sb.Append( "AND servingCarrierSidBid = '" );
            sb.Append( servingCarrierSidBid + "'" );

            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    dataConnection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
                    myDaptor.Fill(ds);

                    // make sure there is only one SID/BID returned here
                    foreach (DataTable myTable in ds.Tables)
                    {
                        if (myTable.Rows.Count.Equals(0))
                        {// log our new add and add the combo to the database
                            FileWriter.Instance.WriteToLogFile("DbMgr::GetSequenceNumber():NORowsForSequenceNumberDetectedForHomeCarrier:" + homeCarrierSidBid + " ServingCarrier:" + servingCarrierSidBid);
                        }
                        else
                        {
                            // only one row with the sidbid
                            foreach (DataRow myRow in myTable.Rows)
                            {
                                seqNumber = (int)myRow.ItemArray[0];
                            }
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                FileWriter.Instance.WriteToLogFile("CiberDbMgr::GetSequenceNumber():FailedTryingToGetSequenceNumberInTheDB");
                FileWriter.Instance.WriteToLogFile("CiberDbMgr::GetSequenceNumber():ECaught:" + ex.Message + ex.StackTrace);
            }

            return seqNumber;

        } // GetSequenceNumber


  
        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateSequenceNumber(string homeCarrierSidBid, string servingCarrierSidBid, string seqNum )
        {
            try
            {
                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    dataConnection.Open();
                    StringBuilder cmdStr = new StringBuilder("UPDATE HomeCarrierSidBidFileInfo SET sequenceNumber = ");

                    cmdStr.Append(seqNum + " WHERE homeCarrierSidBid = '");
                    cmdStr.Append(homeCarrierSidBid + "'");
                    cmdStr.Append("AND servingCarrierSidBid = '");
                    cmdStr.Append(servingCarrierSidBid + "'");

                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (System.Exception e)
            {
                FileWriter.Instance.WriteToLogFile("DbMgr::UpdateSequenceNumber():FailedTryingToUpdateSequenceNumberInTheDB");
                FileWriter.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            FileWriter.Instance.WriteToLogFile("DbMgr::UpdateSequenceNumber():UpdateSequenceNumberInTheDBFor:" + homeCarrierSidBid +":"+servingCarrierSidBid+":"+seqNum);

        }// UpdateSequenceNumber()

        public void AddSequenceNumber (string homeCarrierSidBid, string servingCarrierSidBid, string seqNum )
        {
            StringBuilder commandStr = new StringBuilder("INSERT INTO HomeCarrierSidBidFileInfo ");
            commandStr.Append("(ssidHsid, servingCarrierSidBid, homeCarrierSidBid, sequenceNumber ) VALUES(");

            try
            {
                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    // add try catch block around setting up the command            
                    StringBuilder sb = new StringBuilder();

                    // add the data to the string
                    string hc = homeCarrierSidBid.PadLeft(5, Convert.ToChar('0'));
                    string sc = servingCarrierSidBid.PadLeft(5, Convert.ToChar('0'));

                    sb.Append("'" + sc + hc + "'");
                    sb.Append(",'" + sc + "'");
                    sb.Append(",'" + hc + "'");
                    sb.Append(",'" + seqNum + "'" + ")");

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
                        FileWriter.Instance.WriteToLogFile("DbMgr::AddNewHomeCarrierSequenceNumber():ECaught:" + ex.Message + ex.StackTrace);
                    }
                }

            }//try
            catch (Exception ex)
            {
                FileWriter.Instance.WriteToLogFile("DbMgr::AddNewHomeCarrierSequenceNumber():ECaught:" + ex.Message + ex.StackTrace);
            }

        }
 

    }//class


}//namespace
