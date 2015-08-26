using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;
using TruMobility.Utils.Logging;

namespace TruMobility.Network.Services
{
    public class LTEDbMgr
    {

        private static string m_connectionString = String.Empty;
        private LogFileMgr m_logger = LogFileMgr.Instance;

        /// <summary>
        /// class to handle the database parameters related to the LTE file from 
        /// Sprint.
        /// </summary>
        /// 
        public LTEDbMgr()
        {
            m_connectionString = ConfigurationManager.AppSettings["WirelessCdrSQLConnectString"]; 
        }

        /// <summary>
        /// method to add the fileinfo to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">fileinfo object</param>
        public void AddFileDownLoaded(string fileName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();

                    StringBuilder cmdStr = new StringBuilder("INSERT INTO LTEFilesDownloaded ");

                    cmdStr.Append("(fileName, dateDownloaded) VALUES(");

                    cmdStr.Append("'" + fileName + "' , '" + DateTime.Now.ToString() + "')");


                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), connection);

                    sqlCommand.CommandType = CommandType.Text;

                    sqlCommand.ExecuteNonQuery();
                }

            }
            catch (System.Data.SqlClient.SqlException se)
            {
                m_logger.WriteToLogFile("LTEDbMgr::AddFileDownLoaded():FailedTryingToAddTheFileNameInTheDB:" + se.Message + "\r\n" + se.StackTrace);
            }
            catch (System.Exception e)
            {
                m_logger.WriteToLogFile("LTEDbMgr::AddFileDownLoaded():FailedTryingToAddTheFileNameInTheDB:" + e.Message + "\r\n" + e.StackTrace);
            }

        }// AddDateUpLoaded()
   
        public bool CheckDbFileDownloaded(string fileName)
        {
            bool uploaded = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    StringBuilder cmdStr = new StringBuilder("Select * from LTEFilesDownloaded WHERE fileName = '");
                    cmdStr.Append(fileName + "'");

                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr.ToString(), connection);
                    myDaptor.TableMappings.Add("LTEFilesDownloaded", "Files");

                    DataSet files = new DataSet("Files");
                    myDaptor.Fill(files, "LTEFilesDownloaded");
                   
                    bool done = CheckData( files );
                    if ( done )
                        uploaded = true;
                }
            }
            catch (System.Exception e)
            {
                m_logger.WriteToLogFile("LTEDbMgr::CheckDbFileDownloaded():FailedTryingToCheckTheFileNameInTheDB:" + fileName);
                m_logger.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            return uploaded;

        }

        private bool CheckData(DataSet dataSet)
        { 
            
            DataTableCollection dtc = dataSet.Tables;
            DataTable dt = dtc[0];
            if (dt.Rows.Count.Equals(0))
                return false;
            else
                return true;

            // Get Each DataTable in the DataTableCollection and 
            // print each row value.
            //foreach (DataTable table in dataSet.Tables)
            //    foreach (DataRow row in table.Rows)
            //        foreach (DataColumn column in table.Columns)
            //            if (row[column] != null)
            //                Console.WriteLine(row[column]);
        }

        public void LogError(string msg)
        {
            LogFileMgr.Instance.WriteToLogFile(msg);
            
        }// public void LogFileError(string msg)


    }//class


}//namespace
