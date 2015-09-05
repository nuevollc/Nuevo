using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Strata8.Wireless.Cdr;

namespace Strata8.Wireless.Cdr
{
    public class DisconnectCodeMgr
    {
        public string GetRecordType(string record)
        {
            string recordType = String.Empty;
            int theType = Convert.ToInt32(record);

            switch (theType)
            {
                case (int)OmcCdrEnums.RecordType.Call_Record:
                    recordType = OmcCdrEnums.RecordType.Call_Record.ToString();
                    break;
                case (int)OmcCdrEnums.RecordType.SMS_Record:
                    recordType = OmcCdrEnums.RecordType.SMS_Record.ToString();
                    break;
            }
            return recordType;
        }

        public string GetPartyType(string partyType)
        {
            string type = String.Empty;

            int theType = Convert.ToInt32(partyType);

            switch (theType)
            {
                case (int)OmcCdrEnums.PartyType.Ani_Sub:
                    type = OmcCdrEnums.PartyType.Ani_Sub.ToString();
                    break;
                case (int)OmcCdrEnums.PartyType.CallingCard:
                    type = OmcCdrEnums.PartyType.CallingCard.ToString();
                    break;
                case (int)OmcCdrEnums.PartyType.CallWaiting:
                    type = OmcCdrEnums.PartyType.CallWaiting.ToString();
                    break;
                case (int)OmcCdrEnums.PartyType.Feature:
                    type = OmcCdrEnums.PartyType.Feature.ToString();
                    break;

                case (int)OmcCdrEnums.PartyType.Forward:
                    type = OmcCdrEnums.PartyType.Forward.ToString();
                    break;
                case (int)OmcCdrEnums.PartyType.Land:
                    type = OmcCdrEnums.PartyType.Land.ToString();
                    break;
                case (int)OmcCdrEnums.PartyType.Mobile:
                    type = OmcCdrEnums.PartyType.Mobile.ToString();
                    break;
                case (int)OmcCdrEnums.PartyType.Sip:
                    type = OmcCdrEnums.PartyType.Sip.ToString();
                    break;

                case (int)OmcCdrEnums.PartyType.ThreeWayCall:
                    type = OmcCdrEnums.PartyType.ThreeWayCall.ToString();
                    break;
                case (int)OmcCdrEnums.PartyType.Unknown:
                    type = OmcCdrEnums.PartyType.Unknown.ToString();
                    break;
                default:
                    type = OmcCdrEnums.PartyType.Unknown.ToString();
                    break;
            }

            return type;
        }

