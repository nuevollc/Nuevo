using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Data;

using WCIDIDParser.DidServiceReference;

namespace Strata8.Provisioning.Tools
{

    /// <summary>
    /// class to parse the excel spreadsheet from WCI
    /// </summary>
    public class WCIDIDParser
    {
        private string m_LogFile = ConfigurationManager.AppSettings["LogFile"];
        private string m_wciFile = String.Empty;

        private const string cid1 = "0040432039";
        private const string cid2 = "0040432040";
        private const string tg1 = "32-1006";
        private const string tg2 = "32-1007";
        private const string mci_trunk = "2110/STTLWA62DS1/STRATA8-ORIG";

        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        public void ProcessWCIFile()
        {
            ArrayList theDIDs;
            try
            {
                m_wciFile = ConfigurationManager.AppSettings["WCIFile"];

                theDIDs = ParseTheDIDs(m_wciFile);

                // add code to update the Parsed, and Parsed date fields in 
                // the BworksCdrFilesDownloaded table
                bool dbUpdate = CheckDIDsInDb(theDIDs);


            }//try
            catch (System.Exception e)
            {// generic exception
                LogFileError(e.Message + "\r\n" + e.StackTrace);
            }

        }// private void ProcessWCIFile()


        /// <summary>
        /// private method to format/write the cdrs to the database
        /// </summary>
        /// <param name="theCdr"></param>
        /// <returns></returns>
        private bool CheckDIDsInDb(ArrayList theDidList)
        {
            int controlNbr = 0;
            
            string sourceId = "3"; // wci
            string stateId = "5";  // unassigned
            string typeId = "2";   //wireline

            // make the connection
            SqlConnection dataConnection = null;

            foreach (string theDid in theDidList)
            {
                string npa = theDid.Substring(0, 3);
                string nxx = theDid.Substring(3, 3);
                string lastFour = theDid.Substring(6, 4);

                //DidServiceSoapClient d = new DidServiceSoapClient();
                //DataSet ds = d.GetDID(theDid);

                if (!GetDid(theDid))
                {
                    try
                    {

                        OpenDataConn(ref dataConnection);

                        StringBuilder cmdStr = new StringBuilder("INSERT INTO DID ");
                        cmdStr.Append("(NPA,NXX,LastFour,DID,DIDSourceId, DidStateId, DidTypeId, TrunkGroup)");
                        cmdStr.Append(" VALUES(");

                        cmdStr.Append("'" + npa + "'");
                        cmdStr.Append(",'" + nxx + "'"); // service provider field = userId
                        cmdStr.Append(",'" + lastFour + "'");

                        cmdStr.Append(",'" + theDid + "'");
                        cmdStr.Append(",'" + sourceId + "'");
                        cmdStr.Append(",'" + stateId + "'");

                        cmdStr.Append(",'" + typeId + "'");
                        cmdStr.Append(",'" + mci_trunk + "')");

                        SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();
                        controlNbr++;

                        CloseDataConn(ref dataConnection);

                    }
                    catch (System.Exception e)
                    {
                        LogFileError(e.Message + "\r\n" + e.StackTrace);
                    }
                }

            }//for each CDR


 
            return true;
        }

        private bool GetDid(string did)
        {
            bool gotIt = false;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT did from did where Did = '" + did + "'");

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
                    return true;
                else if (ds.Tables.Count == 0)
                    return false;

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    if (myTable.Rows.Count > 0)
                        return true;
                }

                CloseDataConn(ref dataConnection);
            }
            catch (Exception e)
            {
                LogFileError(e.Message + "\r\n" + e.StackTrace);
            }// catch

            return gotIt;
        }//GetDID

        private static DataSet DidServiceReference()
        {
            throw new NotImplementedException();
        }

        private void OpenDataConn(ref SqlConnection dataConnection)
        {
            try
            {
                dataConnection = new SqlConnection();
                dataConnection.ConnectionString = ConfigurationManager.AppSettings["SQLConnectString"];
                dataConnection.Open();

            }// try 
            catch (Exception e)
            {
                LogFileError(e.Message + "\r\n" + e.StackTrace);
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
                LogFileError( e.Message + "\r\n" + e.StackTrace);
            }// catch
        }// public void CloseDataConn( ref SqlConnection dataConnection )
    
        /// <summary>
        /// private method to parse the dids in the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private ArrayList ParseTheDIDs(string fileName)
        {
            // make the array size (number of cdrs per file ) configurable
            System.Collections.ArrayList theControls = new System.Collections.ArrayList(1000);
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
                            string[] controls = line.Split(sep);
                            if (controls.GetLength(0) < 0 || lineNumber.Equals(1) )
                            {
                                lineNumber++;
                                // we have a non-data line -- header or footer... so skip it
                                continue;
                            }

                            string did = controls[0].Trim().Replace("-", String.Empty);                                

                            // cache the record
                            theControls.Add( did );                      

                            lineNumber++;
                        }
                        catch (System.Exception ex)
                        {
                            string errorMsg = "Error in File>" + fileName + " Line>" + lineNumber;
                            if (line != null)
                            {// add the line information if available
                                errorMsg += "Line>" + line;
                            }
                            LogFileError(errorMsg + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                        }
                    }
                }
            }// try
            catch (Exception e)
            {
                LogFileError(e.Message + "\r\n" + e.StackTrace);
            }// catch

            return theControls;
        }// private void ParseTheDIDs()

        public void LogFileError(string msg)
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
                //
            }
        }// public void LogFileError(string msg)
        

    }
}
