using System;
using System.IO;
using System.Diagnostics;


namespace TruMobility.Network.Services
{

    /// <summary>
    /// class used to write to the CIBER file and log file
    /// singleton pattern, only one object per application
    /// </summary>
    public sealed class FileWriter
    {
        // this is the CIBER error log file for our errors
        private string m_LogFile = System.Configuration.ConfigurationManager.AppSettings["MAFLogFileName"];
        private string m_eventLogName;

        private static volatile FileWriter instance;
        private static object syncRoot = new Object();

        public static FileWriter Instance
        {
            get
            {
                if ( instance == null )
                {
                    lock(syncRoot)
                    {
                        if ( instance == null )
                            instance = new FileWriter();
                    }
                }
                return instance;

            }
        }
        private FileWriter()
        {

            // get the event log name that this object uses to write to the event log
            // m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["MAFEventLogName"];
            // set up event logging
            //if (!EventLog.SourceExists(m_eventLogName))
            //{
            //    //EventLog.CreateEventSource(m_eventLogName, "Application");
            //}
        
        }


        /// <summary>
        /// public method to write to the error log file
        /// </summary>
        /// <param name="msg"></param>
        public void WriteToLogFile( string msg )
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
                    EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
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
                    EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }
            }

            // Create a new streamwriter to write to the file   
            try
            {
                sw = new StreamWriter(fs);
                sw.Write(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + msg + "\r\n");

            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }
        }


        /// <summary>
        /// private method used to write to the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="msg"></param>
        private void WriteToFile( string fileName, string msg )
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if (!File.Exists(fileName))
            {
                try
                {
                    fs = File.Create(fileName);
                }
                catch (System.Exception ex)
                {
                    EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
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
                    EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry(m_eventLogName, "FileWriter::WriteToFile():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

        }// private void WriteToFile(string msg)


    }
}
