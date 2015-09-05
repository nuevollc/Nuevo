using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Cdr.Utils;

namespace CdrFtpHandler
{
    public class FtpDbManager
    {
                
        private static string _connectionString = String.Empty;

        public FtpDbManager()
        {
            _connectionString = ConfigurationManager.AppSettings["SQLConnectString"]; 
 
        }

       public bool CheckDbBeforeDownloadingFile(string fileName)
        {
            bool fileDownloaded = true;
            StringBuilder cmdStr = new StringBuilder("SELECT fileName from BworksCdrFileToLogi where fileName =");
            DataSet mySet = new DataSet();
            SqlDataAdapter rDapter = null;

            try
            {

                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    dataConnection.Open();
                    cmdStr.Append("'" + fileName + "'");
                    // execute and fill
                    rDapter = new SqlDataAdapter(cmdStr.ToString(), dataConnection);
                    rDapter.Fill(mySet);

                    // have one table
                    DataTable theTable = mySet.Tables[0];
                    DataRow[] currentRows = theTable.Select( null, null, DataViewRowState.CurrentRows);

                    if (currentRows.Length < 1)
                    {
                        Console.WriteLine("No Current Rows Found");
                        fileDownloaded = false;
                    }

                }// using
            }//try
            catch (System.Exception e)
            {
                FileWriter.Instance.WriteToLogFile("FtpDbManager::CheckDbBeforeDownloadingFile():FailedTryingGetTheFileNameInTheDB");
                FileWriter.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

            return fileDownloaded;

        }//CheckDbBeforeDownloadingFile

        /// <summary>
        /// method to add the filename to the database to prevent it from being downloaded twice
        /// </summary>
        /// <param name="fileName">name of the file that has been downloaded from the remote server</param>
        public void UpdateFtpFileNameInDb(string fileName)
        {
            // make the connection

            try
            {
                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    dataConnection.Open();
                    StringBuilder cmdStr = new StringBuilder("INSERT INTO BworksCdrFileToLogi ");
                    cmdStr.Append("(fileName) VALUES(");
                    cmdStr.Append("'" + fileName + "' )");

                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();

                }
            }
            catch (System.Exception e)
            {
                FileWriter.Instance.WriteToLogFile("FtpDbManager::UpdateFtpFileNameInDb():FailedTryingToUpdateTheFileNameInTheDB");
                FileWriter.Instance.WriteToLogFile("ECaught:" + e.Message + e.StackTrace);
            }

        }// UpdateFtpFileNameInDb()

    }
}
