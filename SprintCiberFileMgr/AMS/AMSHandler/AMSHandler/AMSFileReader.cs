
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using TruMobility.Utils.Logging;

namespace TruMobility.Sprint.AMS
{
    public class AMSFileReader
    {
        char[] m_sep;
        char[] m_trim;
        char[] m_backslash;
        char[] m_dot;

        AMSDbMgr m_dbMgr = new AMSDbMgr();
        LogFileMgr m_logger = LogFileMgr.Instance;

        public AMSFileReader()
        {

            m_sep = new char[] { ',' };
            m_trim = new char[] { '"' };
            m_backslash = new char[] { '\\' };
            m_dot = new char[] { '.' };

        }


        /// <summary>
        /// exposed method to process the ams data file
        /// reads the file line by line, splits it, creates the ams record
        /// and updates the database with the record
        /// </summary>
        /// <param name="fileName"></param>
        public void ProcessTheFile(string fileName)
        {

            try
            {

                using (StreamReader sr = new StreamReader(fileName))
                {
                    int recordCount = 1;

                    string line;
                    while ( (line = sr.ReadLine()) != null )
                    {
                        string[] amsString = line.Split(m_sep);

                        AMSRecord amsRecord = new AMSRecord(amsString);
                        AddAmsRecord(amsRecord);

                        // increment our record count
                        recordCount++;

                    }// while loop - end of file


                }//using sr

            }//try
            catch (SystemException se)
            {
                m_logger.WriteToLogFile("-NEXCEPTION::MAFFileReader::ProcessTheFile():ECaught:" + se.Message + se.StackTrace);

            }

            //m_logger.WriteToLogFile("-NFORMATIONAL::MAFFileReader::ProcessTheFile()Exiting");

        }// ReadTheFile


        /// <summary>
        /// method to add the AMS record to the database
        /// </summary>
        /// <param name="r">AMS data record to be added to the database</param>
        private void AddAmsRecord(AMSRecord r)
        {

            m_dbMgr.AddAmsRecord(r);

        }


    }//AMSFileReader

}//ns
