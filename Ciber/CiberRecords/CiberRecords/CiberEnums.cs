using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Cdr.Rating
{
    public enum CallDirection
    {
        MobileRoamerOriginated = 1,
        MobileRoamerTerminated = 2,
        MobileRoamerCallForward = 3,
        MobileRoamerToMobile = 4, // originated
        Reserved = 5,
        MobileHomeOriginated = 6,
        MobileHomeTerminated = 7,
        MobileHomeCallForward = 8,
        MobileHomeToMobile = 9
    }

    public enum CallTerminationIndicator
    {
        Abnormal = 1,
        Normal = 2,
        Unknown = 3,
        IncompleteCall = 4

    }

    public enum CurrencyType
    {
        // see section 8, table 14 for more
        USDollar = 1,
        CanadaDollar = 2,
        UKPoundSterling = 3

    }

    public enum InternationTimeZoneIndicator
    {
        GMT = 0,
        November = 01,
        Oscar = 02,
        Papa = 03,
        AtlanticTime = 04,
        EasternTime = 05,
        CentralTime = 06,
        MountainTime = 07,
        PacificTime = 08,
        AlaskanTime = 09,
        India = 30
    }

    public enum DaylightSavingsIndicator
    {
        // used to indicate whether or not daylight savings is in effect or not
        // at the serving network
        DaylightSavingsIsInEffect = 0,
        DaylightSavingsIsNotInEffect = 01,
        DaylightSavingsIsUnknown = 02
    }

    public enum AirRatePeriod
    {
        NoAirTimeIncludedOnThisRecord = 00,
        Peak = 01,
        OffPeak = 02,
        OffOffPeak = 03,
        FlatCharge = 04,
        PeakContinuedIntoAdditionalRatePeriod = 11,
        OffPeakContinuedIntoAdditionalRatePeriod = 12,
        OffOffPeakContinuedIntoAdditionalRatePeriod = 13
    }

    public enum ChargeIndicator
    {
        NoChargeApplicable =0,
        SummarySystemUseSurcharge=01,
        GrossRecieptsSurcharge=02,
        OtherChargesImposedOnTheCarrier=03,
        OtherChargesImposedByTheCarrier=04,
        VoiceMailRetrieve = 05,
        FeatureActDeactCellularNetworking=6,
        NationalRegionalRoamNetworkCall=07,
        GrossReceiptTax = 08,
        InformationUsageCharge = 09,
        AirtimeCharge = 10,
        TandemUseCharge = 11,
        CallDeliveryCharge = 12,
        TollCharges = 13,
        FeatureUseAfterHandoff=14,
        PacketDataTotalOctets=15,
        DataServices=16,
        DirectoryAssistanceCharge=17,
        DirectoryAssistanceOperatorCallCompletionCharge=18,
        PacketDataMessages=22,
        AutomaticReverseTollCharge=23,
        AirChargesT=24, // see conditions on this
        TollChargesT=25 // see conditions on this one
    
    }

    public enum FraudSubIndicator
    {
        NoFraudInvolvement=00,
        ReportOnly=01,
        Other=02
    }

    public enum AirMultiRatePeriod
    {
        NA = 0,
        SingeRatePeriod = 01,
        MultiRatePeriod = 02
    }
    public enum TollMultiRatePeriod
    {
        NA = 0,
        SingeRatePeriod = 01,
        MultiRatePeriod = 02
    }


    public enum SpecialFeaturesUsed
    {
        None = 0,
        CallForward = 1,
        ThreeWayCalling=2,
        CallWaiting=3,
        SpeedCalling=4,
        IntersystemNetworkedCall=5,
        Fax=6,
        DataServices = 7,
        VoiceMailBoxRecord = 8,
        VoiceMailBoxRetrieve =9,
        OperatorAssisted = 0x0a
        //Other = "B",
        //FeatureActivationDeactivationViaCellularNetworking = "C",
        //InternationalCallOriginatedTerminatedInDifferentCountriesNationalDialingPlans = "D",
        //NationalRegionalRoamNetworkCall = "E",
        //NoAirtimeChargeIncludedOnThisRecord = "F",
        //SatelliteVoiceService = "G",
        //SatelliteDataService = "H",
        //SatelliteFax = "I",
        //GSM = "P",
        //TDMA = "Q",
        //iDEN = "R",
        //DirectInternetConnection = "S",
        //PacketDataServices = "T"
    }

    public class ServiceCallDescriptor
    {
        public const string ThreeOneZeroCall = "310Call";
        public const string FourOneOneCall = "DIR ASST";
        public const string IncomingServiceCall = "INCOMING";
        public const string TollFree = "TOLL FREE";
        public const string NationalRegionalRoamerNetwork = "ROAM NETWK";
        public const string DailyUsageCharge = "DAILY SVC";
    }

    public enum TollRateClass
    {
        NotApplicable = 0,
        DialStation=1,
        OperatorCompleted,
        ZeroPlus,
        AutomaticPersonToPerson=4
    }

    public class CountryCode
    {
        // section 8 table 32 country codes

        public const string Afghanistan = "AFG";
        public const string AlandIslands = "ALA";

        public const string Spain = "ESP";
        public const string Turkey = "TUR";
        public const string VirginIslandsBritish = "VGB";
        public const string VirginIslands = "VIR";
        public const string UnitedStates = "USA";
        
        // ,,,

    }

    public enum TollTariffDescriptor
    {
        NotApplicable=00,
        International=01,
        InterstateInterLATA=02,
        InterstateIntraLATA=03

    }

    public enum ServiceProvider
    {
        Initialized = 0,
        Sprint,
        Verizon,
        USCellular,
        OnWaves
    }

    public enum ServiceProviderType
    {
        Initialized = 0,
        Vessel=1,
        Land=2
    }

    public enum VZWLandAirTimeRate
    {
        Year1 = 1,
        Year2 = 2,
        Year3 = 3
    }

}// namespace
