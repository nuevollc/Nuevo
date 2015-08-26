using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Reporting.CDR
{

    /// <summary>
    /// Call statistics for a user
    /// </summary>
    public class UserCallReport
    {
        private string _userId = String.Empty;
        private string _userNumber = String.Empty;
        private string _totalOutboundCalls = "0";
        private string _totalInboundCalls = "0";
        private string _totalInternationalCalls = "0";
        private string _totalCallTime = "0";
        private string _totalInboundCallTime = "0";
        private string _totalOutboundCallTime = "0";
        private string _averageCallTime = "0";
        private string _totalCalls = "0";
        private string _totalCumulativeCalls = "0";
        private string _userName = String.Empty;
        private string _userExtension = String.Empty;
        private string _groupId = String.Empty;
        private string _serviceProviderId = String.Empty;
        private string _firstName = String.Empty;
        private string _lastName = String.Empty;

        // list of call details for the user
        private List<CallInfo> _callReportList = new List<CallInfo>();

        public static int CompareByCumulativeCalls(UserCallReport x, UserCallReport y)
        {

            if (x.TotalCumulativeCalls == null)
            {
                if (y.TotalCumulativeCalls == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y.TotalCumulativeCalls == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // lengths of the two strings.
                    //
                    int intx = Convert.ToInt32(x.TotalCumulativeCalls);
                    int inty = Convert.ToInt32(y.TotalCumulativeCalls);

                    int retval = intx.CompareTo(inty);

                    if (retval != 0)
                    {
                        // If the strings are not of equal,
                        // the longer string is greater.
                        //
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length,
                        // sort them with ordinary string comparison.
                        // done above
                        return retval;
                    }
                }
            }
        }

        public static int CompareByTotalCalls(UserCallReport x, UserCallReport y)
        {

            if (x.TotalCalls == null)
            {
                if (y.TotalCalls == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y.TotalCalls == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // lengths of the two strings.
                    //
                    int intx = Convert.ToInt32(x.TotalCalls);
                    int inty = Convert.ToInt32(y.TotalCalls);

                    int retval = intx.CompareTo(inty);

                    if (retval != 0)
                    {
                        // If the strings are not of equal,
                        // the longer string is greater.
                        //
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length,
                        // sort them with ordinary string comparison.
                        // done above
                        return retval;
                    }
                }
            }
        }

        //assesors
        public string UserId
        {

            get
            {
                return this._userId;
            }
            set
            {
                _userId = value;

            }

        }//UserId       
        public string Group 
        {

            get
            {
                return this._groupId;
            }
            set
            {
                _groupId = value;

            }

        }//Group 
        public string ServiceProvider
        {

            get
            {
                return this._serviceProviderId;
            }
            set
            {
                _serviceProviderId = value;

            }

        }//ServiceProvider

        public string UserName
        {

            get
            {
                return this._userName;
            }
            set
            {
                _userName = value;

            }

        }//UserName
        public string FirstName
        {

            get
            {
                return this._firstName;
            }
            set
            {
                _firstName = value;

            }

        }//FirstName
        public string LastName
        {

            get
            {
                return this._lastName;
            }
            set
            {
                _lastName = value;

            }

        }//LastName

        public string UserNumber
        {

            get
            {
                return this._userNumber;
            }
            set
            {
                _userNumber = value;

            }

        }//_userNumber

        public string UserExtension
        {

            get
            {
                return this._userExtension;
            }
            set
            {
                _userExtension = value;

            }

        }//UserExtension

        public string TotalOutboundCalls
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

        public string TotalInboundCalls
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

        public string TotalInternationalCalls
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

        public string TotalCallTime
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

        public string TotalInboundCallTime
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

        public string TotalOutboundCallTime
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

        public string AverageCallTime
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

        public string TotalCalls
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

        public string TotalCumulativeCalls
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

        public List<CallInfo> Calls
        {

            get
            {
                return this._callReportList;
            }
            set
            {
                _callReportList = value;

            }

        }//list of calls for the user


    }// Call report

}//namespace

