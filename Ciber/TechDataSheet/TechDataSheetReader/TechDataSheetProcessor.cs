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

using Strata8.Wireless.Data;

namespace Strata8.Wireless.Data
{
    public class TechDataSheetProcessor
    {

        public TechDataSheetProcessor()
        { }

        private string m_inventoryReport = string.Empty;

        private string m_LogFile = ConfigurationManager.AppSettings["TDS_LogFile"];
        private string m_techDataSheetFile = String.Empty;

        private string m_connectionString = ConfigurationManager.AppSettings["WirelessTdsSQLConnectString"];

        /// <summary>
        /// ProcessTheFile
        /// This method processes the Verizon Tech DataSheet and puts it into the database
        /// for our application to use.
        /// </summary>
        public void ProcessTheFile()
        {
            try
            {
                m_techDataSheetFile = ConfigurationManager.AppSettings["TDS_DataFile"];

                List<TechData> theDataList = ParseTheFile(m_techDataSheetFile);

                // add code to update the Parsed, and Parsed date fields in 
                // the BworksCdrFilesDownloaded table
                // bool dbUpdate = UpdateTechDataSheetInDb( theDataList );


            }//try
            catch (System.Exception e)
            {// generic exception
                LogError(e.Message + "\r\n" + e.StackTrace);
            }

        }// private void ProcessWCIFile()
        /// <summary>
        /// private method to parse the file and store the data in an array
        /// the array is used later to update the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<TechData> ParseTheFile(string fileName)
        {
            // make the array , let size grow as we add data to it
            System.Collections.ArrayList theData = new System.Collections.ArrayList();
            
            // params to store 
            // update data of the file and the data itself
            DateTime fileUpdateDate;
            List<TechData> techDataList = new List<TechData>();

            char[] sep = new char[] { ',' };
            char[] trim = new char[] { ' ' };
            int lineNumber = 1;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {

                            // parse the line
                            string[] data = line.Split(sep);
                            if (data.GetLength(0) < 0)
                            {
                                // we have a non-data line -- header or footer... so skip it
                                continue;
                            }
                            else if (data[2].Contains("Updated:"))
                            {
                                // grab the update date of this file
                                int yy = Convert.ToInt32( data[2].Substring(16, 4)) ;
                                int mm = Convert.ToInt32( data[2].Substring(13,2) );
                                int dd = Convert.ToInt32( data[2].Substring(10,2) );
                                fileUpdateDate = new DateTime(yy, mm, dd, DateTime.Now.Hour, DateTime.Now.Minute, 0);

                                // update our date in the database as well 
                                // also figure out how to roll back if there is an error loading the data
                                UpdateTechDataSheetDateInDb( fileUpdateDate );
                            }
                            else if (data[0].Contains("VERIZON") )
                            {
                                // process the data
                                TechData td = ParseTheLine( data );

                                // add the record to the list for now as well as update the database
                                techDataList.Add(td);
                                this.UpdateTechDataSheetInDb(td);
                            }

                            lineNumber++;
                        }
                        catch (System.Exception ex)
                        {
                            string errorMsg = "Error in File>" + fileName + " Line>" + lineNumber;
                            if (line != null)
                            {// add the line information if available
                                errorMsg += "Line>" + line;
                            }
                            LogError(errorMsg + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                        }
                    }
                }
            }// try
            catch (Exception e)
            {
                LogError(e.Message + "\r\n" + e.StackTrace);
            }// catch

