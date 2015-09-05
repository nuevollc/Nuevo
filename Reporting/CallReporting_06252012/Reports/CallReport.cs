using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Reporting.CDR
{
    /// <summary>
    /// Class that contains the call reports for each user and the cumulative
    /// report totals for the users within a group or put another way
    /// the call report contains individual call reports for each user and the 
    /// total for all users within the group.
    /// </summary>
    public class CallReport
    {
        // time report was generated
        private DateTime _reportTime;
        private DateTime _startTime; // start time of the report 
        private DateTime _endTime; // end time of the report 
        private string _groupId; // the group the user belongs to
        private string _serviceProviderId;
                     
        // call report list for all users
        private List<UserCallReport> _callReportList = new List<UserCallReport>();

        // initiate our group cumulative report object; contains the running totals for the group of users
        private CumulativeReport _totalsReport = new CumulativeReport();

        //assesors
        public List<UserCallReport> UserCallReportList
        {

            get
            {
                return this._callReportList;
            }
            set
            {
                this._callReportList = value;
            }

        } //CallReportList

        public CumulativeReport TotalsReport
        {

            get
            {
                return this._totalsReport;
            }

            set
            {
                this._totalsReport = value;
            }

        } //TotalsReport

        public DateTime ReportTime
        {

            get
            {
                return this._reportTime;
            }

            set
            {
                this._reportTime = value;
            }

        } //ReportTime

        public DateTime StartTime
        {

            get
            {
                return this._startTime;
            }

            set
            {
                this._startTime = value;
            }

        } //ReportTime

        public DateTime EndTime
        {

            get
            {
                return this._endTime;
            }

            set
            {
                this._endTime = value;
            }

        } //ReportTime

        public string GroupId
        {

            get
            {
                return this._groupId;
            }

            set
            {
                this._groupId = value;
            }

        } //GroupId       
 
        public string ServiceProvider
        {

            get
            {
                return this._serviceProviderId;
            }

            set
            {
                this._serviceProviderId = value;
            }

        } //ServiceProvider

    }// CallReport

} //ns
