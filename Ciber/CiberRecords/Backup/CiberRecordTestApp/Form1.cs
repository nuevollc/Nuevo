using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Configuration;

using Strata8.Wireless.Cdr;
using Strata8.Wireless.Cdr.Ciber;
using Strata8.Did;
using Strata8.Wireless;
using Strata8.Voip.Cdr;
using Strata8.Wireless.Utils; 

using Strata8.Wireless.Cdr.Reporting;

namespace CiberRecordTestApp
{
    public partial class Form1 : Form
    {
        // object to read the CIBER records from Syniverse
        private CiberCdrReader cr = null;

        private OmcFtpHandler m_omcCdrFtpHandler = new OmcFtpHandler();
        private OmcCiberHandler m_ciberHandler = new OmcCiberHandler();
        private WirelessCdrProcessor r = new WirelessCdrProcessor();

        private FileHandler m_FileHandler = new FileHandler();

        //private
        private OmcCdrHandler m_omcCdrHandler = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void ReadCiberRecord_Click(object sender, EventArgs e)
        {

            string cdrFile = System.Configuration.ConfigurationManager.AppSettings["CiberFileToRead"];
            cr = new CiberCdrReader();
            cr.ProcessCiberFile( cdrFile );

        }



        private void FtpTest_Click(object sender, EventArgs e)
        {
            //UriBuilder ub = new UriBuilder(@"ftp://cdrs.strata8.net/starsolutions/omc");
            //Uri u = ub.Uri;
            //string m_FTPServerDirectory = ConfigurationManager.AppSettings["FTPServerDirectory"];           
            
            //string user = ConfigurationManager.AppSettings["FTPUserName"];
            //string pass = ConfigurationManager.AppSettings["FTPPwd"];
            //UriBuilder ub = new UriBuilder(m_FTPServerDirectory);
            //Uri u = ub.Uri;
            //List<string> directoryList = FtpUtils.ListDirectoryOnFtpSite( u, user,pass);
            //UriBuilder ub1 = new UriBuilder(m_FTPServerDirectory + @"/" + directoryList.Last());
            //Uri u1 = ub1.Uri;

            //string fileData = FtpUtils.GetFileFromSite(u1, user, pass);
            m_omcCdrFtpHandler.StartProcessing();


        }

        private void FTPStop_Click(object sender, EventArgs e)
        {
            m_omcCdrFtpHandler.StopProcessing();
        }

        private void StartOmcCdrProcessorSvc_Click(object sender, EventArgs e)
        {
            if (m_omcCdrHandler == null)
                m_omcCdrHandler = new OmcCdrHandler();
            m_omcCdrHandler.StartProcessing();

        }

        private void OmcCdrHandlerStop_Click(object sender, EventArgs e)
        {
            if ( m_omcCdrHandler != null)

                m_omcCdrHandler.StopProcessing();
        }

        private void ReadSyniverseSampleFile_Click(object sender, EventArgs e)
        {
            string cdrFile = System.Configuration.ConfigurationManager.AppSettings["SyniverseTestFile"];
            cr = new CiberCdrReader();
            cr.ProcessCiberFile( cdrFile );
        }

        private void ImportNpaNxxData_Click(object sender, EventArgs e)
        {
            NpaNxxReader n = new NpaNxxReader();
            n.ProcessTheFile();
        }

        private void ProcessMulitpleSidBids_Click(object sender, EventArgs e)
        {

            // method to parse the CDRs
            // string cdrFile = System.Configuration.ConfigurationManager.AppSettings["OmcCdrFileToRead"];

            m_ciberHandler.StartProcessing();

            //List<OmcCdr> theCdrs = ch.ProcessOmcCdrFile(cdrFile);
            //CiberFileCreator.Instance.ProcessCallRecords(theCdrs);
            //CiberFileCreator.Instance.CreateCiberRecords();

            // housekeeping, update our database that we have processed this file
            // UpdateFileInfo(parsedFileName);

            // move the file with the full path name 
            // this.MoveTheFile(fName);
        }

        private void StopProcessingCdrToCiber_Click(object sender, EventArgs e)
        {
            if (!m_ciberHandler.Equals(null))
                m_ciberHandler.StopProcessing();
        }

        private void CreateBworksCdr_Click(object sender, EventArgs e)
        {

            DateTime d = DateTime.Now;
            Console.WriteLine(d.ToUniversalTime());
            Console.WriteLine(d.ToString("yyyyMMddHHmmsss.fff"));
            
            CdrFormatter f = new CdrFormatter();

            // method to parse the CDRs  
            string cdrFile = System.Configuration.ConfigurationManager.AppSettings["OmcCdrFileToRead"];
            string fname = ParseFileName(cdrFile);

            List<OmcCdr> theCdrs = m_ciberHandler.ProcessOmcCdrFile(cdrFile);

            // bworks file name format : BW-CDR-20090126124500-2-845c4d1e-017296.csv
            // keep the BW-CDR-OMCCDFILENAMEHERE-2-845c4d1e--017296.csv format for our file format
            string s = @"d:\apps\data\out\BW-CDR-MSC-20090126124500-2-845c4d1e-017296.csv";

            // create the cdr list
            foreach (OmcCdr o in theCdrs)
            {
                Bcdr b = new Bcdr(o);
                f.CreateAndAddCdr(o);
            }

            f.CreateCdrFile(fname);
            f.ClearCdrList();

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
               newFileName = fileName;
            }

            if (newFileName.Contains('.'))
            {
                int i = newFileName.IndexOf('.');
                newFileName = newFileName.Substring(0, i );
            }
         
            
            return newFileName;

        }

        private void CreateOWReport_Click(object sender, EventArgs e)
        {

            if (!r.Equals(null))
                r.StartProcessing();
        }

        private void StopOWReportSvc_Click(object sender, EventArgs e)
        {
            if (!r.Equals(null))
                r.StopProcessing();
        }

        private void DirListAndParseOmcCdrFiles_Click(object sender, EventArgs e)
        {
            if (!m_FileHandler.Equals(null))
                m_FileHandler.StartProcessing();
        }

        private void StopDirListAndParseOmcCdrFiles_Click(object sender, EventArgs e)
        {
            if (!m_FileHandler.Equals(null))
                m_FileHandler.StartProcessing();
        }

        private void PicoCellReport_Click(object sender, EventArgs e)
        {
            PicoCdrProcessor p = new PicoCdrProcessor();
            p.StartProcessing();
        }


    }
}