            return techDataList;
        }// private void ParseTheFile()



        /// <summary>
        /// method to update the database with the parameters from the tech data sheet
        /// </summary>
        /// <param name="d"></param>
        private void UpdateTechDataSheetDateInDb(DateTime d )
        {

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            StringBuilder cmdStr = new StringBuilder("INSERT INTO UpdateDate ");
            cmdStr.Append("( updateDate ) VALUES(");
            // add the data to the string
            cmdStr.Append("'" + d.ToString("g") + "'" + ")");

            try
            {
                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("ECaught::" + e.Message + e.StackTrace);

            }

            this.CloseDataConn(ref dataConnection);

        }
        private void UpdateTechDataSheetInDb( TechData td  )
        {
            char[] sep = new char[] { ',' };
            char[] trim = new char[] { '"' };
            StringBuilder cmdStr = new StringBuilder("INSERT INTO VerizonTechData ");

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);
            
            // add code to check the data connection

            // add try catch block around setting up the command

            try
            {
                cmdStr.Append("(carrierName,sidBid,sidName,state,band,");
                cmdStr.Append("mbiMin,lowLineRange,highLineRange,hlrPointCode, hlrClliCode, hlrMscid, hlrEsid, transmittedCdmaSid,");
                cmdStr.Append("gteTsiCarrierCode, rateCenter, rateCenterState, county, mtasLikeCode, rbtEmrsGroup ) VALUES(");
                // add the data to the string
                cmdStr.Append("'" + td.CarrierName + "'");
                cmdStr.Append(",'" + td.SidBid + "'");
                cmdStr.Append(",'" + td.SidName + "'");
                cmdStr.Append(",'" + td.State + "'");
                cmdStr.Append(",'" + td.Band + "'");
                cmdStr.Append(",'" + td.MbiMin + "'");
                cmdStr.Append(",'" + td.LowLineRange + "'");
                cmdStr.Append(",'" + td.HighLineRange + "'");
                cmdStr.Append(",'" + td.HlrPointCode + "'");
                cmdStr.Append(",'" + td.HlrClliCode + "'");
                cmdStr.Append(",'" + td.HlrMscid + "'");
                cmdStr.Append(",'" + td.HlrEsid + "'");
                cmdStr.Append(",'" + td.TransmittedCdmaSid + "'");
                cmdStr.Append(",'" + td.GteTsiCarrierCode + "'");
                cmdStr.Append(",'" + td.RateCenter + "'");
                cmdStr.Append(",'" + td.RateCenterState + "'");
                if (td.County.Contains("'"))
                    cmdStr.Append(",'" + td.County.Replace(Convert.ToChar("'"), Convert.ToChar(" ")) + "'");
                else
                    cmdStr.Append(",'" + td.County + "'");

                cmdStr.Append(",'" + td.MtasLikeCode + "'");

                // note special terminator here to end the sql command string
                cmdStr.Append(",'" + td.RbtEmrsGroup + "'" + ")");
            }

            catch (Exception ex)
            {
                Console.WriteLine("ECaughtConfiguringSQLCommand::" + ex.Message + ex.StackTrace);
            }


            try
            {
                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("ECaught::" + e.Message + e.StackTrace);

            }

            this.CloseDataConn(ref dataConnection);

        }

        private TechData ParseTheLine(string[] data )
        {
            TechData d = new TechData(data);
            return d;
        }

        public void LogError(string msg)
        {
            try
            {
                FileStream file = new FileStream(m_LogFile, FileMode.Append, FileAccess.Write);

                // Create a new stream to write to the file
                StreamWriter sw = new StreamWriter(file);
                //sw.Write("----------\r\n");
                // Write a string to the file
                sw.Write(msg + "\r\n");
                //sw.Write("----------\r\n");
                // Close StreamWriter
                sw.Close();
                // Close file
                file.Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error Ocurred While Writing to User Log File>" + ex.ToString());
            }
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

        /// <summary>
        /// method to get the SID/BID for the subscriber based on the subscribers MSID
        /// 
        /// </summary>
        /// <param name="msid">The MSID of the subscriber</param>
        /// <returns></returns>
        public string GetSubscriberSidBid(string msid)
        {
            // make the connection
            SqlConnection dataConnection = null;
            string sidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
            string npanxx = msid.Substring(0, 6);
            string lastFour = msid.Substring(5, 4);
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT sidBid from VerizonTechData where mbiMin = '" + npanxx + "' ");
            sb.Append("AND lowLineRange <=  '" + lastFour + "' ");
            sb.Append("AND highLineRange >= '" + lastFour + "' ");

            DataSet ds = new DataSet();

            try
            {

                OpenDataConn(ref dataConnection);

                // execute and fill
                SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), dataConnection);
                myDaptor.Fill(ds);

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                    return TechDataEnums.SIDBID_NOT_FOUND.ToString();
                else if (ds.Tables.Count == 0)
                    return TechDataEnums.SIDBID_NOT_FOUND.ToString();

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
                        sidBid = myRow.ItemArray[0].ToString();
                    }
                }

            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            this.CloseDataConn(ref dataConnection);
            return sidBid;

        }

    }//class


}//namespace
