using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections; 
using System.Data; 

using TruMobility.Utils.Logging;

namespace TruMobility.Reporting.CDR
{

    /// <summary>
    /// methods to create a call report for each user
    /// and for the group of users
    /// </summary>
    class CdrReportProcessor
    {
        private List<string> _phoneList = new List<string>();  // list of users
        private CdrDbProcessor _db = new CdrDbProcessor();

        private HashSet<UserCallReport> _users = new HashSet<UserCallReport>();

        /// <summary>
        /// public method to create a call report stat for each user
        /// the default interval starts daily at midnight up until time now
        /// the following parameters are calculated for each user in the list
        /// 
        /// Total Outbound
        /// Total Inbound
        /// Total Call Duration
        /// Average Call Duration
        /// 
        /// </summary>
        public CallReport CreateUserCallReport(List<Subscriber> subList, DateTime reportTime, DateTime endTime)
        {
            // our base reference time that we calculate from midnight every day
            DateTime referenceTime = new DateTime(DateTime.Now.Year, reportTime.Month , reportTime.Day, reportTime.Hour, 0, 0);

            // call report list for all users
            List<UserCallReport> userCallReportList = new List<UserCallReport>();

            // initiate our group cumulative report object; contains the running totals for the group of users
            CumulativeReport gcr = new CumulativeReport();

            // get the time that we are running at
            string timeString = DateTime.Now.ToString();

            // our reference time that we calculate from 
            DateTime timeNow = DateTime.Now;

            // the call report with the userCallReportList and the cumulative totals
            CallReport cr = new CallReport();
            cr.StartTime = referenceTime;
            cr.EndTime = endTime;
            cr.ReportTime = timeNow;

            int indx = 0;

            // get the call date for each user in the list
            foreach (Subscriber s in subList)
            {
                // get all cdrs for this user from the reference time to present
                DataSet ds = _db.GetCdrsForPhoneNumber(s.PhoneNumber, reportTime, endTime);

                // For each User the following calculations are performed :  userId : field 3
                // 
                // Total Inbound Calls : total number of inbound calls determined from the "direction" field 5 (terminating)
                // Total Outbound Calls : total number of outbound calls determined from the direction field 5 (originating)
                // Total Call Time : the total of inbound/outbound call times calculated from each of the CDRs for this user
                // Call Time : answerTime - releaseTime ( field 13 - field 12 )
                // Average Call Time : Call Time / Total Number of Calls
                // 

                // user call report some inits
                UserCallReport ucr = new UserCallReport();
                ucr.UserNumber = s.PhoneNumber;
                ucr.Group = s.Group;
                ucr.ServiceProvider = s.ServiceProvider;

                int totalIn = 0;
                int totalOut = 0;
                int totalIntlOrig = 0;
                TimeSpan totalCallDuration = new TimeSpan();

                // for each CDR we update our report values
                foreach (DataTable myTable in ds.Tables)
                {
                    ucr.TotalCalls = myTable.Rows.Count.ToString();

                    foreach (DataRow myRow in myTable.Rows)
                    {
                        if (myRow.ItemArray[6].Equals("Originating"))
                        {
                            totalOut++;
                            // log the originating  international call
                            if (myRow.ItemArray[19].Equals("internat"))
                                totalIntlOrig++;
                        }
                        else if (myRow.ItemArray[6].Equals("Terminating"))
                            totalIn++;
                        // get the call duration for this call
                        DateTime d1 = (DateTime)myRow.ItemArray[10];
                        DateTime d2 = (DateTime)myRow.ItemArray[14];
                        TimeSpan callDuration = d2.Subtract(d1);

                        // calculate total call duration for this user
                        totalCallDuration = totalCallDuration.Add(callDuration);

                    }

                }

                // user totals
                ucr.TotalInboundCalls = totalIn.ToString();
                ucr.TotalOutboundCalls = totalOut.ToString();
                ucr.TotalInternationalCalls = totalIntlOrig.ToString();
                ucr.TotalCallTime = totalCallDuration.ToString();
                if ((totalIn + totalOut) != 0)
                {
                    double avg = totalCallDuration.TotalMinutes / (totalIn + totalOut);
                    ucr.AverageCallTime = String.Format("{0}", avg);
                }

                indx++;

                // store the userReport in the list
                userCallReportList.Add(ucr);

                // maintain the group totals here
                gcr.TotalCalls = gcr.TotalCalls + Convert.ToInt32( ucr.TotalCalls );
                gcr.TotalCallTime = gcr.TotalCallTime.Add(totalCallDuration);
                gcr.TotalInboundCalls = gcr.TotalInboundCalls + Convert.ToInt32( ucr.TotalInboundCalls );
                gcr.TotalOutboundCalls = gcr.TotalOutboundCalls + Convert.ToInt32( ucr.TotalOutboundCalls );
                gcr.TotalInternationalCalls = gcr.TotalInternationalCalls + Convert.ToInt32( ucr.TotalInternationalCalls );

            }// get next user

            // copy the usercall report list
            cr.UserCallReportList = userCallReportList;
 
            if ((gcr.TotalCalls) != 0)
            {
                double gavg = gcr.TotalCallTime.TotalMinutes / gcr.TotalCalls;
                gcr.AverageCallTime = gavg.ToString("F3");
            }
            // copy the group cumulative total as well
            cr.TotalsReport = gcr;

            return cr;
        }

/// <summary>
/// method to create a group call report
/// we get the unique groupids in the system and then for each group we get 
/// the users in the group and create a call report for each user in the group.
/// from this, we can generate either a group summary report or a group report including
/// the details of each user.
/// </summary>
/// <param name="reportTime"></param>
/// <returns></returns>
        public List<CallReport> CreateGroupCallReport(DateTime reportTime, DateTime endTime)
        {

            // call report for each group in the system
            List<CallReport> groupCallReportList = new List<CallReport>();

            Hashtable spInfo = new Hashtable();

            List<string> groupList = _db.GetGroupIds(reportTime, endTime);

            // get the call report for each group
            foreach (string groupId in groupList)
            {
                // get the list of users in each group
                List<Subscriber> userList = _db.GetUsersInGroup(groupId, reportTime, endTime);

                // get the call report for the list of users in the group
                CallReport groupCallReport = this.CreateUserCallReport(userList, reportTime, endTime);
                groupCallReport.GroupId = groupId;
                if ( userList.Count > 0 )
                    groupCallReport.ServiceProvider = userList[0].ServiceProvider;

                // add the group call report to our list
                groupCallReportList.Add(groupCallReport);

            }

            return groupCallReportList;
        }

