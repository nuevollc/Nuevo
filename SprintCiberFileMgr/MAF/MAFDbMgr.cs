using System;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Strata8.Wireless.Cdr.Rating;// ciber record code

namespace TruMobility.Network.Services
{
    public class MAFDbMgr
    {

        private static string _connectionString = String.Empty;

        /// <summary>
        /// class to handle the database parameters related to the CIBER file and 
        /// the batch sequence numbers.  Every batch submitted needs to have the next batch
        /// sequence number for Syniverse to process these.  The batch file also needs to be
        /// incremented to make error handling/tracking easier for Strata8 to resolve
        /// </summary>
        /// 
        public MAFDbMgr()
        {
            _connectionString = ConfigurationManager.AppSettings["WirelessCDRSQLConnectString"]; 
        }


        private void LogError(string msg)
        {
            FileWriter.Instance.WriteToLogFile(msg);
            
        }// public void LogFileError(string msg)

        public void InsertMafRecord( Record22 r22 )
        {
            //FileWriter.Instance.WriteToLogFile("-NFORMATIONAL::MAFDbMgr::InsertMafRecord()Entering");

            StringBuilder commandStr = new StringBuilder("INSERT INTO SprintMafRecord ");                
            commandStr.Append(" ( HomeCarrierSidBid, CalledNumber, CallDate, ServingPlace, ServingStateProvince, CallerId, AirConnectTime,");
            commandStr.Append(" AirChargeableTime, InitialCellSite, TollChargeableTime, MsidIndicator, Msid, MsisdnMdnLength, Msisdn, EsnImeiIndicator,");
            commandStr.Append(" CallDirection, EsnImei, ServingCarrierSidBid, CallCompletionIndicator, CallTerminationIndicator, CallerIdLength,");
            commandStr.Append(" CalledNumberLength, Tldn, TldnLength, LocRoutingNumber, LocRoutingNumberLength, TimeZoneIndicator, DaylightSavingsIndicator,");
            commandStr.Append(" AirElapsedTime, SpecialFeaturesUsed, TollConnectTime, TollNetworkCarrierId, TollElapsedTime, TollRatingPointLengthIndicator,");
            commandStr.Append(" TollRatingPoint ) VALUES(" );

            try
            {
                using (SqlConnection dataConnection = new SqlConnection(_connectionString))
                {
                    // add try catch block around setting up the command            
                    StringBuilder sb = new StringBuilder();
                    DateTime dt = new DateTime( DateTime.Now.Year, Convert.ToInt16( r22.CallDate.Substring(2, 2)),
                       Convert.ToInt16(r22.CallDate.Substring(4, 2)), Convert.ToInt16( r22.AirConnectTime.Substring(0, 2)),
                       Convert.ToInt16(r22.AirConnectTime.Substring(2, 2)), Convert.ToInt16( r22.AirConnectTime.Substring(4, 2) ) );
                    
                    // get the call direction
                    String theCallDirection = GetCallDirection(r22.CallDirection);
                    String theSpecialFeature = GetSpecialFeatures(r22.SpecialFeaturesUsed);

                    // for now going to log the air connect time to find out why not converting time right (AM time, 00:00:00 AM )    
                    //FileWriter.Instance.WriteToLogFile("MAFDbMgr::InsertMafRecord():CallDate:AirConnectTime:AirConnectTime(0,2):" + r22.CallDate + " : " +
                      //  r22.AirConnectTime + " : "  + r22.AirConnectTime.Substring(0, 2) + " : " + " DateTime::" + dt.ToString());

                    sb.Append("'" + r22.HomeCarrierSidBid + "'"); 
                    sb.Append(",'" + r22.CalledNumberDigits + "'");
                    sb.Append(",'" + dt.ToString() +"'"); 
                    sb.Append(",'" + r22.ServingPlace + "'");
                    sb.Append(",'" + r22.ServingStateProvince + "'");
                    sb.Append(",'" + r22.CallerId + "'");
                    sb.Append(",'" + r22.AirConnectTime + "'");
                    sb.Append(",'" + r22.AirChargeableTime + "'");
                    sb.Append(",'" + r22.InitialCellSite + "'");
                    sb.Append(",'" + r22.TollChargeableTime + "'");
                    sb.Append(",'" + r22.MsidIndicator + "'");
                    sb.Append(",'" + r22.Msid + "'");
                    sb.Append(",'" + r22.MsisdnMdnLength + "'");
                    sb.Append(",'" + r22.MsisdnMdn + "'");
                    sb.Append(",'" + r22.EsnUimidImeiMeidIndicator + "'");
                    sb.Append(",'" + theCallDirection + "'");
                    sb.Append(",'" + r22.EsnUimidImeiMeid + "'");
                    sb.Append(",'" + r22.ServingCarrierSidBid + "'");
                    sb.Append(",'" + r22.CallCompletionIndicator + "'");
                    sb.Append(",'" + r22.CallTerminationIndicator + "'");
                    sb.Append(",'" + r22.CallerIdLength + "'");
                    sb.Append(",'" + r22.CalledNumberLength + "'");
                    sb.Append(",'" + r22.Tldn + "'");
                    sb.Append(",'" + r22.TldnLength + "'");
                    sb.Append(",'" + r22.LocationRoutingNumber + "'");
                    sb.Append(",'" + r22.LocationRoutingNumberLengthIndicator + "'");
                    sb.Append(",'" + r22.TimeZoneIndicator + "'");
                    sb.Append(",'" + r22.DaylightSavingIndicator + "'");
                    sb.Append(",'" + r22.AirElapsedTime + "'");
                    sb.Append(",'" + theSpecialFeature + "'");
                    sb.Append(",'" + r22.TollConnectTime + "'");
                    sb.Append(",'" + r22.TollNetworkCarrierId + "'");
                    sb.Append(",'" + r22.TollElapsedTime + "'");
                    sb.Append(",'" + r22.TollRatingPointLengthIndicator + "'");
                    sb.Append(",'" + r22.TollRatingPoint + "'" + ")");

                    // write the CDR to the database
                    string tc = commandStr.ToString() + sb.ToString();
                    try
                    {
                        dataConnection.Open();
                        SqlCommand sqlCommand = new SqlCommand(tc, dataConnection);
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        FileWriter.Instance.WriteToLogFile("MAFDbMgr::InsertMafRecord():ECaught:" + ex.Message + ex.StackTrace);
                    }

                }//using


            }//try
            catch (Exception ex)
            {
                FileWriter.Instance.WriteToLogFile("MAFDbMgr::InsertMafRecord():ECaught:" + ex.Message + ex.StackTrace);
            }

            // FileWriter.Instance.WriteToLogFile("-NFORMATIONAL::MAFDbMgr::InsertMafRecord()Exiting");

        }

