using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;


namespace Strata8.Provisioning.Reports
{

    /// <summary>
    ///  class to parse the inventory report produced from the broadworks platform
    ///  using the following OCI command :
    ///  
    ///  <command xsi:type="UserGetListInServiceProviderRequest" xmlns="">
    ///     <serviceProviderId>Strata8_sp</serviceProviderId>
    ///  </command>
    /// 
    /// </summary>
    public class InventoryReportParser
    {
        private string m_inventoryReport = string.Empty;
        private string m_LogFile = string.Empty;


        public void ProcessInventoryReport()
        {
            XmlTextReader reader = null;
            int colIndx = 0;

            //// get the event log name
            m_inventoryReport = System.Configuration.ConfigurationManager.AppSettings["ReportFileName"];            //// get the event log name
            m_LogFile = System.Configuration.ConfigurationManager.AppSettings["UserFileName"];

            try
            {
                // Load the reader with the data file and ignore 
                // all white space nodes.
                reader = new XmlTextReader(m_inventoryReport);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                StringBuilder sb = null;

                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.Write("<{0}>", reader.Name);
                            if (reader.Name.Equals("col"))
                            {
                                if (colIndx == 0)
                                {
                                    sb = new StringBuilder();
                                }
                                else
                                {
                                    // append the comma to add the field
                                    sb.Append(",");
                                }
                                colIndx++;
                            }
                            break;
                        case XmlNodeType.Text:
                            Console.Write(reader.Value);
                            if (colIndx == 10)
                                sb.Append(reader.Value.Trim());
                            else if (colIndx > 0)
                                 sb.Append(reader.Value.Trim()); 
                            break;
                        case XmlNodeType.CDATA:
                            Console.Write("<![CDATA[{0}]]>", reader.Value);
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            Console.Write("<?{0} {1}?>", reader.Name, reader.Value);
                            break;
                        case XmlNodeType.Comment:
                            Console.Write("<!--{0}-->", reader.Value);
                            break;
                        case XmlNodeType.XmlDeclaration:
                            Console.WriteLine("<?xml version='1.0'?>");
                            break;
                        case XmlNodeType.Document:
                            break;
                        case XmlNodeType.DocumentType:
                            Console.Write("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
                            break;
                        case XmlNodeType.EntityReference:
                            Console.Write(reader.Name);
                            break;
                        case XmlNodeType.EndElement:
                            Console.WriteLine("</{0}>", reader.Name);
                            if (reader.Name.Equals("row"))
                            {
                                    colIndx = 0;
                                    // terminate the line since this user is complete
                                    this.WriteUserToFile(sb.ToString());
                                    this.UpdateDb(sb.ToString());
                                
                             
                            }

                            break;
                    }
                }
            }

            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }


        private void UpdateDb(string valueString)
        {
            char[] sep = new char[] { ',' };
            char[] trim = new char[] { '"' };
            // parse the line
            string[] theData = valueString.Split(sep);

            // make the connection
            SqlConnection dataConnection = null;
            OpenDataConn(ref dataConnection);

            StringBuilder cmdStr = new StringBuilder("INSERT INTO User ");
            cmdStr.Append("(userId,groupId,lastName,firstName,department,");
            cmdStr.Append("phoneNumber,email,hiraganaFirstName,hiraganaLastName,");
            cmdStr.Append("inTrunkGroup ) VALUES(");
            // add the data to the string
            cmdStr.Append("'" + theData[0] + "'");
            cmdStr.Append(",'" + theData[1] + "'");
            cmdStr.Append(",'" + theData[2] + "'");
            cmdStr.Append(",'" + theData[3] + "'");
            cmdStr.Append(",'" + theData[4] + "'");
            cmdStr.Append(",'" + theData[5] + "'");
            cmdStr.Append(",'" + theData[6] + "'");
            cmdStr.Append(",'" + theData[7] + "'");
            cmdStr.Append(",'" + theData[8] + "'");
            cmdStr.Append(",'" + theData[9] + "'" + ")");


            try
            {
                SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), dataConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("ECaught::" + e.Message + e.StackTrace);

            }
        }

        public void WriteUserToFile(string msg)
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
                Console.WriteLine("Error Ocurred While Writing to User Log File>" + ex.ToString() );
            }
        }// public void LogFileError(string msg)

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
               // EventLog.WriteEntry(m_eventLogName, "InventoryReportParser Service FAILED trying to get a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
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
                //EventLog.WriteEntry(m_eventLogName, "InventoryReportParser Service FAILED trying to close a DB connection -- error is " + e.ToString(), EventLogEntryType.Error, 3012);
            }// catch
        }// public void CloseDataConn( ref SqlConnection dataConnection )

    }//class


}//namespace
