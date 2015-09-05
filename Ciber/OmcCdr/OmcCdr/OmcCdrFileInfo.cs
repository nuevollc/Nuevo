using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Cdr
{
    public class OmcCdrFileInfo
    {
        private string fileName = "NONE";
        private byte downloaded = 0;
        private byte storedInDb = 0;
        private byte fileMerged = 0; // indicates whether or not file was merged from another file
        private byte ciberCreated = 0;
        private DateTime dateDownloaded;
        private DateTime dateStoredInDb;
        private DateTime dateFileMerged;
        private DateTime dateCiberCreated;


        // accesors
        public byte Downloaded
        {
            get
            {
                return downloaded;
            }
            set
            {
                downloaded = value;
            }
        }
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }
        public byte StoredInDb
        {
            get
            {
                return storedInDb;
            }
            set
            {
                storedInDb = value;
            }
        }
        public byte CiberCreated
        {
            get
            {
                return ciberCreated;
            }
            set
            {
                ciberCreated = value;
            }
        }
        public byte FileMerged
        {
            get
            {
                return fileMerged;
            }
            set
            {
                fileMerged = value;
            }
        }
        public DateTime DateDownloaded
        {
            get
            {
                return dateDownloaded;
            }
            set
            {
                dateDownloaded = value;
            }
        }
        public DateTime DateStoredInDb
        {
            get
            {
                return dateStoredInDb;
            }
            set
            {
                dateStoredInDb = value;
            }
        }
        public DateTime DateFileMerged
        {
            get
            {
                return dateFileMerged;
            }
            set
            {
                dateFileMerged = value;
            }
        }
        public DateTime DateCiberCreated
        {
            get
            {
                return dateCiberCreated;
            }
            set
            {
                dateCiberCreated = value;
            }
        }

    }
}
