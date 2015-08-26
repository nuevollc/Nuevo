using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Cdr
{
    class OmcCdrEnums
    {
        // cdr type
        public enum RecordType
        {
            Call_Record = 1,
            SMS_Record = 2
        }

        // A, B Party Type
        public enum PartyType
        {
            Unknown = 0,
            Mobile = 1,
            Land = 2,
            Feature = 5,
            Forward = 6,
            ThreeWayCall = 8,
            CallWaiting = 9,
            Ani_Sub = 10,
            CallingCard = 11,
            Sip = 12
        }

        public enum DisconnectCodes
        {
            // see Chapter 2, page 124 of the CDR spec from StarSolutions
            No_Answer = 1,
            B_Party_Busy = 2,
            No_Page_Resp = 3,
            Bad_Digits = 4,
            Unknown_Mobile = 5,
            Vacant_Code = 6,
            B_Party_Denied = 8,
            No_Circuits = 9,
            Equipment_Fail = 10,
            A_Party_Sat_Loss = 11,
            B_Party_Sat_Loss = 12,
            A_Party_Handoff_Fail = 13,
            B_Party_Handoff_Fail = 14,
            Mishandled = 15,
            No_Voice_Channels = 18,
            Setup_Sat_Loss = 19,
            Mob_Below_Disc_Thresh = 24,
            Alert_Fail = 25,
            Signaling_Failure = 26,
            Internal_Routing_Failure = 27,
            Congestion = 28,
            Internal_Resource_Shortage = 50,
            Clone_Origination = 51,
            Database_Error = 52,
            Translate_Limit = 53,
            Protocol_Failure = 54,
            Resource_Shortage = 55,
            Network_Call_Fail = 56,
            Misrouted_LNP_Call = 57,
            Bearer_Service_Unavailable = 58,
            Teleservice_Unavailable = 59,
            Delinquent_Account = 101,
            Feature_Denied = 102,
            Toll_Denied = 103,
            Originations_Denied = 104,
            Unknown_Sub_Origination = 106,
            Bad_ESN = 107,
            International_Denied = 108,
            CFWD_Limit_Exceeded = 109,
            Bandit_Mobile = 111,
            Feature_Confirmation = 112,
            Authentication_Failure = 113,
            Call_Barred = 114,
            Roaming_Barred = 115,
            Unknown_Location_Area = 116,
            Feature_Failed = 150,
            Mobile_Inactive = 151,
            Mobile_Unavailable = 152,
            Stolen_Unit = 153,
            Subscriber_Disabled = 154,
            A_Party_Normal_Disc = 201,
            B_Party_Normal_Disc = 202,
            A_Party_Abort = 203,
            Forced_Disc = 204,
            Long_Call_Timeout = 205,
            B_Party_Abort = 206,
            Transfer = 250,
            UserDefined_1 = 300,
            UserDefined_2 = 301,
            UserDefined_3 = 303,
            UserDefined_4 = 303,
            UserDefined_5 = 304,
            UserDefined_6 = 305,
            UserDefined_7 = 306,
            UserDefined_8 = 307,
            UserDefined_9 = 308,
            Star_Solutions_Unknown

        }// disconnectCodes

    }// class CdrEnums

}//namespace