        public void UpdateFileStatus( string fname )
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    StringBuilder cmdStr = new StringBuilder("UPDATE FilesDownloaded SET storedInDb = 1, dateStoredInDb = '");

                    cmdStr.Append(DateTime.Now.ToString() + "'" + " WHERE fileName = '");
                    cmdStr.Append(fname + "'");

                    SqlCommand sqlCommand = new SqlCommand(cmdStr.ToString(), connection);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (System.Exception e)
            {
                FileWriter.Instance.WriteToLogFile("MAFDbMgr::UpdateFileStatus():FailedTryingToUpdateTheFileNameInTheDB:" + fname);
                FileWriter.Instance.WriteToLogFile( "ECaught:" + e.Message + e.StackTrace );
            }

        }// UpdateFileStatus()


        public string GetCallDirection(string dirCall)
        {
            string theCallDirection = string.Empty;

            int call = Convert.ToInt16( dirCall );
            switch (call)
            {
                case (int)MAFCallDirection.MobileHomeCallForwardOrRoleUndefined:
                    {
                        theCallDirection = MAFCallDirection.MobileHomeCallForwardOrRoleUndefined.ToString();
                        break;
                    }
                case (int)MAFCallDirection.MobileHomeOriginated:
                    {
                        theCallDirection = MAFCallDirection.MobileHomeOriginated.ToString();
                        break; 
                    }
                case (int)MAFCallDirection.MobileHomeTerminated:
                    {
                        theCallDirection = MAFCallDirection.MobileHomeTerminated.ToString();
                        break; 
                    }
                case (int)MAFCallDirection.MobileHomeToMobile:
                    {
                        theCallDirection = MAFCallDirection.MobileHomeToMobile.ToString();
                        break;
                    }
                case (int)MAFCallDirection.MobileRoamerCallForward:
                    {
                        theCallDirection = MAFCallDirection.MobileRoamerCallForward.ToString();
                        break;
                    }
                case (int)MAFCallDirection.MobileRoamerOriginated:
                    {
                        theCallDirection = MAFCallDirection.MobileRoamerOriginated.ToString();
                        break;
                    }
                case (int)MAFCallDirection.MobileRoamerTerminated:
                    {
                        theCallDirection = MAFCallDirection.MobileRoamerTerminated.ToString();
                        break;
                    }

                case (int)MAFCallDirection.MobileRoamerToMobile:
                    {
                        theCallDirection = MAFCallDirection.MobileRoamerToMobile.ToString();
                        break;
                    }
                default:
                    theCallDirection = string.Empty;
                    break;
            }

            return theCallDirection;
        }

