using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Reporting.CDR
{

    /// <summary>
    /// service provider cumulative totals 
    /// </summary>
    public class SPReport
    {

        public CumulativeReport cr = new CumulativeReport();
        public String _sp = String.Empty;

        // a list of the groups in the service provider
        public List<string> _groups = new List<string>();

    }
}
