
using System;
using System.Text;
using System.IO;
using Strata8.Wireless.Cdr.Rating;// ciber record code

namespace TruMobility.Network.Services

{
    public class MAFFileReader
    {
        char[] m_sep;
        char[] m_trim;
        char[] m_backslash;
        char[] m_dot;

        MAFDbMgr m_dbMgr = new MAFDbMgr();
        FileWriter m_logger = FileWriter.Instance;

        public MAFFileReader()
        {
        
            m_sep = new char[] { ',' };
            m_trim = new char[] { '"' };
            m_backslash = new char[] { '\\' };
            m_dot = new char[] { '.' };

        }

        public void ProcessTheFile( string fileName )
        {
            // MAF only supports type01, 22 and 98.
            m_logger.WriteToLogFile("-NFORMATIONAL::MAFFileReader::ProcessTheFile():Entered");

            string type01 = "01";
            string type02 = "02"; 
            string type22 = "22";
            string type32 = "32";
            string type97 = "97";
            string type98 = "98";
            

            try 
            {

                using (StreamReader sr = new StreamReader(fileName))
                {
                    char[] buff = new char[2];
                    int recordCount = 1;
                   
                    //string str = sr.ReadToEnd();
                    while ((sr.Read(buff, 0, 2) != 0 ))
                    {
                        string recordType = buff[0].ToString() + buff[1].ToString();

                        if (recordType.Equals(type01))
                        {
                            // type 01, 02 record length = 200
                            // read the rest of the type 01 record
                            char[] buff1 = new char[200];

                            // first two elements are 01
                            sr.Read(buff1, 2, 198);
                            buff1[0] = buff[0];
                            buff1[1] = buff[1]; 

                            StringBuilder sb = new StringBuilder();
                            
                            // process type 01 record
                            foreach (char c in buff1)
                                sb.Append( c.ToString() );

                            Record01 r01 = new Record01(sb.ToString());
                            //WriteToFile(@"d:\apps\data\test.dat", sb.ToString() );

                        }
                        else if (recordType.Equals(type22))
                        {
                            // type 22, record length = 547
                            // read the rest of the type 01 record
                            char[] buff22 = new char[547];
                            sr.Read(buff22, 2, 545);
                            buff22[0] = buff[0];
                            buff22[1] = buff[1]; 

                            StringBuilder sb = new StringBuilder();
                            // process type 22 record
                            foreach (char c in buff22)
                                sb.Append(c.ToString());

                            Record22 r22 = new Record22(sb.ToString());

                            // update the database with the following parameters for reporting
                            UpdateDb( r22 );
                            // FileWriter.Instance.WriteToLogFile( sb.ToString() );                            
                        }
                        else if (recordType.Equals(type32))
                        {
                            // type 32, record length = 567
                        }
                        else if (recordType.Equals(type02))
                        {
                            // type 22, record length = 200
                        }
                        else if (recordType.Equals(type97))
                        {
                            // type 97, record length = 200
                        }
                        else if (recordType.Equals(type98))
                        {
                            // type 98, record length = 200
                            // read the rest of the type 01 record
                            char[] buff98 = new char[200];
                            sr.Read(buff98, 2, 198);
                            buff98[0] = buff[0];
                            buff98[1] = buff[1];

                            StringBuilder sb = new StringBuilder();
                            // process type 22 record
                            foreach ( char c in buff98 )
                                sb.Append( c.ToString() );

                           Record98 r98 = new Record98( sb.ToString() );
                           // WriteToFile( "test", sb.ToString() );
                           break;

                        }
                        else
                        {
                            //
                        }

                        // increment our record count
                        recordCount++;

                    }// while loop - end of file


                }//using sr
                        
            }//try
            catch (SystemException se)
            {
                m_logger.WriteToLogFile("-NEXCEPTION::MAFFileReader::ProcessTheFile():ECaught:" + se.Message + se.StackTrace );

            }
            m_logger.WriteToLogFile("-NFORMATIONAL::MAFFileReader::ProcessTheFile()Exiting");

        }// ReadTheFile

        private void WriteToFile(string fileName1, string msg)
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;
            string fileName = @"d:\apps\data\output.txt";

            if (!File.Exists(fileName))
            {
                try
                {
                    fs = File.Create(fileName);
                }
                catch (System.Exception ex)
                {
                    // EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }

            }// created new file and file stream
            else
            {
                // we just append to the file
                try
                {
                    fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                }
                catch (System.Exception ex)
                {
                    //EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
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
                //EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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
                // EventLog.WriteEntry(m_eventLogName, "FtpHandler::LogFileMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

        }// private void LogFileMsg(string msg)
    
        private void UpdateDb( Record22 r22 )
        {

            //add code for the update db method here
            // update the database with the following parameters
            // r22.CalledNumberDigits, r22.CallDate, r22.ServingPlace, r22.ServingStateProvince, r22.CallerId, r22.AirConnectTime,
            // r22.AirChargeableTime, r22.InitialCellSite, r22.TollChargeableTime, r22.Msid, r22.MsisdnMdn, r22.CallDirection, r22.EsnUimidImeiMeid
            m_dbMgr.InsertMafRecord(r22);
        
        }

    }//CiberReader

}//ns
