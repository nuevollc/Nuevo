 
using System.Linq; 
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.Collections;
using System.Data;
using System;

namespace Nuevo.CallCruncher.CDR.FTP
{
    /// <summary>
    /// public class to hold the FTP site information 
    /// including login credentials and file to post
    /// </summary>
    public class FtpSiteInfo
    {
        private string sp;
        private string site;
        private string username;
        private string password;
        private string filename;

        public FtpSiteInfo()
        { }

        // accessors
        public string ServiceProvider
        {
            get
            {
                return this.sp;
            }
            set
            {
                sp = value;
            }
        }
        public string Site
        {
            get
            {
                return this.site;
            }
            set
            {
                site = value;
            }
        }
        public string Username
        {
            get
            {
                return this.username;
            }
            set
            {
                username = value;
            }
        }
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                password = value;
            }
        }// Password        
        public string Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                filename = value;
            }
        }// Password

    }// 

}// namespace
