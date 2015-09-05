using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace Strata8.Wireless.Cdr
{

    /// <summary>
    /// class to read the directory of the OMC files once a day
    /// and create one file from the 24 hourly files that are produced
    /// by the MSC on a daily basis
    /// </summary>
    public class FileReadWriteMgr
    {
        private static string m_FileName = String.Empty;

        // path to the directory of CDRs to process
        // not terminated with a "/", @"d:\processedCdrs\starsolutions\omc"
        private string m_CdrFileDirectory = ConfigurationManager.AppSettings["OmcCdrFileDirectory"];

        // directory where the processed CDRs are copied to, these are the files used to create the 
        // one file.  This parameter is terminated with a "/"  e.g., @"d:\processedCdrs\starsolutions\omc\forCiber\"
        private string m_filemovePath = ConfigurationManager.AppSettings["OmcCdrMoveFileDirectory"];

        // @"d:\processedCdrs\starsolutions\omc\toCiber\";
        private string m_filemoveCdrPath = ConfigurationManager.AppSettings["CdrToCiberMoveDirectory"];
        private string m_fileReadWriteMgrLogFile = ConfigurationManager.AppSettings["ReadWriteManagerLogFile"];
        private string m_MergedCdrFileTempDirectory = ConfigurationManager.AppSettings["MergedCdrFileTempDirectory"];

        private const string m_terminator = @"</Telos_CDRS>";
        private const string m_beginning = @"<Telos_CDRS>"; 

        // objects used to check file processing
        private OmcCdrDb m_db = new OmcCdrDb();

        public FileReadWriteMgr()
        {        

        }

        /// <summary>
        /// public method to process the files.  does a directory listing, gets the files in the directory
        /// processes the files, moves the files to the processed directory and finally places this file
        /// in the processedCdrs directory where it is picked up by the ciber processor to create ciber records
        /// </summary>
        public void ProcessFiles()
        {
             bool rFirstFile = true;
            // this is called by the filehandler to process all the files in the directory
            // we assign the new cdr file name based on the current time, all files are merged into this file.
            DateTime m_date = DateTime.Now;
            string cdrFileName = m_MergedCdrFileTempDirectory + "MSC-CDR-" + m_date.ToString("yyyyMMddHHmmss") + ".xml";

            string[] theFiles = GetDirectoryListing();

            // if there are files to process
            if (theFiles.Length > 0)
            {

                foreach (string fileName in theFiles)
                {
                    // make sure we have not already merged this file into our CDR file going to the CIBER processing
                    string parsedFileName = this.ParseFileName(fileName);
                    if (!this.CheckDbBeforeProcessingFile(parsedFileName))
                    {
                        ProcessTheFile(fileName, cdrFileName, ref rFirstFile);
                        MoveTheFile(fileName);
                        // update the file stats to avoid processing again
                        UpdateFileInfo(parsedFileName);
                    }
                    else
                    {
                        WriteToLogFile("FileReadWriteMgr::ProcessFiles():FileAlreadyProcessed-" + parsedFileName);
                    }

                }

                // terminate the XML
                WriteToFile(cdrFileName, m_terminator);

                // move our CDR file when done processing all files
                MoveTheNewCdrFileForCiberProcessing(cdrFileName);
            
            } // if no files, no processing done

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

        private void ProcessTheFile(string fileName, string mergedCdrFileName, ref bool firstFileProcessed )
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
                            if ( firstFileProcessed )
                            {    // write the line to the new file
                                WriteToFile(mergedCdrFileName, line);
                            }
                        }
                        else if (cnt == 1)
                        {
                            if (firstFileProcessed)
                            {
                                WriteToFile(mergedCdrFileName, line);
                                firstFileProcessed = false;
                            }
                        }
                        else
                        {
                            // don't terminate the XML until the last file
                            if (!line.Contains("</Telos_CDRS>"))
                                // write the 2nd and beyond lines 
                                WriteToFile(mergedCdrFileName, line);
                        }

                        cnt++;
                    }
                }// end of the file
            }
            catch (SystemException ex)
            {
                WriteToLogFile( ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        private string ParseFileName(string fileName)
        {
            string newFileName = String.Empty;

            if (fileName.Contains('\\'))
            {// move the file
                int index = fileName.LastIndexOf('\\');
                newFileName = fileName.Substring(index + 1);
            }
            else
            {
                return newFileName = fileName;
            }

            return newFileName;

        }

        private void MoveTheFile(string fileName)
        {
            string newFileName = ParseFileName(fileName);

            try
            {
                File.Move(fileName, m_filemovePath + newFileName);
                WriteToLogFile( "FileReadWriteMgr::MoveTheFile():INFORMATIONAL:MovedFile: " + fileName + " ToLocation: " + m_filemovePath + newFileName);
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                WriteToLogFile( "FileReadWriteMgr::MoveTheFile():ERROR:Could not move file: " + fileName + " to location: " + m_filemovePath + newFileName + "error>" + ex.ToString());
            }

        }// MoveTheFile()

        private void MoveTheNewCdrFileForCiberProcessing(string fileName)
        {
            string newFileName = ParseFileName(fileName);

            try
            {
                File.Move(fileName, m_filemoveCdrPath + newFileName);
                WriteToLogFile("FileReadWriteMgr::MoveTheNewCdrFileForCiberProcessing():INFORMATIONAL:MovedFile: " + fileName + " ToLocation: " + m_filemoveCdrPath + newFileName);
            }
            catch (System.IO.IOException ex)
            {// file already exists?
                WriteToLogFile("FileReadWriteMgr::MoveTheNewCdrFileForCiberProcessing():IOException:Could not move file: " + fileName + " ToLocation: " + m_filemoveCdrPath + newFileName + "error>" + ex.ToString());
            }

        }// MoveTheNewCdrFileForCiberProcessing()

        /// <summary>
        /// method to make sure that we have not created CIBER records based on 
        /// this file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool CheckDbBeforeProcessingFile(string fileName)
        {
            OmcCdrFileInfo info = m_db.GetOmcCdrInfo(fileName);
            // if we have already merged this file
            if (info.FileMerged == 1)
                return true;

            // otherwise we need to merge the file
            return false;

        }//CheckDbBeforeDownloadingFile

        private void UpdateFileInfo(string fileName)
        {
            // update our file info to reflect that we have also
            // stored the CDRs in the database
            m_db.UpdateDateFileMerged(fileName);

        }// UpdateFileInfo

        private void WriteToLogFile(string msg)
        {
            string s = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + msg;
            WriteToFile(m_fileReadWriteMgrLogFile, s);

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
