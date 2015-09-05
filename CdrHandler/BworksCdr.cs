using System;
using System.Collections.Generic;
using System.Text;

namespace Strata8.Telephony.MiddleTier.Services.CDR
{
    
    public class BworksCdr
    {
        public string recordId;
        public string serviceProvider;
        public string type;
        public string userNumber;
        public string groupNumber;
        public string direction;
        public string callingNumber;
        public string callingPresentationIndicator;
        public string calledNumber;
        public string startTime;
        public string userTimeZone;
        public string answerIndicator;
        public string answerTime;
        public string releaseTime;
        public string terminationCause;
        public string networkType;
        public string carrierIdentificationCode;
        public string dialedDigits;
        public string callCategory;
        public string networkCallType;
        public string networkTranslatedNumber;
        public string networkTranslatedGroup;
        public string releasingParty;
        public string route;
        public string networkCallId;
        public string codec;
        public string accessDeviceAddress;//26 = 27 -1    
        
        public string group; // 31 = 32 - 1

        public string chargeIndicator; // 41 = 42 - 1

        public string callingPartyCategory; // 46 = 47 - 1
        
        public string conferenceId; // 51 = 52 - 1 

        public string userId; // 120 = 121 - 1
    
    }//BworksCdr

}