        public string GetSpecialFeatures(string f)
        {
            MAFSpecialFeaturesString theFeature = MAFSpecialFeaturesString.Unknown;
            string fT = f.Trim();
            switch ( fT )
            { 
                case MAFSpecialFeaturesUsed.CallForward:
                    {
                        theFeature = MAFSpecialFeaturesString.CallForward;
                        break;
                    }
                case MAFSpecialFeaturesUsed.CallWaiting:
                    {
                        theFeature = MAFSpecialFeaturesString.CallWaiting;
                        break;
                    }
                case MAFSpecialFeaturesUsed.DataServices:
                    {
                        theFeature = MAFSpecialFeaturesString.DataServices;
                        break;
                    }
                case MAFSpecialFeaturesUsed.DirectInternetConnection:
                    {
                        theFeature = MAFSpecialFeaturesString.DirectInternetConnection;
                        break;
                    }
                case MAFSpecialFeaturesUsed.Fax:
                    {
                        theFeature = MAFSpecialFeaturesString.Fax;
                        break;
                    }
                case MAFSpecialFeaturesUsed.FeatureActivationDeactivationViaCellularNetworking:
                    {
                        theFeature = MAFSpecialFeaturesString.FeatureActivationDeactivationViaCellularNetworking;
                        break;
                    }
                case MAFSpecialFeaturesUsed.GSM:
                    {
                        theFeature = MAFSpecialFeaturesString.GSM;
                        break;
                    }

                case MAFSpecialFeaturesUsed.iDEN:
                    {
                        theFeature = MAFSpecialFeaturesString.iDEN;
                        break;
                    }
                case MAFSpecialFeaturesUsed.InternationalCallOriginatedTerminatedInDifferentCountriesNationalDialingPlans:
                    {
                        theFeature = MAFSpecialFeaturesString.InternationalCallOriginatedTerminatedInDifferentCountriesNationalDialingPlans;
                        break;
                    }
                case MAFSpecialFeaturesUsed.IntersystemNetworkedCall:
                    {
                        theFeature = MAFSpecialFeaturesString.IntersystemNetworkedCall;
                        break;
                    }

                case MAFSpecialFeaturesUsed.NationalRegionalRoamNetworkCall:
                    {
                        theFeature = MAFSpecialFeaturesString.NationalRegionalRoamNetworkCall;
                        break;
                    }
                case MAFSpecialFeaturesUsed.NoAirtimeChargeIncludedOnThisRecord:
                    {
                        theFeature = MAFSpecialFeaturesString.NoAirtimeChargeIncludedOnThisRecord;
                        break;
                    }
                case MAFSpecialFeaturesUsed.None:
                    {
                        theFeature = MAFSpecialFeaturesString.None;
                        break;
                    }
                case MAFSpecialFeaturesUsed.OperatorAssisted:
                    {
                        theFeature = MAFSpecialFeaturesString.OperatorAssisted;
                        break;
                    }

                case (string) MAFSpecialFeaturesUsed.SMS:
                    {
                        theFeature = MAFSpecialFeaturesString.SMS;
                        break;
                    }
                case MAFSpecialFeaturesUsed.PacketDataServices:
                    {
                        theFeature = MAFSpecialFeaturesString.PacketDataServices;
                        break;
                    }
                case MAFSpecialFeaturesUsed.SatelliteDataService:
                    {
                        theFeature = MAFSpecialFeaturesString.SatelliteDataService;
                        break;
                    }
                case MAFSpecialFeaturesUsed.SatelliteFax:
                    {
                        theFeature = MAFSpecialFeaturesString.SatelliteFax;
                        break;
                    }

                case MAFSpecialFeaturesUsed.SatelliteVoiceService:
                    {
                        theFeature = MAFSpecialFeaturesString.SatelliteVoiceService;
                        break;
                    }

                case MAFSpecialFeaturesUsed.SpeedCalling:
                    {
                        theFeature = MAFSpecialFeaturesString.SpeedCalling;
                        break;
                    }
                case MAFSpecialFeaturesUsed.TDMA:
                    {
                        theFeature = MAFSpecialFeaturesString.TDMA;
                        break;
                    }
                case MAFSpecialFeaturesUsed.ThreeWayCalling:
                    {
                        theFeature = MAFSpecialFeaturesString.ThreeWayCalling;
                        break;
                    }

                case MAFSpecialFeaturesUsed.VoiceMailBoxRecord:
                    {
                        theFeature = MAFSpecialFeaturesString.VoiceMailBoxRecord;
                        break;
                    }
                case MAFSpecialFeaturesUsed.VoiceMailBoxRetrieve:
                    {
                        theFeature = MAFSpecialFeaturesString.VoiceMailBoxRetrieve;
                        break;
                    }
                default:
                    theFeature = MAFSpecialFeaturesString.Unknown;
                    break;
            }

            return theFeature.ToString();
        }


    }//class


}//namespace

