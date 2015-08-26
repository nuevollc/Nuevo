using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;

namespace Strata8.Wireless.Cdr
{
    public class OmcCdrDb
    {
        // our CDR list to load into the database
        List<OmcCdr> techDataList = new List<OmcCdr>();
        private static DateTime NullDateTime = new DateTime(1977, 12, 25);

        /// <summary>
        /// Method to store a List of CDRs in the database
        /// </summary>
        /// <param name="cdrList"></param>
        public void StoreCdrInDb( List<OmcCdr> cdrList )
        {
            StringBuilder commandStr = new StringBuilder("INSERT INTO omcCdr ");                
            commandStr.Append("(seqNumber, version, recordType, aPartyNum, bPartyNum, aPartyType, bPartyType, aPartyDigits, bPartyDigits,");
            commandStr.Append("aPartyTrunk, bPartyTrunk, aPartyTrkGrp, bPartyTrkGrp, seizeTime, answerTime, discTime, discCode, discReason,");
            commandStr.Append("mscId, origEsn, termEsn, cellId, bCellId, aFeatureBits, bFeatureBits, oMsisdn, tMsisdn, oExchange,tExchange,");
            commandStr.Append("oMarketId, oSwno, oBillingId, tMarketId, tSwno, tBillingId, oBillingDigits, tBillingDigits, oServiceId, tServiceId,");
            commandStr.Append("crgChargeInfo, ocpn, icprn ) VALUES(");


            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                foreach (OmcCdr cdr in cdrList)
                {
                    // add try catch block around setting up the command            
                    StringBuilder sb = new StringBuilder();

                    // add the data to the string
                    sb.Append("'" + cdr.SequenceNumber + "'");
                    sb.Append(",'" + cdr.Version + "'");
                    sb.Append(",'" + cdr.Type + "'");
                    sb.Append(",'" + cdr.A_Party_Num + "'");
                    sb.Append(",'" + cdr.B_Party_Num + "'");
                    sb.Append(",'" + cdr.A_Party_Type + "'");
                    sb.Append(",'" + cdr.B_Party_Type + "'");
                    sb.Append(",'" + cdr.A_Party_Digits + "'");
                    sb.Append(",'" + cdr.B_Party_Digits + "'");
                    sb.Append(",'" + cdr.A_Party_Trunk + "'");
                    sb.Append(",'" + cdr.B_Party_Trunk + "'");
                    sb.Append(",'" + cdr.A_Party_TrunkGroup + "'");
                    sb.Append(",'" + cdr.B_Party_TrunkGroup + "'");
                    sb.Append(",'" + cdr.SeizeTime + "'");
                    sb.Append(",'" + cdr.AnswerTime + "'");
                    sb.Append(",'" + cdr.DisconnectTime + "'");
                    sb.Append(",'" + cdr.DisconnectCode + "'");
                    sb.Append(",'" + cdr.DisconnectReason + "'");
                    sb.Append(",'" + cdr.MscId + "'");
                    sb.Append(",'" + cdr.OriginatingEsn + "'");
                    sb.Append(",'" + cdr.TerminatingEsn + "'");
                    sb.Append(",'" + cdr.CellId + "'");
                    sb.Append(",'" + cdr.B_CellId + "'");
                    sb.Append(",'" + cdr.A_Feature_Bits + "'");
                    sb.Append(",'" + cdr.B_Feature_Bits + "'");
                    sb.Append(",'" + cdr.OriginatingMsisdn + "'");
                    sb.Append(",'" + cdr.TerminatingMsisdn + "'");
                    sb.Append(",'" + cdr.OriginatingExchange + "'");
                    sb.Append(",'" + cdr.TerminatingExchange + "'");

                    sb.Append(",'" + cdr.OriginatingMarketId + "'");
                    sb.Append(",'" + cdr.OriginatingSwno + "'");
                    sb.Append(",'" + cdr.OriginatingBillingId + "'");

                    sb.Append(",'" + cdr.TerminatingMarketId + "'");
                    sb.Append(",'" + cdr.TerminatingSwno + "'");
                    sb.Append(",'" + cdr.TerminatingBillingId + "'");

                    sb.Append(",'" + cdr.OriginatingBillingDigits + "'");
                    sb.Append(",'" + cdr.TerminatingBillingDigits + "'");

                    sb.Append(",'" + cdr.OriginatingServiceId + "'");
                    sb.Append(",'" + cdr.TerminatingServiceId + "'");
                    sb.Append(",'" + cdr.CRG_Charge_Info + "'");
                    sb.Append(",'" + cdr.Ocpn+ "'");

                    sb.Append(",'" + cdr.Icprn + "'" + ")");

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
                        OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::StoreCdrInDb():ECaught:" + ex.Message + ex.StackTrace);
                    }

                }
            }//try
            catch (Exception ex)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::StoreCdrInDb():ECaught:" + ex.Message + ex.StackTrace);
            }

            this.CloseDataConn(ref dataConnection);

        }

        public OmcCdrFileInfo GetOmcCdrInfo(string fileName)
        {

            StringBuilder cmdStr = new StringBuilder("SELECT * from FilesDownloaded where fileName =");
            DataSet ds = new DataSet();
            SqlDataAdapter rDapter = null;
            SqlConnection dataConnection = null;

            OmcCdrFileInfo rInfo = new OmcCdrFileInfo();

            try
            {
                // make the connection
                OpenDataConn(ref dataConnection);

                cmdStr.Append("'" + fileName + "'");
                // execute and fill
                rDapter = new SqlDataAdapter(cmdStr.ToString(), dataConnection);
                rDapter.Fill(ds);

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    //r.count = myTable.Rows.Count.ToString();

                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        rInfo.FileName =  myRow.ItemArray[0].ToString();
                        rInfo.Downloaded = (byte) myRow.ItemArray[1];

                        if (!myRow.ItemArray[2].Equals(System.DBNull.Value))
                            rInfo.DateDownloaded = (DateTime)myRow.ItemArray[2];
                        else
                            rInfo.DateDownloaded = NullDateTime;
                        
                        rInfo.StoredInDb = (byte) myRow.ItemArray[3];
                        if (!myRow.ItemArray[4].Equals(System.DBNull.Value))
                            rInfo.DateStoredInDb = (DateTime)myRow.ItemArray[4];
                        else
                            rInfo.DateStoredInDb = NullDateTime;

                        rInfo.CiberCreated = (byte) myRow.ItemArray[5];
                        if ( !myRow.ItemArray[6].Equals(System.DBNull.Value ) )
                            rInfo.DateCiberCreated = (DateTime) myRow.ItemArray[6];
                        else
                            rInfo.DateCiberCreated = NullDateTime;
                        
                        rInfo.FileMerged = (byte)myRow.ItemArray[7];
                        if (!myRow.ItemArray[8].Equals(System.DBNull.Value))
                            rInfo.DateFileMerged = (DateTime)myRow.ItemArray[8];
                        else
                            rInfo.DateFileMerged = NullDateTime;
                    }
                }
                
                // shut it down
                CloseDataConn( ref dataConnection);

            }
            catch (Exception ex)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile( "OmcCdrDb::GetOmcCdrInfo():ECaught:" + ex.Message + "\r\n" + ex.StackTrace );
            }
            return rInfo;

        }

        public bool CheckDbForFileDownloaded(string fileName)
        {
            bool fileDownloaded = false;

            try
            {
                OmcCdrFileInfo ri = GetOmcCdrInfo(fileName);
                if ( ri.Downloaded.Equals(0) )
                    fileDownloaded = false;
                else
                    fileDownloaded = true;

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::CheckDbForFileDownloaded():FailedTryingToGetTheFileNameInTheDB:" + fileName + e.Message + "\r\n" + e.StackTrace);
            }


            return fileDownloaded;

        }//CheckDbBeforeDownloadingFile

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateDateFileDownloaded(string fileName)
        {
            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("UPDATE FilesDownloaded SET downloaded = 1, dateDownloaded = '");

                cmdStr.Append(DateTime.Now.ToString() + "'" + " WHERE fileName = '");
                cmdStr.Append(fileName + "'");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::UpdateDateFileDownloaded():FailedTryingToUpdateTheFileNameInTheDB:" + fileName);
                OmcCdrFileWriter.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// UpdateDateDownloaded()

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateDateFileMerged(string fileName)
        {
            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("UPDATE FilesDownloaded SET fileMerged = 1, dateFileMerged = '");

                cmdStr.Append(DateTime.Now.ToString() + "'" + " WHERE fileName = '");
                cmdStr.Append(fileName + "'");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::UpdateDateFileDownloaded():FailedTryingToUpdateTheFileNameInTheDB:" + fileName);
                OmcCdrFileWriter.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// UpdateDateDownloaded()

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateDateOmcCdrsStored( string fileName )
        {
            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("UPDATE FilesDownloaded SET storedInDb = 1, dateStoredInDb = '");

                cmdStr.Append(DateTime.Now.ToString() + "'" + " WHERE fileName = '");
                cmdStr.Append(fileName + "'");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::UpdateDateOmcCdrsStored():FailedTryingToUpdateTheFileNameInTheDB:" + fileName + e.Message);
            }

            CloseDataConn(ref dataConnection);

        }// UpdateDateDownloaded()   
  
        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateDateCiberRecordsCreated(string fileName)
        {
            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("UPDATE FilesDownloaded SET ciberCreated = 1, dateCiberCreated = '");

                cmdStr.Append(DateTime.Now.ToString() + "'" + " WHERE fileName = '");
                cmdStr.Append(fileName + "'");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::UpdateDateCiberRecordsCreated():FailedTryingToUpdateTheFileNameInTheDB:" + fileName + e.Message + "\r\n" + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// UpdateDateDownloaded()   

        /// <summary>
        /// method to add the fileinfo to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">fileinfo object</param>
        public void AddDateDownloaded( string fileName )
        {
            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("INSERT INTO FilesDownloaded ");
                cmdStr.Append("(fileName, downloaded, dateDownloaded, storedInDb, ciberCreated, fileMerged) VALUES(");
                cmdStr.Append("'" + fileName + "', 1 , '" + DateTime.Now.ToString() + "',");
                cmdStr.Append("0,0,0 )");

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException se)
            {
                if (se.Message.Contains("PRIMARY KEY"))
                {
                    // then we update the info and not add a new one
                    try
                    {

                        OmcCdrFileInfo ri = GetOmcCdrInfo(fileName);
                        ri.Downloaded = 1;
                        ri.DateDownloaded = DateTime.Now;
                        UpdateFileInfo(ri);
                    }
                    catch (System.Exception e)
                    {
                        OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::UpdateFileInfo():FailedTryingToUPDATETheFileInfoInTheDB:" + fileName + e.Message + "\r\n" + e.StackTrace);
                    }
                }// if

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::AddDateDownloaded():FailedTryingToAddTheFileNameInTheDB:" + e.Message + "\r\n" + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// AddFileInfo()

        /// <summary>
        /// method to add the fully populated fileinfo to the database to prevent:
        ///  the file from being dowloaded twice
        ///  the CDRs from being duplicated into the database
        ///  the CIBER records being created twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void AddFileInfo(OmcCdrFileInfo fileInfo )
        {
            // add logic to verify all fields in the fileInfo are set before trying to use them

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder("INSERT INTO FilesDownloaded ");
                cmdStr.Append("(fileName, downloaded, dateDownloaded, storedInDb, dateStoredInDb, ciberCreated, dateCiberCreated ) VALUES(");
                cmdStr.Append("'" + fileInfo.FileName + "', "+fileInfo.StoredInDb+ ", '" + fileInfo.DateDownloaded.ToString() + "'," + fileInfo.StoredInDb);
                cmdStr.Append(",'" + fileInfo.DateStoredInDb.ToString() + "',"+ fileInfo.CiberCreated + ",'" + fileInfo.DateCiberCreated.ToString() + "' )" );

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::AddFileInfo():FailedTryingToAddTheFileNameInTheDB:" + e.Message + "\r\n" + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// AddFileInfo()

        /// <summary>
        /// method to update the database with the user provided fileinfo
        /// </summary>
        /// <param name="fileName">user populated file information </param>
        public void UpdateFileInfo( OmcCdrFileInfo i )
        {

            // add code to make sure the fields are populated
            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            try
            {
                StringBuilder cmdStr = new StringBuilder( "UPDATE FilesDownloaded SET downloaded = " + i.Downloaded + ",");
                cmdStr.Append( " dateDownloaded = '" + i.DateDownloaded.ToString() + "'," );
                cmdStr.Append( " storedInDb = " + i.StoredInDb + "," );
                cmdStr.Append( " dateStoredInDb = '" + i.DateStoredInDb.ToString() + "'," );
                cmdStr.Append(" ciberCreated = " + i.CiberCreated + ",");
                cmdStr.Append(" dateCiberCreated = '" + i.DateCiberCreated.ToString() + "',");
                cmdStr.Append(" fileMerged = " + i.FileMerged + ",");
                cmdStr.Append(" dateFileMerged = '" + i.DateFileMerged.ToString() + "'");
                cmdStr.Append( " WHERE fileName = '" );
                cmdStr.Append( i.FileName + "'" );

                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();

            }
            catch (System.Exception e)
            {
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::UpdateFileInfo():FailedTryingToUpdateTheFileInfoInTheDB:" + i.FileName + e.Message + "\r\n" + e.StackTrace);
            }

            CloseDataConn(ref dataConnection);

        }// UpdateDateDownloaded()   

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
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::StoreCdrInDb():ECaught:" + e.Message + "\r\n" + e.StackTrace);
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
                OmcCdrFileWriter.Instance.WriteToLogFile("OmcCdrDb::StoreCdrInDb():ECaught:" + e.Message + "\r\n" + e.StackTrace);
            }// catch

        }// public void CloseDataConn( ref SqlConnection dataConnection )


    }// class

}// namespace