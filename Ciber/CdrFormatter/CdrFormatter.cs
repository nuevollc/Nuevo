using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using Strata8.Wireless.Utils;
using Strata8.Wireless.Cdr;
using Strata8.Wireless.Cdr.Rating;

namespace Strata8.Voip.Cdr
{

    /// <summary>
    /// class used to convert from the OMC cdr format to the Bworks CDR format
    /// // this object has a 
    /// </summary>
    public class CdrFormatter
    {
        // line1, start and end lines
        private const string line1 = "version=14.2 encoding=ISO-8859-1";
        private const string startLine = "460474845c4d1e20090126203000.1380-080000,,Start";
        private const string endLine = "460563845c4d1e20090126204500.1160-080000,,End";
        private const string startFileName = "BW-CDR-";
        private const string exFilename = "BW-CDR-OMCCDFILENAMEHERE-2-845c4d1e-017296.csv";
        private const string endFileName = "-2-845c4d1e-017296.csv";
        
        private string m_cdrPath = ConfigurationManager.AppSettings["BworksCdrFileFolder"];

        // used to manage the cdr list
        private static CdrMgr m_cdrMgr = CdrMgr.Instance;

        private static FileWriter m_fileWriter = FileWriter.Instance;

        /// <summary>
        /// method used to create a CDR based on the MSC/OMC CDR and the 
        /// CIBER record type 22.
        /// </summary>
        /// <param name="oCdr"></param>
        /// <param name="r22"></param>
        /// <returns></returns>
        public Bcdr CreateCdr(OmcCdr oCdr, Record22 r22 )
        {
            // populate the bworks cdr here and return it
            Bcdr bCdr = new Bcdr(oCdr);
            return bCdr;
        }
        
        /// <summary>
        /// method used to create a CDR from an OMC/MSC CDR
        /// </summary>
        /// <param name="omcCdr"></param>
        /// <returns></returns>
        public Bcdr CreateCdr( OmcCdr omcCdr )
        {
            // populate the bworks cdr here and return it
            Bcdr bCdr = new Bcdr( omcCdr );
            return bCdr;
        }

        /// <summary>
        /// public method to create a bcdr and add it to the list
        /// to be written to the cdr file later
        /// </summary>
        /// <param name="o"></param>
        public void CreateAndAddCdr( OmcCdr o )
        {
            Bcdr bCdr = new Bcdr( o );
            m_cdrMgr.AddCdr(bCdr);
        
        }

        /// <summary>
        /// method that clears the CDR list held by the CdrMgr
        /// </summary>
        /// <param name="cdrFileName"></param>
        /// <param name="cdr"></param>
        /// 
        public void ClearCdrList()
        {
            m_cdrMgr.ClearCdrList();
        }

        public void WriteCdrToFile( string cdrFileName, Bcdr cdr )
        {
            m_fileWriter.WriteToCdrFile(cdrFileName, cdr.ToString() );
        }

        public bool CreateCdrFile( string cdrFileName )
        {
            // if there are CDRs to process, otherwise don't create the file
            if (m_cdrMgr.CdrList.Count > 0)
            {
                string s = ParseCdrFileName(cdrFileName);
                // create the header, cdr files, trailer
                string fname = this.m_cdrPath + startFileName + s + endFileName;

                CreateCdrHeader(fname);
                WriteCdrListToFile(fname, m_cdrMgr.CdrList);
                CreateCdrTrailer(fname);
                return true;
            }
            else
            {
                m_fileWriter.WriteToLogFile("CdrFormatter::CreateCdrFile():NoCDRsDetectedInFile: " + cdrFileName);
                return false;
            }

        }

        private string ParseCdrFileName(string fileName)
        {
            string newFileName = String.Empty;

            if (fileName.Contains('\\'))
            {// move the file
                int index = fileName.LastIndexOf('\\');
                newFileName = fileName.Substring(index + 1);

            }
            else
            {
                newFileName = fileName;
            }

            if (newFileName.Contains('.'))
            {
                int i = newFileName.IndexOf('.');
                newFileName = newFileName.Substring(0, i);
            }


            return newFileName;

        }
        
        /// <summary>
        /// public method used to write a list of cdrs to a cdr file
        /// </summary>
        /// <param name="cdrFileName"></param>
        /// <param name="cdrList"></param>
        public void WriteCdrListToFile(string cdrFileName, List<Bcdr> cdrList)
        {
            foreach (Bcdr b in cdrList)
            {
                WriteCdrToFile(cdrFileName, b);
            }
        }

        public void CreateCdrHeader( string cdrFileName )
        {
            m_fileWriter.WriteToCdrFile( cdrFileName, Line1 );
            m_fileWriter.WriteToCdrFile( cdrFileName, StartLine );
        }

        public void CreateCdrTrailer( string cdrFileName )
        {
            m_fileWriter.WriteToCdrFile( cdrFileName, EndLine );
        }

        public string Line1
        {
            get
            {
                return line1;
            }

        }
        public string StartLine
        {
            get
            {
                return startLine;
            }
        }
        public string EndLine
        {
            get
            {
                return endLine;
            }
        }       
        public string StartFileName
        {
            get
            {
                return startFileName;
            }
        }
        public string EndFileName
        {
            get
            {
                return endFileName;
            }
        }
    }
}
