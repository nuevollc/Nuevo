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
using Strata8.Wireless.Cdr.Ciber;

namespace Strata8.Wireless.Cdr
{

    /// <summary>
    /// class to parse and read the CIBER files 
    /// </summary>
    public class CiberCdrReader
    {
        private string m_LogFile = ConfigurationManager.AppSettings["LogFile"];
        private string m_ciberFile = String.Empty;

        /// <summary>
        /// ProcessJobControlFileThread:Method that blocks on the file Q waiting for a file
        /// to be queued up that needs to be processed.
        /// </summary>
        public void ProcessCiberFile( string fileName )
        {
            ArrayList theDIDs;
            try
            {
                m_ciberFile = fileName; // ConfigurationManager.AppSettings["CiberFileToRead"];

                theDIDs = ReadTheFile(m_ciberFile);


            }//try
            catch (System.Exception e)
            {// generic exception
                LogFileError(e.Message + "\r\n" + e.StackTrace);
            }

        }// private void ProcessWCIFile()



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
        private ArrayList ReadTheFile(string fileName)
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

                            //// parse the line
                            //string[] controls = line.Split(sep);
                            //if (controls.GetLength(0) < 0)
                            //{
                            //    // we have a non-data line -- header or footer... so skip it
                            //    continue;
                            //}

                            //string did = controls[0].Trim().Replace(" ", String.Empty);
                            //// cache the record
                            //theControls.Add( did );  
                            string recordType = line.Substring(0, 2);
                            
                            
                            
                            switch (recordType)
                            {
                                case ("01"):
                                    {
                                        Console.WriteLine("Type 01 RecordType : Batch Header Record");
                                        Record01 r = ProcessRecordType01(line);

                                        // let us look and see
                                        Console.WriteLine(r.ToString());
                                        break;
                                    }
                                case ("02"):
                                    {
                                        Console.WriteLine("Type 01 RecordType : ClearingHouse Batch Header Record");
                                        Record02 r = ProcessRecordType02(line);

                                        // let us look and see
                                        Console.WriteLine(r.ToString());
                                        break;
                                    }
                                case ("22"):
                                    {
                                        Console.WriteLine("Type 22 RecordType : Air and Toll Charges Record");
                                        Record22 r = new Record22(line);

                                        //look and see for now
                                        Console.WriteLine(r.ToString());
                                        break;
                                    }
                                case ("52"):
                                    {
                                        Console.WriteLine("Type 52 RecordType : Billing OCC Charge Record");
                                        //Record52 r = new Record52(line);

                                        ////look and see for now
                                        //Console.WriteLine(r.ToString());
                                        break;
                                    }
                                case ("98"):
                                    {
                                        Console.WriteLine("Type 98 RecordType : Batch Trailer Record");
                                        Record98 r = ProcessRecordType98(line);

                                        // let us look and see
                                        Console.WriteLine(r.ToString());
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }


                            }
                            Console.WriteLine(line.ToString());
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
            
        }// public void LogFileError(string msg)

        private Strata8.Wireless.Cdr.Ciber.Record01 ProcessRecordType01(string line)
        {
            // construct the record
            Record01 r = new Record01(line);
            return r;
        }
        private Strata8.Wireless.Cdr.Ciber.Record02 ProcessRecordType02(string line)
        {
            // construct the record
            Record02 r = new Record02(line);
            return r;
        }
        private Strata8.Wireless.Cdr.Ciber.Record98 ProcessRecordType98(string line)
        {
            // construct the record
            Record98 r = new Record98(line);
            return r;
        }

        private Strata8.Wireless.Cdr.Ciber.Record22 ProcessRecordType22(string line)
        {
            // construct the record
            Record22 r = new Record22(line);
            return r;
        }
    }
}
