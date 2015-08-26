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

namespace Strata8.Did
{
    public class NpaNxxReader
    {

        private string m_LogFile = ConfigurationManager.AppSettings["NpaNxxLogFile"];
        private string m_npaNxxFile = String.Empty;

        private string m_connectionString = ConfigurationManager.AppSettings["DidSQLConnectString"];

        /// <summary>
        /// ProcessTheFile
        /// This method processes the Verizon Tech DataSheet and puts it into the database
        /// for our application to use.
        /// </summary>
        public void ProcessTheFile()
        {
            try
            {
                m_npaNxxFile = ConfigurationManager.AppSettings["NpaNxxDataFile"];

                List<NpaNxxData> theDataList = ParseTheFile(m_npaNxxFile);

                // add code to update the Parsed, and Parsed date fields in 
                // the BworksCdrFilesDownloaded table
                // bool dbUpdate = UpdateTechDataSheetInDb( theDataList );


            }//try
            catch (System.Exception e)
            {// generic exception
                LogMsg(e.Message + "\r\n" + e.StackTrace);
            }

        }// private void ProcessWCIFile()
        /// <summary>
        /// private method to parse the file and store the data in an array
        /// the array is used later to update the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<NpaNxxData> ParseTheFile(string fileName)
        {
            // make the array , let size grow as we add data to it
            System.Collections.ArrayList theData = new System.Collections.ArrayList();
            
            // params to store 
            // update data of the file and the data itself
            List<NpaNxxData> npaDataList = new List<NpaNxxData>();

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
                            if ( data[0].Contains("npanxx") )
                            {
                                // we have a non-data line -- header or footer... so skip it
                                continue;
                            }
                            else 
                            {
                                // process the data
                                NpaNxxData n = ParseTheLine( data );

                                // add the record to the list for now as well as update the database
                                npaDataList.Add(n);
                                this.UpdateTechDataSheetInDb(n);
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
                            LogMsg(errorMsg + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                        }
                    }
                }
            }// try
            catch (Exception e)
            {
                LogMsg(e.Message + "\r\n" + e.StackTrace);
            }// catch

            return npaDataList;
        }// private void ParseTheFile()


        private void UpdateTechDataSheetInDb(NpaNxxData n)
        {
            char[] sep = new char[] { ',' };
            char[] trim = new char[] { '"' };
            StringBuilder cmdStr = new StringBuilder("INSERT INTO NpaNxx ");

            try
            {
                cmdStr.Append("( npanxx,lata,city,state ) VALUES( ");
                // add the data to the string
                cmdStr.Append("'" + n.NpaNxx + "'");
                cmdStr.Append(",'" + n.Lata + "'");
                cmdStr.Append(",'" + n.City + "'");

                // note special terminator here to end the sql command string
                cmdStr.Append(",'" + n.State + "'" + ")");

                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), connection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ECaughtConfiguringSQLCommand::" + ex.Message + ex.StackTrace);
            }

        } // UpdateTechDataSheetInDb

        private NpaNxxData ParseTheLine(string[] data)
        {
            NpaNxxData d = new NpaNxxData(data);
            return d;
        }

        private void LogMsg(string msg)
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if (!File.Exists(m_LogFile))
            {
                try
                {
                    fs = File.Create(m_LogFile);
                }
                catch (System.Exception ex)
                {
                   // EventLog.WriteEntry(m_eventLogName, "NpaNxxReader::LogMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }

            }// created new file and file stream
            else
            {
                // we just append to the file
                try
                {
                    fs = new FileStream(m_LogFile, FileMode.Append, FileAccess.Write);
                }
                catch (System.Exception ex)
                {
                    //EventLog.WriteEntry(m_eventLogName, "NpaNxxReader::LogMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }
            }


            // Create a new streamwriter to write to the file   
            try
            {
                sw = new StreamWriter(fs);
                sw.Write(msg + "\r\n");

            }
            catch (System.Exception ex)
            {
                //EventLog.WriteEntry(m_eventLogName, "NpaNxxReader::LogMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

            try
            {
                // Close StreamWriter and the file
                if (sw != null)
                    sw.Close();
                fs.Close();
            }
            catch (System.Exception ex)
            {
                //EventLog.WriteEntry(m_eventLogName, "NpaNxxReader::LogMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

        }// private void LogFileMsg(string msg)

        /// <summary>
        /// method to get the SID/BID for the subscriber based on the subscribers MSID
        /// 
        /// </summary>
        /// <param name="msid">The MSID of the subscriber</param>
        /// <returns></returns>
        public NpaNxxData GetNpaNxxInfo(string npanxx)
        {
            NpaNxxData n = new NpaNxxData();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * from NpaNxx where npanxx = '" + npanxx + "' ");

            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();

                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), connection);
                    myDaptor.Fill(ds);


                    // make sure there is only one SID/BID returned here
                    foreach (DataTable myTable in ds.Tables)
                    {
                        //r.count = myTable.Rows.Count.ToString();

                        // only one row with the sidbid
                        foreach (DataRow myRow in myTable.Rows)
                        {
                            n.NpaNxx = myRow.ItemArray[1].ToString();
                            n.Lata = myRow.ItemArray[2].ToString();
                            n.City = myRow.ItemArray[3].ToString();
                            n.State = myRow.ItemArray[4].ToString();
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            return n;
        }

    }
}


