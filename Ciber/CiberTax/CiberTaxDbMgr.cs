using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;
using Strata8.Wireless.Utils;

namespace EPCS.Ciber.Tax
{
    public class CiberTaxDbMgr
    {        
        private static string _connectionString = String.Empty;
        /// <summary>
        /// class to handle the database parameters related to the CIBER file and 
        /// the batch sequence numbers.  Every batch submitted needs to have the next batch
        /// sequence number for Syniverse to process these.  The batch file also needs to be
        /// incremented to make error handling/tracking easier for Strata8 to resolve
        /// </summary>
        /// 
        public CiberTaxDbMgr()
        {
            _connectionString = ConfigurationManager.AppSettings["WirelessTdsSQLConnectString"]; 
        }

    }
}
