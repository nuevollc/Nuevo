using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Reporting.CDR
{
    /// <summary>
    /// Class that contains the cumulative call report stats
    /// </summary>
    public class CumulativeReport
    {
        private int _totalOutboundCalls = 0;
        private int _totalInboundCalls = 0;
        private int _totalInternationalCalls = 0;
        private TimeSpan _totalCallTime = new TimeSpan();
        private TimeSpan _totalInboundCallTime = new TimeSpan();
        private TimeSpan _totalOutboundCallTime = new TimeSpan();
        private string _averageCallTime = String.Empty;
        private int _totalCalls = 0;
        private int _totalCumulativeCalls = 0;

        //assesors
        public int TotalCalls
        {

            get
            {
                return this._totalCalls;
            }
            set
            {
                _totalCalls = value;

            }

        }//TotalCalls

        public int TotalCumulativeCalls
        {

            get
            {
                return this._totalCumulativeCalls;
            }
            set
            {
                _totalCumulativeCalls = value;

            }

        }//TotalCumulativeCalls

        public int TotalOutboundCalls
        {

            get
            {
                return this._totalOutboundCalls;
            }
            set
            {
                _totalOutboundCalls = value;

            }

        }//TotalOutboundCalls

        public int TotalInboundCalls
        {

            get
            {
                return this._totalInboundCalls;
            }
            set
            {
                _totalInboundCalls = value;

            }

        }//TotalInboundCalls

        public int TotalInternationalCalls
        {

            get
            {
                return this._totalInternationalCalls;
            }
            set
            {
                _totalInternationalCalls = value;

            }

        }//_totalInternationalCalls

        public TimeSpan TotalCallTime
        {

            get
            {
                return this._totalCallTime;
            }
            set
            {
                _totalCallTime = value;

            }

        }//TotalCallTime

        public TimeSpan TotalInboundCallTime
        {

            get
            {
                return this._totalInboundCallTime;
            }
            set
            {
                _totalInboundCallTime = value;

            }

        }//TotalInboundCallTime

        public TimeSpan TotalOutboundCallTime
        {

            get
            {
                return this._totalOutboundCallTime;
            }
            set
            {
                _totalOutboundCallTime = value;

            }

        }//TotalOutboundCallTime

        public String AverageCallTime
        {

            get
            {
                return this._averageCallTime;
            }
            set
            {
                _averageCallTime = value;

            }

        }//AverageCallTime


    }
}
