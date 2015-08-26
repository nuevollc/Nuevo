namespace TruMobility.Network.Services
{
    public enum MAFCallDirection
    {
        MobileRoamerOriginated =1,
        MobileRoamerTerminated=2,
        MobileRoamerCallForward = 3,
        MobileRoamerToMobile = 4 ,
        MobileHomeOriginated = 6,
        MobileHomeTerminated = 7,
        MobileHomeCallForwardOrRoleUndefined = 8,
        MobileHomeToMobile = 9,
    }

    public class MAFSpecialFeaturesUsed
    {
        public const string None = "0";
        public const string CallForward = "1";
        public const string ThreeWayCalling = "2";
        public const string CallWaiting = "3";
        public const string SpeedCalling = "4";
        public const string IntersystemNetworkedCall = "5";
        public const string Fax = "6";
        public const string DataServices = "7";
        public const string VoiceMailBoxRecord = "8";
        public const string VoiceMailBoxRetrieve = "9";
        public const string OperatorAssisted = "A";
        public const string SMS = "B";
        public const string FeatureActivationDeactivationViaCellularNetworking = "C";
        public const string InternationalCallOriginatedTerminatedInDifferentCountriesNationalDialingPlans = "D";
        public const string NationalRegionalRoamNetworkCall = "E";
        public const string NoAirtimeChargeIncludedOnThisRecord = "F";
        public const string SatelliteVoiceService = "G";
        public const string SatelliteDataService = "H";
        public const string SatelliteFax = "I";
        public const string GSM = "P";
        public const string TDMA = "Q";
        public const string iDEN = "R";
        public const string DirectInternetConnection = "S";
        public const string PacketDataServices = "T";
    }

    public enum MAFSpecialFeaturesString
    {
        None,
        CallForward ,
        ThreeWayCalling,
        CallWaiting ,
        SpeedCalling,
        IntersystemNetworkedCall,
        Fax,
        DataServices,
        VoiceMailBoxRecord,
        VoiceMailBoxRetrieve,
        OperatorAssisted,
        SMS,
        FeatureActivationDeactivationViaCellularNetworking,
        InternationalCallOriginatedTerminatedInDifferentCountriesNationalDialingPlans,
        NationalRegionalRoamNetworkCall ,
        NoAirtimeChargeIncludedOnThisRecord,
        SatelliteVoiceService,
        SatelliteDataService,
        SatelliteFax,
        GSM,
        TDMA,
        iDEN,
        DirectInternetConnection,
        PacketDataServices, 
        Unknown
    }



}
