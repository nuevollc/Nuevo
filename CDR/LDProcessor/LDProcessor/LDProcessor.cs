using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using TruMobility.Utils.Logging;

namespace TruMobility.CDR.LD
{
    /// <summary>
    /// class to process CDRs for each service provider in the config file
    /// for each service provider, the cdrs are processed and if the call 
    /// is a long distance call based on the fact that the call is outside of
    /// the rate center then the duration and dollar amount is calculated
    /// </summary>
    public class LDProcessor
    {
        private DbProcessor _db = new DbProcessor();

        private int _startMonth = Convert.ToInt32(ConfigurationManager.AppSettings["StartMonth"]);
        private int _endMonth = Convert.ToInt32(ConfigurationManager.AppSettings["EndMonth"]);
        private string _sp = ConfigurationManager.AppSettings["ServiceProvider"];
        public void ProcessCalls()
        {
            DateTime s = new DateTime( DateTime.Now.Year, _startMonth, 1 );
            DateTime e = new DateTime( DateTime.Now.Year, _endMonth, 1 );

            // add code for each service provider here
            int ldMins = this.GetData( s, e, _sp );

            // log the calls
            List<CallInfo> c = _db.Calls;
            double total = 0.0;
            foreach (CallInfo ci in c)
            {
                double f = ci.CallDuration * 0.025;
                LogFileMgr.Instance.WriteToFile(@"C:\Users\RobertL\Documents\apps\logs\" + _sp + "Calls" + ".csv", ci.Id + "," + ci.CallingNumber + ","
                    + ci.CalledNumber + "," + ci.StartTime.ToShortDateString()
                    + "," + ci.CallDuration + "," + f.ToString() + "," + ci.Group + "," + ci.LocalCall);
                total = f + total;
            }

            LogFileMgr.Instance.WriteToFile(@"C:\Users\RobertL\Documents\apps\logs\" + _sp + "LDMins" + ".txt", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " " + "LongDistanceMins:: " + ldMins.ToString() + "Cost:" + total.ToString());

        }

        private int GetData(DateTime startDate, DateTime endDate, string serviceProvider)
        {
            int mins = _db.GetCdrData(startDate, endDate, serviceProvider);

            return mins;
        }
    }
    
}
