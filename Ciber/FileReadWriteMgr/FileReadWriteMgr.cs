using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Strata8.Wireless.Utils;

namespace Strata8.Wireless.Utils
{

    /// <summary>
    /// class to read the directory of the OMC files once a day
    /// and create one file from the 24 hourly files that are produced
    /// by the MSC on a daily basis
    /// </summary>
    public class FileReadWriteMgr
    {

        private static string m_FileName = String.Empty;
        private static int m_firstFile = 0;

        // not terminated with a "/"
        private string m_CdrFileDirectory = @"d:\processedCdrs\starsolutions\omc";

        // terminated with a "/"
        private string m_filemovePath = @"d:\processedCdrs\starsolutions\omc\forCiber\";
        private string m_filemoveCdrPath = @"d:\processedCdrs\starsolutions\omc\toCiber\";
        private string m_CdrFile = String.Empty;
        private DateTime m_date = DateTime.Now;
        private string m_fileReadWriteMgrLogFile = @"d:\apps\logs\ReadWriteMgrLog.log";
        private const string m_terminator = @"</Telos_CDRS>";
        private const string m_beginning = @"<Telos_CDRS>";
        private static bool m_beginXml = false;

        public FileReadWriteMgr()
        {
            m_CdrFile = @"d:\apps\temp\MSC-CDR-" + m_date.ToString("yyyyMMddHHmmss") + ".xml";

        }

        /// <summary>
        /// public method to process the files.  does a directory listing, gets the files in the directory
        /// processes the files, moves the files to the processed directory and finally places this file
        /// in the processedCdrs directory where it is picked up by the ciber processor to create ciber records
        /// </summary>
        public void ProcessFiles()
        {
            string[] theFiles = GetDirectoryListing();
            foreach ( string fileName in theFiles )
            {
                ProcessTheFile(fileName);
                MoveTheFile(fileName);
            }
            
            // terminate the XML
            WriteToFile(m_CdrFile, m_terminator);

            // move our CDR file when done processing all files
            MoveTheNewCdrFileForProcessing(m_CdrFile);

        }

        private string[] GetDirectoryListing()
        {
            string[] theFiles = null;
            if ( Directory.Exists( m_CdrFileDirectory ) )
            { 
                // get the list of files
                theFiles = Directory.GetFiles(m_CdrFileDirectory );
            }

            return theFiles;

        }

        private void ProcessTheFile(string fileName)
        {
            // process the file
            // read and write to the one file here
            try
            {
                int cnt = 0;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {

                        // logic for our first file being processed, write lines 1, 2
                        if (cnt == 0)
                        { // new file being processed, first line
                            // if the first file we are processing, write the header
                            if (m_firstFile == 0)
                            {    // write the line to the new file
                                WriteToFile(m_CdrFile, line);
                            }
                        }
                        else if (cnt == 1)
                        {
                            if (m_firstFile == 0)
                            {
                                WriteToFile(m_CdrFile, line);
                                m_firstFile++;
                            }
                        }
                        else
                        {
                            // don't terminate the XML until the last file
                            if (!line.Contains("</Telos_CDRS>"))
                                // write the 2nd and beyond lines 
                                WriteToFile(m_CdrFile, line);
                        }

                        cnt++;
                    }
                }// end of the file
            }
            catch (SystemException ex)
            {
                WriteToFile(m_fileReadWriteMgrLogFile, ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        private void MoveTheFile(string fileName)
        {
            // move the file
            int index = fileName.LastIndexOf('\\');
            string newFileName = fileName.Substring(index + 1);

            // add a date time stamp to the filename; removed for now
            DateTime aTime = DateTime.Now;
            // newFileName += "."+ aTime.Month.ToString() + aTime.Day.ToString() + aTime.Year.ToString() + "-" + aTime.Hour.ToString() + aTime.Minute.ToString() + aTime.Second.ToString() + aTime.Millisecond.ToString();
            try
            {
                File.Move(fileName, m_filemovePath + newFileName);
                WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::MoveTheFile():INFORMATIONAL:MovedFile: " + fileName + " ToLocation: " + m_filemovePath + newFileName);
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::MoveTheFile():ERROR:Could not move file: " + fileName + " to location: " + m_filemovePath + newFileName + "error>" + ex.ToString());
            }

        }// MoveTheFile()
        private void MoveTheNewCdrFileForProcessing(string fileName)
        {
            // move the file
            int index = fileName.LastIndexOf('\\');
            string newFileName = fileName.Substring(index + 1);

            // add a date time stamp to the filename; removed for now
            DateTime aTime = DateTime.Now;
            // newFileName += "."+ aTime.Month.ToString() + aTime.Day.ToString() + aTime.Year.ToString() + "-" + aTime.Hour.ToString() + aTime.Minute.ToString() + aTime.Second.ToString() + aTime.Millisecond.ToString();
            try
            {
                File.Move(fileName, m_filemoveCdrPath + newFileName);
                WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::MoveTheFile():INFORMATIONAL:MovedFile: " + fileName + " ToLocation: " + m_filemovePath + newFileName);
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::MoveTheFile():ERROR:Could not move file: " + fileName + " to location: " + m_filemovePath + newFileName + "error>" + ex.ToString());
            }

        }// MoveTheFile()


        private void WriteToLogFile(string msg)
        {
            WriteToFile(m_fileReadWriteMgrLogFile, msg);
        }

        /// <summary>
        /// private method used to write to the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="msg"></param>
        private void WriteToFile(string fileName, string msg)
        {
            // if log file does not exist, we create it, otherwise we append to it.     
            FileStream fs = null;
            StreamWriter sw = null;

            if ( !File.Exists( fileName ) )
            {
                try
                {
                    fs = File.Create( fileName );
                }
                catch (System.Exception ex)
                {
                    WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::WriteToFile():ECaughtWhileTryingToCreateLogFile:" + ex.ToString() + " Log file name is>");
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
                    WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::WriteToFile():ErrorWhileCreateFileStreamForLogFile:" + ex.ToString() + " Log file name is>");
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
                WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::WriteToFile():ECaughtWhileTryingToWriteToLogFile:" + ex.Message + ex.StackTrace);
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
                WriteToFile(m_fileReadWriteMgrLogFile, "FileReadWriteMgr::WriteToFile():ECaughtWhileTryingToCloseLogFile:" + ex.Message + ex.StackTrace);
                return;
            }

        }// private void WriteToFile(string msg)

    }
}
