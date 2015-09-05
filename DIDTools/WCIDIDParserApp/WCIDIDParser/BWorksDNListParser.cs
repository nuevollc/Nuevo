 
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

namespace Strata8.Provisioning.Tools
{


    public class Info
    {
        string did;
        string groupName;
        int groupNum;
        int spId;

        public string DID
        {
                    get
            {
                return this.did;
            }
            set
            {
                did = value;
            }
        }

        public string GroupName
        {
                    get
            {
                return this.groupName;
            }
            set
            {
                groupName = value;
            }

        }

        public int GroupNum
        {
            get
            {
                return this.groupNum;
            }
            set
            {
                groupNum = value;
            }
        }
        public int SPID
        {
            get
            {
                return this.spId;
            }
            set
            {
                spId = value;
            }
        }

    }// Info

    /// <summary>
    /// class to parse the excel spreadsheet from WCI
    /// </summary>
    /// 
    public class BWorksDNListParser
    {
        private string m_LogFile = ConfigurationManager.AppSettings["LogFile"];
        private string m_File = String.Empty;

        private const string cid1 = "0040432039";
        private const string cid2 = "0040432040";
        private const string tg1 = "32-1006";
        private const string tg2 = "32-1007";
        private const string mci_trunk = "2110/STTLWA62DS1/STRATA8-ORIG";
        //private  string[] groups = { "701310", "722005", "780085", "AGM_gp", "Anka_gp", "Atlas_Networks_gp", "CommunityServices_gp", "Floorcraft_gp", "Melodeo_gp", "Microsoft_gp", "Nuevo_gp", "Premiere_gp", "Redapt_gp", "Sonicboon_gp" };
        //private  int[] groupNum = {41, 35,36,4,6,42,30,9,24,44,26,17,28,27};
        //private int[] spId = { 4,4,4,1,1,7,1,1,1,1,1,4,1,1};

        private string[] groups = { "Strata8_gp", "Strata8Eng_gp", "SWU_gp", "USSeafoods_gp"};
        private int[] groupNum = { 1,23,45,37 };
        private int[] spId = { 1,5,6,1 };

        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        public void ProcessFile()
        {
            ArrayList theDIDs;
            try
            {
                m_File = ConfigurationManager.AppSettings["BWorksFile"];

                theDIDs = ParseTheDIDs(m_File);

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
            int assignedStateId=4;

            int typeId = 2;   //wireline

            // make the connection
            SqlConnection dataConnection = null;

            foreach (Info theDid in theDidList)
            {
                string npa = theDid.DID.Substring(0, 3);
                string nxx = theDid.DID.Substring(3, 3);
                string lastFour = theDid.DID.Substring(6, 4);
                int spId = theDid.SPID;
                int groupId = theDid.GroupNum;

                //DidServiceSoapClient d = new DidServiceSoapClient();
                //DataSet ds = d.GetDID(theDid);

                if (!GetDid(theDid.DID))
                {
                    // not in the database, let's put it in and label it as assigned
                    int sourceId = 6;// unknown since they are in broadworks but not in our inventory 
                    try
                    {

                        OpenDataConn(ref dataConnection);

                        StringBuilder cmdStr = new StringBuilder("INSERT INTO DID ");
                        cmdStr.Append("(NPA,NXX,LastFour,DID,DIDSourceId, DidStateId, DidTypeId, SPId, GroupId)");
                        cmdStr.Append(" VALUES(");

                        cmdStr.Append("'" + npa + "'");
                        cmdStr.Append(",'" + nxx + "'"); // service provider field = userId
                        cmdStr.Append(",'" + lastFour + "'");

                        cmdStr.Append(",'" + theDid.DID + "'");
                        cmdStr.Append("," + sourceId );
                        cmdStr.Append("," + assignedStateId);

                        cmdStr.Append("," + typeId);
                        cmdStr.Append("," + spId + "," + groupId + ")" );

                        SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();

                        CloseDataConn(ref dataConnection);

                    }
                    catch (System.Exception e)
                    {
                        LogFileError(e.Message + "\r\n" + e.StackTrace);
                    }
                }
                else
                {
                    UpdateDid( theDid );
                }


            }//for each CDR


 
            return true;
        }

        private void UpdateDid(Info did)
        {
            SqlConnection dataConnection = null;

            try
            {
                StringBuilder cmdStr = new StringBuilder("UPDATE DID SET DidStateId = 4, SPId = " + did.SPID + ",");

                cmdStr.Append(" GroupId = " + did.GroupNum + " WHERE DID = '" + did.DID + "'");

                // make the connection

                OpenDataConn(ref dataConnection);

                try
                {

                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogFileError("BWorksDNListParser::UpdateDid():ECaught:" + ex.Message + ex.StackTrace);
                }

            }//try
            catch (Exception ex)
            {
                LogFileError("BWorksDNListParser::UpdateDid():ECaught:" + ex.Message + ex.StackTrace);
            }

            CloseDataConn(ref dataConnection);

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

            bool newGroup = false;
            int groupNumber = 0;
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
                            if (controls[0].Contains("<command") )
                            {
                                newGroup = true;
                                // we have a non-data line -- header or footer... so skip it
                                continue;
                            }
                            else if ( controls[0].Contains("</command") )
                            {
                                newGroup = false;
                                groupNumber++;
                                continue;
                            }  
                            else if (controls[0].Contains("phoneNumber") )
                            {
                                int indx = controls[0].IndexOf(">");
                                string did = controls[0].Substring(indx+1, 10 );
                                Info info = new Info();
                                info.DID = did;
                                info.GroupName = groups[groupNumber];
                                info.GroupNum = groupNum[groupNumber];
                                info.SPID = spId[groupNumber];

                                theControls.Add( info ); 
                            }

                        }
                        catch (System.Exception ex)
                        {
                            string errorMsg = "Error in File>" + fileName + " Line>" + groupNumber;
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
