using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;
using Strata8.Wireless.Utils;

namespace Strata8.Wireless.Db
{
    public class CiberDbMgr
    {

        /// <summary>
        /// class to handle the database parameters related to the CIBER file and 
        /// the batch sequence numbers.  Every batch submitted needs to have the next batch
        /// sequence number for Syniverse to process these.  The batch file also needs to be
        /// incremented to make error handling/tracking easier for Strata8 to resolve
        /// </summary>
        public CiberDbMgr()
        { }

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateFileNumber()
        {
            int fileNum = this.GetFileNumber();
            fileNum++;

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("UPDATE CiberFileInfo SET fileNumber = ");

                cmdStr.Append(fileNum + " WHERE id = 1");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                FileWriter.Instance.WriteToLogFile("CiberDbMgr::UpdateFileNumber():FailedTryingToUpdateTheFileNameInTheDB");
                FileWriter.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// UpdateDateDownloaded()

        public void LogError(string msg)
        {
            FileWriter.Instance.WriteToLogFile(msg);
            
        }// public void LogFileError(string msg)

        private void OpenDataConn(ref SqlConnection dataConnection)
        {
            try
            {
                dataConnection = new SqlConnection();
                dataConnection.ConnectionString = ConfigurationManager.AppSettings["WirelessTdsSQLConnectString"];
                dataConnection.Open();

            }// try 
            catch (Exception e)
            {
                // EventLog.WriteEntry(m_eventLogName, "InventoryReportParser Service FAILED trying to get a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
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
                //EventLog.WriteEntry(m_eventLogName, "InventoryReportParser Service FAILED trying to close a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch

        }// public void CloseDataConn( ref SqlConnection dataConnection )

        // delete this when ready
        ///// <summary>
        ///// method to get the latest sequence number
        ///// 
        ///// </summary>
        ///// <param name="msid">The MSID of the subscriber</param>
        ///// <returns></returns>
        //public int GetSequenceNumber( string homeCarrierSidBid, string servingCarrierSidBid )
        //{
        //    int seqNumber =0;

        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT sequenceNumber from CiberFileInfo where id = 1");

        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        // make the connection
        //        SqlConnection dataConnection = null;
        //        OpenDataConn(ref dataConnection);

        //        // execute and fill
        //        SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
        //        myDaptor.Fill(ds);

        //        // if we have too many SID/BIDS or none at all it is not a Verizon customer
        //        // no need to create the CIBER record for this guy
        //        if (ds.Tables.Count > 1)
        //            return seqNumber;
        //        else if (ds.Tables.Count == 0)
        //            return seqNumber;

        //        // make sure there is only one SID/BID returned here
        //        foreach (DataTable myTable in ds.Tables)
        //        {
        //            //r.count = myTable.Rows.Count.ToString();

        //            // only one row with the sidbid
        //            foreach (DataRow myRow in myTable.Rows)
        //            {
        //                //foreach (DataColumn myColumn in myTable.Columns)
        //                //{
        //                //    Console.WriteLine(myRow[myColumn]);
        //                //}
        //                seqNumber = (int) myRow.ItemArray[0];
        //            }
        //        }

        //        CloseDataConn(ref dataConnection);

        //    }
        //    catch (System.Exception ex)
        //    {
        //        // log the error, what is our logging policy?
        //    }

        //    return seqNumber;
        //}

        /// <summary>
        /// method to get the latest sequence number
        /// 
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
                // make the connection
                SqlConnection dataConnection = null;
                OpenDataConn(ref dataConnection);

                // execute and fill
                SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
                myDaptor.Fill(ds);

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                {
                     FileWriter.Instance.WriteToLogFile("CiberDbMgr::GetSequenceNumber():MultipleSeqNumbersDetected:"+homeCarrierSidBid+" ServingCarrier:"+servingCarrierSidBid );
                    return seqNumber;
                }
                else if (ds.Tables.Count == 0)
                {
                    // log our new add and add the combo to the database
                    FileWriter.Instance.WriteToLogFile("CiberDbMgr::GetSequenceNumber():NOTablesForSequenceNumberDetectedForHomeCarrier:"+homeCarrierSidBid+" ServingCarrier:"+servingCarrierSidBid );
                    AddNewHomeCarrierSequenceNumber(homeCarrierSidBid, servingCarrierSidBid);
                    seqNumber++;
                    return seqNumber;
                }
                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    if (myTable.Rows.Count.Equals(0))
                    {// log our new add and add the combo to the database
                        FileWriter.Instance.WriteToLogFile("CiberDbMgr::GetSequenceNumber():NORowsForSequenceNumberDetectedForHomeCarrier:" + homeCarrierSidBid + " ServingCarrier:" + servingCarrierSidBid);
                        AddNewHomeCarrierSequenceNumber(homeCarrierSidBid, servingCarrierSidBid);
                        seqNumber++;
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

                CloseDataConn(ref dataConnection);

            }
            catch (System.Exception ex)
            {
                FileWriter.Instance.WriteToLogFile("CiberDbMgr::GetSequenceNumber():FailedTryingToGetSequenceNumberInTheDB");
                FileWriter.Instance.WriteToLogFile("CiberDbMgr::GetSequenceNumber():ECaught:" + ex.Message + ex.StackTrace);
            }

            // add logic to check to make sure it is not 999
            // if it is roll it over to 001.
            return seqNumber;
        } // GetSequenceNumber


        private void AddNewHomeCarrierSequenceNumber(string homeCarrierSidBid, string servingCarrierSidBid)
        {
            int seqNum = 1; // we set the sequence number to 1 for the first batch being sent

            StringBuilder commandStr = new StringBuilder("INSERT INTO HomeCarrierSidBidFileInfo ");                
            commandStr.Append("(ssidHsid, servingCarrierSidBid, homeCarrierSidBid, sequenceNumber ) VALUES(");

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                    // add try catch block around setting up the command            
                    StringBuilder sb = new StringBuilder();

                    // add the data to the string
                    string hc = homeCarrierSidBid.PadLeft(5,Convert.ToChar('0'));
                    string sc = servingCarrierSidBid.PadLeft(5,Convert.ToChar('0'));

                    sb.Append("'" + sc + hc  + "'");
                    sb.Append(",'" + sc + "'");
                    sb.Append(",'" + hc + "'");
                    sb.Append(",'" + seqNum.ToString() + "'" + ")");

                    // write the CDR to the database
                    string tc = commandStr.ToString() + sb.ToString();
                    try
                    {

                        SqlCommand sqlCommand = new SqlCommand(tc, dataConnection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch( Exception ex )
                    {
                        FileWriter.Instance.WriteToLogFile("CiberDbMgr::AddNewHomeCarrierSequenceNumber():ECaught:" + ex.Message + ex.StackTrace);
                    }

            }//try
            catch (Exception ex)
            {
                FileWriter.Instance.WriteToLogFile("CiberDbMgr::AddNewHomeCarrierSequenceNumber():ECaught:" + ex.Message + ex.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateSequenceNumber(string homeCarrierSidBid, string servingCarrierSidBid)
        {
            int seqNumber = this.GetSequenceNumber(homeCarrierSidBid, servingCarrierSidBid);
            seqNumber++;

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("UPDATE HomeCarrierSidBidFileInfo SET sequenceNumber = ");

                cmdStr.Append(seqNumber + " WHERE homeCarrierSidBid = '");
                cmdStr.Append(homeCarrierSidBid + "'");
                cmdStr.Append("AND servingCarrierSidBid = '");
                cmdStr.Append(servingCarrierSidBid + "'");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                FileWriter.Instance.WriteToLogFile("CiberDbMgr::UpdateSequenceNumber():FailedTryingToUpdateSequenceNumberInTheDB");
                FileWriter.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// UpdateSequenceNumber()

        /// <summary>
        /// method to get the latest sequence number
        /// 
        /// </summary>
        /// <param name="msid">The MSID of the subscriber</param>
        /// <returns></returns>
        public int GetFileNumber()
        {
            int fileNum = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT fileNumber from CiberFileInfo where id = 1");

            DataSet ds = new DataSet();

            try
            {
                // make the connection
                SqlConnection dataConnection = null;
                OpenDataConn(ref dataConnection);

                // execute and fill
                SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
                myDaptor.Fill(ds);

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                    return fileNum;
                else if (ds.Tables.Count == 0)
                    return fileNum;

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    //r.count = myTable.Rows.Count.ToString();

                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        //foreach (DataColumn myColumn in myTable.Columns)
                        //{
                        //    Console.WriteLine(myRow[myColumn]);
                        //}
                        fileNum = (int) myRow.ItemArray[0];
                    }
                }

                CloseDataConn(ref dataConnection);
            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            return fileNum;
        }

        /// <summary>
        /// method to get the SID/BID based on the pico cellId
        /// 
        /// </summary>
        /// <param name="cellId">The cellid of the pico</param>
        /// <returns></returns>
        public string GetSidBidForCellId(string cellId)
        {
            string sidBid = String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT sidBid from SidBidCellId where cellid = '" + cellId + "'");

            DataSet ds = new DataSet();

            try
            {
                // make the connection
                SqlConnection dataConnection = null;
                OpenDataConn(ref dataConnection);

                // execute and fill
                SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
                myDaptor.Fill(ds);

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                    return sidBid;
                else if (ds.Tables.Count == 0)
                    return sidBid;

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    //r.count = myTable.Rows.Count.ToString();

                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        sidBid = myRow.ItemArray[0].ToString().Trim();
                    }
                }

                CloseDataConn(ref dataConnection);

            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            return sidBid;
        }

        public string GetSiteNameForCellId(string cellId)
        {
            string siteName = String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT siteName from SidBidCellId where cellid = '" + cellId + "'");

            DataSet ds = new DataSet();

            try
            {
                // make the connection
                SqlConnection dataConnection = null;
                OpenDataConn(ref dataConnection);

                // execute and fill
                SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
                myDaptor.Fill(ds);

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                    return siteName;
                else if (ds.Tables.Count == 0)
                    return siteName;

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        siteName = myRow.ItemArray[0].ToString().Trim();
                    }
                }

                CloseDataConn(ref dataConnection);

            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            return siteName;
        }

        /// <summary>
        /// method to get the SID/BID based on the pico cellId
        /// 
        /// </summary>
        /// <param name="cellId">The cellid of the pico</param>
        /// <returns></returns>
        public string GetUserIdForCellId(string cellId)
        {
            string userId = String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT userId from SidBidCellId where cellid = '" + cellId + "'");

            DataSet ds = new DataSet();

            try
            {
                // make the connection
                SqlConnection dataConnection = null;
                OpenDataConn(ref dataConnection);

                // execute and fill
                SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
                myDaptor.Fill(ds);

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                    return userId;
                else if (ds.Tables.Count == 0)
                    return userId;

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        userId = myRow.ItemArray[0].ToString().Trim();
                    }
                }

                CloseDataConn(ref dataConnection);

            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            return userId;
        }

    }//class


}//namespace
