using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using TruMobility.Utils.Logging;


namespace TruMobility.Reporting.CDR
{
    public class ExcelMgr 
    {
        public static string comma = ",";
        private static CdrDbProcessor _db = new CdrDbProcessor(); 

        /// <summary>
        /// method to create a list of all calls per user
        /// </summary>
        /// <param name="reportList"></param>
        /// <param name="fileName"></param>
        public static void CreateUserCSVReport(List<UserCallReport> reportList, string fileName)
        {
            // create the header
            StringBuilder sb = new StringBuilder("UserNumber, Direction, CallingNumber,CalledNumber, CallDate, AnswerIndicator, CallDuration");
            LogFileMgr.Instance.WriteToFile(fileName, sb.ToString());

            // create the data
            foreach (UserCallReport userReport in reportList)
            {
                List<CallInfo> ciList = userReport.Calls;

                // for each user get the call detail
                foreach (CallInfo ci in ciList)  //(CallInfo ci in userReport.Calls)
                {
                    // let's get the extension for each user
                    // string ext = _db.GetExtension(userReport.UserNumber);
                    StringBuilder b = new StringBuilder(ci.UserNumber + comma + ci.Direction + comma + ci.CallingNumber + comma + ci.CalledNumber + comma + ci.CallDate);
                    b.Append(comma + ci.AnswerIndicator + comma + ci.Duration);
                    LogFileMgr.Instance.WriteToFile(fileName, b.ToString());
                } // for each user in the group

            } // for each group

        } // user csv call report

        /// <summary>
        /// method to create a call summary per user
        /// </summary>
        /// <param name="reportList"></param>
        /// <param name="fileName"></param>
        public static void CreateCSV(List<UserCallReport> reportList, string fileName)
        {

            // create the header
            StringBuilder sb = new StringBuilder("Name, UserNumber, InboundCalls, OutboundCalls,TotalCalls, TotalCallTime");
            LogFileMgr.Instance.WriteToFile(fileName, sb.ToString());

            // create the data
            foreach (UserCallReport userReport in reportList)
            {
                // let's get the extension for each user
                string name = _db.GetName( userReport.UserNumber );
                StringBuilder b = new StringBuilder(name + comma + userReport.UserNumber + comma + userReport.TotalInboundCalls + comma + userReport.TotalOutboundCalls + comma + userReport.TotalCalls +
                    comma + userReport.TotalCallTime);
                LogFileMgr.Instance.WriteToFile(fileName, b.ToString());

                // for each user get the call detail
                //foreach (CallInfo ci in userReport.Calls)
                //{
                //    StringBuilder b = new StringBuilder(ext + comma + userReport.UserNumber + comma + ci.CalledNumber + comma + ci.CallingNumber + comma + ci.Direction +
                //        comma + ci.CallDate.ToString("s") + comma + ci.Duration.ToString("g"));
                //    LogFileMgr.Instance.WriteToFile(fileName, b.ToString());
                //}
            }

        }
    }//class
}
