using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Diagnostics;
using System.Configuration;

using TruMobility.Utils.Logging;

namespace TruMobility.CDR.LD
{
    public class DbProcessor
    {

    /// <summary>
    /// class to interface to the database to get the CDR
    /// data
    /// 
    /// </summary>

        private List<CallInfo> _calls = new List<CallInfo>();  // list of users

        private string m_bworksconnectionString = ConfigurationManager.AppSettings["BWorksCdr_SQLConnectString"];
        private string m_rateconnectionString = ConfigurationManager.AppSettings["RateCenter_SQLConnectString"];

        public DbProcessor()
        {
            try
            {
                // method that initialized params from config file
                 
            }
            catch (System.Exception ex)
            {
                LogFileMgr.Instance.WriteToLogFile("DbProcessor::DbProcessor():Failed");
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            LogFileMgr.Instance.WriteToLogFile("DbProcessor::DbProcessor():IsComplete"); 
        }

        public int GetCdrData( DateTime startDate, DateTime endTime, string serviceProvider )
        {
            int mins = 0;

            StringBuilder cmdStr = new StringBuilder("SELECT userNumber, callingNumber, calledNumber, startTime, answerTime, releaseTime, DATEDIFF(mi, startTime, releaseTime) AS CallDuration, groupId, cdrId FROM BworksCdr where ( startTime > '");
            string rCmdString = startDate.ToString();
            cmdStr.Append(rCmdString);
            StringBuilder moreCmd = new StringBuilder("') AND (direction = 'Originating') AND (answerTime < '");
            string endString = endTime.ToString();
            moreCmd.Append( endString );
            moreCmd.Append("') AND ");
            moreCmd.Append("(callCategory <> 'private') AND (networkCallType <> 'tf') AND (callCategory <> 'local') AND (answerIndicator = 'yes') AND (serviceProvider LIKE '%");
            moreCmd.Append( serviceProvider + "%') order by groupId");

            cmdStr.Append(moreCmd.ToString());

            DataSet ds = new DataSet();

            try
            {
                ds = GetData(cmdStr.ToString(), m_bworksconnectionString);

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {

                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        try
                        {
                            CallInfo c = new CallInfo();
                            if (myRow.ItemArray.Length.Equals(9))
                            {
                                c.CallingNumber = (String)myRow.ItemArray[0];
                                c.CalledNumber = (String)myRow.ItemArray[2];
                                c.StartTime = (DateTime)myRow.ItemArray[3];
                                c.AnswerTime = (DateTime)myRow.ItemArray[4];
                                c.ReleaseTime = (DateTime)myRow.ItemArray[5];
                                c.CallDuration = (int)myRow.ItemArray[6];
                                c.Group = (String)myRow.ItemArray[7];
                                c.Id = (int)myRow.ItemArray[8];
                                c.LocalCall = CheckIfLocalCall(c.CallingNumber, c.CalledNumber);
                                if (!c.LocalCall)
                                {
                                    _calls.Add(c);
                                    mins = mins + c.CallDuration;
                                }
                            }
                            else
                                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetCdrData():InvalidCallParams:CallNotProcessed");

                        }
                        catch (System.Exception ex)
                        {
                            LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetCdrData():FailedTryingToGetCallData");
                            LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);

                        }
                    }
                }

            }
            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetCdrData():FailedTryingToGetTheCDRsFor" + serviceProvider);
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return mins;

        }      

        private bool CheckIfLocalCall( string callingNumber, string calledNumber )
        {
            bool isLocal = false;

            // callingNumber format is +1(10digitnumber)
            // calledNumber format is unknown
            string callingNpaNxx = callingNumber.Substring(2,6);
            string calledNpaNxx = string.Empty;

            if (calledNumber.Length.Equals(11))
            {
                // strip off the leading 1
                calledNpaNxx = calledNumber.Substring(1, 6);
            } 
            else if (calledNumber.Length.Equals(12))
            {
                // strip off the leading +1
                calledNpaNxx = calledNumber.Substring(2, 6);
            }
            else if (calledNumber.Length.Equals(7) )
            {
                // get the NPA from the calling number
                string npa = callingNumber.Substring(2,3);
                string nxx = calledNumber.Substring(0,3);
                calledNpaNxx = npa+nxx;
            }
            else if (calledNumber.Length.Equals( 10 ) )
            {
                 // easy
                calledNpaNxx = calledNumber.Substring(0,6);
            }
            else
            {
              LogFileMgr.Instance.WriteToLogFile( "DbProcessor::CheckIfLocal():InvalidPhoneNumberFormat"+ calledNumber );
            }

            isLocal = CheckIfLongDistance(calledNpaNxx, callingNpaNxx);

            // isLocal = CheckRateCenter(callingNpaNxx, calledNpaNxx );
        
            return isLocal;

        }
  
        private bool CheckRateCenter(string callingNpaNxx, string calledNpaNxx )
        {
            string callingRateCenter = string.Empty;
            string calledRateCenter = string.Empty;

            // if some npanxx, all done
            if ( callingNpaNxx.Equals(calledNpaNxx) )
                return true;

            // get the calling number rate center
            StringBuilder cmdStr = new StringBuilder("SELECT rateCenterName FROM rateCentre where npanxx ='" + callingNpaNxx +"'");
            DataSet ds = new DataSet();
            ds = GetData(cmdStr.ToString(), m_rateconnectionString);
                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {

                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        CallInfo c = new CallInfo();
                         callingRateCenter = (String)myRow.ItemArray[0];
                    }
                }

            // get the calling number rate center
            StringBuilder cmdStr2 = new StringBuilder("SELECT rateCenterName FROM rateCentre where npanxx ='" + calledNpaNxx +"'");
            DataSet ds1 = new DataSet();
            ds1 = GetData(cmdStr2.ToString(), m_rateconnectionString);

            foreach (DataTable myTable in ds1.Tables)
            {
                    
                foreach (DataRow myRow in myTable.Rows)
                {
                    CallInfo c = new CallInfo();
                    calledRateCenter = (String)myRow.ItemArray[0];
                }
            }

            if (calledRateCenter.Equals(callingRateCenter) )
                return true;
            else
                return false;
        
        }

        private bool CheckIfLongDistance(string callingNpaNxx, string calledNpaNxx)
        {
            // if some npanxx, all done
            if (callingNpaNxx.Equals(calledNpaNxx))
                return true;
            else
                return ( LDCalculator.CheckIt(callingNpaNxx, calledNpaNxx) );
        }


        /// <summary>
        /// method to execute the sql query
        /// </summary>
        /// <param name="cmdStr"></param>
        /// <returns></returns>
        private DataSet GetData(string cmdStr, string connectionStr)
        {

            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionStr))
                {
                    connection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(cmdStr, connectionStr);
                    myDaptor.Fill(ds);
                }

            }

            catch (System.Exception ex)
            {

                LogFileMgr.Instance.WriteToLogFile("DbProcessor::GetData():FailedCommand" + cmdStr);
                LogFileMgr.Instance.WriteToLogFile("ECaught:" + ex.Message + ex.StackTrace);
            }

            return ds;

        }

        public List<CallInfo> Calls
        {
            get
            {
                return this._calls;
            }

        }//_calls

    }// class

} // namespace

