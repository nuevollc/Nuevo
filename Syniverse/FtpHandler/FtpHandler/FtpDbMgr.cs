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

namespace EPCS.Syniverse.TN
{
    class FtpDbMgr
    {
        private static DateTime NullDateTime = new DateTime(1977, 12, 25);
        private string m_connectionString = ConfigurationManager.AppSettings["SyniverseTNSQLConnectString"];
        private static FtpFileWriter m_logger = FtpFileWriter.Instance;
 
        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateDateFileUploaded(string fileName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection( m_connectionString ))
                {
                    connection.Open();
                    StringBuilder cmdStr = new StringBuilder("UPDATE SyniverseFiles SET uploaded = 1, dateFileUploaded = '");

                    cmdStr.Append(DateTime.Now.ToString() + "'" + " WHERE fileName = '");
                    cmdStr.Append(fileName + "'");

                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), connection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (System.Exception e)
            {
                m_logger.WriteToLogFile("FtpDbMgr::UpdateDateFileDownloaded():FailedTryingToUpdateTheFileNameInTheDB:" + fileName);
                m_logger.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

        }// UpdateDateDownloaded()


        public bool CheckDbForSyniverseFileUploaded(string fileName)
        {
            DataSet ds = new DataSet();
            bool uploaded = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    StringBuilder cmdStr = new StringBuilder("Select * from SyniverseFiles WHERE fileName = '");
                    cmdStr.Append(fileName + "'");

                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr.ToString(), connection);
                    myDaptor.Fill(ds);

                    if (ds.Tables.Count == 0)
                        return uploaded;
                    else
                        uploaded = true;
                }
            }
            catch (System.Exception e)
            {
                m_logger.WriteToLogFile("FtpDbMgr::UpdateDateFileDownloaded():FailedTryingToUpdateTheFileNameInTheDB:" + fileName);
                m_logger.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }
            
            return uploaded;

        }


        /// <summary>
        /// method to add the fileinfo to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">fileinfo object</param>
        public void AddDateUpLoaded( string fileName )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();

                    StringBuilder cmdStr = new StringBuilder("INSERT INTO SyniverseFiles ");
                
                    cmdStr.Append("(fileName, uploaded, dateFileUploaded) VALUES(");
                
                    cmdStr.Append("'" + fileName + "', 1 , '" + DateTime.Now.ToString() + "')"); 

                
                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), connection);
                
                    sqlCommand.CommandType = CommandType.Text;
                
                    sqlCommand.ExecuteNonQuery();
                }

            }
            catch (System.Data.SqlClient.SqlException se)
            {
                if (se.Message.Contains("PRIMARY KEY"))
                {
                    // then we update the info and not add a new one
                    try
                    {
                        this.UpdateDateFileUploaded(fileName) ; 
                    }
                    catch (System.Exception e)
                    {
                        m_logger.WriteToLogFile("FtpDbMgr::UpdateFileInfo():FailedTryingToUPDATETheFileInfoInTheDB:" + fileName + e.Message + "\r\n" + e.StackTrace);
                    }
                }// if

            }
            catch (System.Exception e)
            {
                m_logger.WriteToLogFile("FtpDbMgr::AddDateDownloaded():FailedTryingToAddTheFileNameInTheDB:" + e.Message + "\r\n" + e.StackTrace);
            }

        }// AddDateUpLoaded()
  
    }// class

}// namespace