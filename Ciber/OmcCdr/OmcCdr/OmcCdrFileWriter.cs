using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace Strata8.Wireless.Cdr
{
    /// <summary>
    /// class used to write to the CIBER file and log file
    /// singleton pattern, only one object per application
    /// </summary>
    public sealed class OmcCdrFileWriter
    {
        private string m_eventLogName;
        private string m_LogFile;

        private static volatile OmcCdrFileWriter instance;
        private static object syncRoot = new Object();

        public static OmcCdrFileWriter Instance
        {
            get
            {
                if ( instance == null )
                {
                    lock(syncRoot)
                    {
                        if ( instance == null )
                            instance = new OmcCdrFileWriter();
                    }
                }
                return instance;

            }
        }

        private OmcCdrFileWriter()
        {
            m_LogFile = System.Configuration.ConfigurationManager.AppSettings["OmcCdrHandlerLogName"];
            // get the event log name
            m_eventLogName = System.Configuration.ConfigurationManager.AppSettings["OmcCdrHandlerEventLogName"];
            // set up event logging
            if (!EventLog.SourceExists(m_eventLogName))
            {
                EventLog.CreateEventSource(m_eventLogName, "Application");
            }

        }

        // LOG FILE logging
        /// <summary>
        /// method used to log messages to the error log file
        /// </summary>
        /// <param name="msg"></param>
        public void WriteToLogFile(string msg)
        {
            this.LogFileMsg(msg);

        }// public void WriteToFile(string msg)

        private void LogFileMsg(string msg)
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
                    EventLog.WriteEntry(m_eventLogName, "OmcCdrFileWriter::LogFileMsg():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
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
                    EventLog.WriteEntry(m_eventLogName, "OmcCdrFileWriter::LogFileMsg():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
                    return;
                }
            }


            // Create a new streamwriter to write to the file   
            try
            {
                sw = new StreamWriter(fs);
                sw.Write(DateTime.Now.ToShortDateString() + ":" + DateTime.Now.ToLongTimeString() + msg + "\r\n");

            }
            catch (System.Exception ex)
            {
                EventLog.WriteEntry(m_eventLogName, "OmcCdrFileWriter::LogFileMsg():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
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
                EventLog.WriteEntry(m_eventLogName, "OmcCdrFileWriter::LogFileMsg():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 3020);
                return;
            }

        }// private void LogFileMsg(string msg)
    }
}