        public Hashtable CreateSPReport(List<CallReport> groupCallReport)
        {
            Hashtable sp = new Hashtable();

            // get the call report for each group
            foreach (CallReport cr in groupCallReport)
            {
                if (sp.ContainsKey(cr.ServiceProvider))
                {
                    // add to the data
                    CumulativeReport r = (CumulativeReport)sp[cr.ServiceProvider];
                    r.TotalCalls = cr.TotalsReport.TotalCalls + r.TotalCalls;
                    r.TotalCallTime = cr.TotalsReport.TotalCallTime + r.TotalCallTime;
                    r.TotalInboundCalls = cr.TotalsReport.TotalInboundCalls + r.TotalInboundCalls;
                    r.TotalOutboundCalls = cr.TotalsReport.TotalOutboundCalls + r.TotalOutboundCalls;
                    r.TotalInternationalCalls = cr.TotalsReport.TotalInternationalCalls + r.TotalInternationalCalls;
                    // update the hashtable
                    sp[cr.ServiceProvider] = r;
                }
                else
                {
                    // create the key, value
                    CumulativeReport r = new CumulativeReport();
                    r.TotalCalls = cr.TotalsReport.TotalCalls;
                    r.TotalCallTime = cr.TotalsReport.TotalCallTime;
                    r.TotalInboundCalls = cr.TotalsReport.TotalInboundCalls;
                    r.TotalOutboundCalls = cr.TotalsReport.TotalOutboundCalls;
                    r.TotalInternationalCalls = cr.TotalsReport.TotalInternationalCalls;

                    // update the hashtable
                    sp.Add(cr.ServiceProvider, r);
                }

            }

            return sp;

        }

    }
}
