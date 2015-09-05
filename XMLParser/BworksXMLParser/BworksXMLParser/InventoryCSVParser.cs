
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.Collections;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System;

namespace Strata8.Provisioning.Reports
{

    /// <summary>
    /// CdrHandler : Responsible for the Cdr processing.
    /// </summary>
    public class InventoryCSVParser
    {
        private string m_eventLogName;  
        private string m_LogFile; 

        public InventoryCSVParser()
        {
            //
            // TODO: Add constructor logic here
            //
            // ADD log files and locations

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
                EventLog.WriteEntry(m_eventLogName, "CDRHandler Service FAILED trying to get a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
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
                EventLog.WriteEntry(m_eventLogName, "CDRHandler Service FAILED trying to close a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void CloseDataConn( ref SqlConnection dataConnection )


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
                EventLog.WriteEntry(m_eventLogName, "Error Ocurred While Writing to Error Log File>" + ex.ToString() + " Log file name is>" + m_LogFile, EventLogEntryType.Error, 3020);
            }
        }// public void LogFileError(string msg)
        

    }
}