        public string GetDisconnectMessage(string disconnectCode)
        {
            string d = String.Empty;

            int theType = Convert.ToInt32( disconnectCode );

            switch (theType)
            {
                case (int)OmcCdrEnums.DisconnectCodes.A_Party_Abort:
                    d = OmcCdrEnums.DisconnectCodes.A_Party_Abort.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.A_Party_Handoff_Fail:
                    d = OmcCdrEnums.DisconnectCodes.A_Party_Handoff_Fail.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.A_Party_Normal_Disc:
                    d = OmcCdrEnums.DisconnectCodes.A_Party_Normal_Disc.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.A_Party_Sat_Loss:
                    d = OmcCdrEnums.DisconnectCodes.A_Party_Sat_Loss.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Alert_Fail:
                    d = OmcCdrEnums.DisconnectCodes.Alert_Fail.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Authentication_Failure:
                    d = OmcCdrEnums.DisconnectCodes.Authentication_Failure.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.B_Party_Abort:
                    d = OmcCdrEnums.DisconnectCodes.B_Party_Abort.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.B_Party_Busy:
                    d = OmcCdrEnums.DisconnectCodes.B_Party_Busy.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.B_Party_Denied:
                    d = OmcCdrEnums.DisconnectCodes.B_Party_Denied.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.B_Party_Handoff_Fail:
                    d = OmcCdrEnums.DisconnectCodes.B_Party_Handoff_Fail.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.B_Party_Normal_Disc:
                    d = OmcCdrEnums.DisconnectCodes.B_Party_Normal_Disc.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.B_Party_Sat_Loss:
                    d = OmcCdrEnums.DisconnectCodes.B_Party_Sat_Loss.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Bad_Digits:
                    d = OmcCdrEnums.DisconnectCodes.Bad_Digits.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Bad_ESN:
                    d = OmcCdrEnums.DisconnectCodes.Bad_ESN.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Bandit_Mobile:
                    d = OmcCdrEnums.DisconnectCodes.Bandit_Mobile.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Bearer_Service_Unavailable:
                    d = OmcCdrEnums.DisconnectCodes.Bearer_Service_Unavailable.ToString();
                    break;

                case (int)OmcCdrEnums.DisconnectCodes.Call_Barred:
                    d = OmcCdrEnums.DisconnectCodes.Call_Barred.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.CFWD_Limit_Exceeded:
                    d = OmcCdrEnums.DisconnectCodes.CFWD_Limit_Exceeded.ToString();
                    break;

                case (int)OmcCdrEnums.DisconnectCodes.Clone_Origination:
                    d = OmcCdrEnums.DisconnectCodes.Clone_Origination.ToString();
                    break;

                case (int)OmcCdrEnums.DisconnectCodes.Congestion:
                    d = OmcCdrEnums.DisconnectCodes.Congestion.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Database_Error:
                    d = OmcCdrEnums.DisconnectCodes.Database_Error.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Delinquent_Account:
                    d = OmcCdrEnums.DisconnectCodes.Delinquent_Account.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Equipment_Fail:
                    d = OmcCdrEnums.DisconnectCodes.Equipment_Fail.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Feature_Confirmation:
                    d = OmcCdrEnums.DisconnectCodes.Feature_Confirmation.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Feature_Denied:
                    d = OmcCdrEnums.DisconnectCodes.Feature_Denied.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Feature_Failed:
                    d = OmcCdrEnums.DisconnectCodes.Feature_Failed.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Forced_Disc:
                    d = OmcCdrEnums.DisconnectCodes.Forced_Disc.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Internal_Resource_Shortage:
                    d = OmcCdrEnums.DisconnectCodes.Internal_Resource_Shortage.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Internal_Routing_Failure:
                    d = OmcCdrEnums.DisconnectCodes.Internal_Routing_Failure.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.International_Denied:
                    d = OmcCdrEnums.DisconnectCodes.International_Denied.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Long_Call_Timeout:
                    d = OmcCdrEnums.DisconnectCodes.Long_Call_Timeout.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Mishandled:
                    d = OmcCdrEnums.DisconnectCodes.Mishandled.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Misrouted_LNP_Call:
                    d = OmcCdrEnums.DisconnectCodes.Misrouted_LNP_Call.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Mob_Below_Disc_Thresh:
                    d = OmcCdrEnums.DisconnectCodes.Mob_Below_Disc_Thresh.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Mobile_Inactive:
                    d = OmcCdrEnums.DisconnectCodes.Mobile_Inactive.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Mobile_Unavailable:
                    d = OmcCdrEnums.DisconnectCodes.Mobile_Unavailable.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Network_Call_Fail:
                    d = OmcCdrEnums.DisconnectCodes.Network_Call_Fail.ToString();
                    break;

                case (int)OmcCdrEnums.DisconnectCodes.No_Answer:
                    d = OmcCdrEnums.DisconnectCodes.No_Answer.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.No_Circuits:
                    d = OmcCdrEnums.DisconnectCodes.No_Circuits.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.No_Page_Resp:
                    d = OmcCdrEnums.DisconnectCodes.No_Page_Resp.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.No_Voice_Channels:
                    d = OmcCdrEnums.DisconnectCodes.No_Voice_Channels.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Originations_Denied:
                    d = OmcCdrEnums.DisconnectCodes.Originations_Denied.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Protocol_Failure:
                    d = OmcCdrEnums.DisconnectCodes.Protocol_Failure.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Resource_Shortage:
                    d = OmcCdrEnums.DisconnectCodes.Resource_Shortage.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Roaming_Barred:
                    d = OmcCdrEnums.DisconnectCodes.Roaming_Barred.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Setup_Sat_Loss:
                    d = OmcCdrEnums.DisconnectCodes.Setup_Sat_Loss.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Signaling_Failure:
                    d = OmcCdrEnums.DisconnectCodes.Signaling_Failure.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Stolen_Unit:
                    d = OmcCdrEnums.DisconnectCodes.Stolen_Unit.ToString();
                    break;

                case (int)OmcCdrEnums.DisconnectCodes.Subscriber_Disabled:
                    d = OmcCdrEnums.DisconnectCodes.Subscriber_Disabled.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Teleservice_Unavailable:
                    d = OmcCdrEnums.DisconnectCodes.Teleservice_Unavailable.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Toll_Denied:
                    d = OmcCdrEnums.DisconnectCodes.Toll_Denied.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Transfer:
                    d = OmcCdrEnums.DisconnectCodes.Transfer.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Translate_Limit:
                    d = OmcCdrEnums.DisconnectCodes.Translate_Limit.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Unknown_Location_Area:
                    d = OmcCdrEnums.DisconnectCodes.Unknown_Location_Area.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Unknown_Mobile:
                    d = OmcCdrEnums.DisconnectCodes.Unknown_Mobile.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.Unknown_Sub_Origination:
                    d = OmcCdrEnums.DisconnectCodes.Unknown_Sub_Origination.ToString();
                    break;
                case (int)OmcCdrEnums.DisconnectCodes.UserDefined_1:
                    d = OmcCdrEnums.DisconnectCodes.UserDefined_1.ToString();
                    break;

                default:
                    d = OmcCdrEnums.DisconnectCodes.Star_Solutions_Unknown.ToString();
                    break;

            }

            return d;

        }


    }//class
}
