using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Cdr.Rating
{
    /// <summary>
    /// this is the CIBER record type 22
    /// This record is the Air and Toll Charges Record
    /// currently there are no value checks on the fields even though there are constraints
    /// on what values the fields can take, this is to be implemented (TBI).
    /// The length of this record is : 547.
    /// </summary>
    public class Record22
    {
        // required fields
        private string m_newline = "\r\n";
        private char zero_pad = Convert.ToChar("0");
        private char blank_pad = Convert.ToChar(" ");
        private string zero = "0";
        private string m_one = "1";

        private string recordType = "22";
        private string returnCode;
        private string ciberRecordReturnReasonCode;
        private string invalidFieldIdentifier;
        private string homeCarrierSIDBID;
        private string msidIndicator; // 1 = ITU-T E.212 MIN or IMSI; 2 = Mobile Identification Number (MIN)

        // used to identify a subscribers home carrier and the subs account
        // the MSID can be either the ITU-T E.212 MIN (IMSI) or the MIN
        private string msid; 

        // if the MSISDN/MDN is not available on the switch call detail record, this field must be equal to zero.
        // this field is used to indicate the length of the MSISDN/MDN
        private string msisdnMdnLength;

        // this field is used to record the subscriber's directory number phone number.  In a ported environment, 
        // the MSISDN/MDN is the number that is kept with the subscriber
        private string msisdnMdn;

        // this field indicates which mobile station identifier is being used
        // 0 = not available, 1 = ESN or UIMID, 2 = IMEI, 3 = MEID, 4 = Psuedo ESN/MEID
        private string esnUimidImeiMeidIndicator;
        private string esnUimidImeiMeid;

        // this field is used to identify the anchor system/market and consequently the carrier that provided
        // services to a roaming mobile station
        private string servingCarrierSIDBID;

        // records the total charges and taxes associated with this CIBER record.  The total charges and taxes
        // is the sum of the all the charge fields in a CIBER record.
        private string totalChargesAndTaxes;
        private string systemReservedFiller1=" ";// blank zero_pad

        // records all the state/province taxes associated with a CIBER record
        private string totalStateProvinceTaxes;
        private string systemReservedFiller2 = " ";// blank zero_pad

        // records all local taxes associated with a CIBER record.
        private string totalLocalTaxes;
        private string systemReservedFiller3 = " "; // blank zero_pad

        // the date that the call originated
        private string callDate;

        // indicates the direction of the mobile call 
        // accepted values are defined in section 8, table 4
        private string callDirection;

        // indicates completion status for the call
        // 1 = incomplete, 2 = called party answered, 3 = call completed but midnight passed before called 
        // party answered
        private string callCompletionIndicator;
        private string callTerminationIndicator;
        private string callerIdLength;
        private string callerId;
        private string calledNumberLength;
        private string calledNumberDigits;
        private string locationRoutingNumberLengthIndicator;
        private string locationRoutingNumber;

        // fields used to record the TLDN assigned to a mobile unit by the serving carrier while the mobile
        // station is roaming
        private string tldnLength;
        private string tldn;
        private string currencyType;
        private string systemReservedFiller4 ="  ";// 2 blank zero_pad;
        private string originalBatchSequenceNumber;

        // this field is used to capture the ceell site number or CLLI code where a roaming mobile call
        // was initiated or received.  Cell site numbers should be unique for each switch.
        // justification : right, zero_padded : blanks.
        private string initialCellSite;
        private string timeZoneIndicator;

        // used to indicate whether or not daylight savings is in effect or not
        // at the serving network
        private string daylightSavingIndicator;
        private string messageAccountingDigits;

        // records the time that the mbile unit successsfully connected to a wireless system
        // in the event of "no Air", a time is still populated to represent the time of the event
        // being recorded by the record
        // HHMMSS: HH = 00-23, MM = 00-59, SS = 00-59
        private string airConnectTime;

        // the billable elapsed time used to calculate air time charges
        private string airChargeableTime;

        // the elapsed time associated with a call
        //This field must contain a value greater than zero, and within the range of accepted values as specified above, 
        // except for the following condition:
        //•Special Features Used contains a “F”
        //•Call Completion Indicator = 1
        //•Special Features Used contains an “X”
        // If any of the above conditions are true, then this field may be zero filled.
        private string airElapsedTime;

        // rate period in affect for the respective Air Connect Time
        // must contain a value greater than zero and with a value listed in Section 8, Table 11.
        private string airRatePeriod;
        
        // to indicate whether the air time associated with a call was contained in one rate period or 
        // spanned multiple rate periods.  0 = N/A, 1 = single rate period, 2 = multi-rate period
        private string airMultiRatePeriod;

        // this field is used to record the charges associated with the air time portion of a call.
        // it must not contain any taxes.
        // if airChargeable time equals zero, this must be zero filled.
        private string airCharge;
        private string systemReservedFiller5 = " ";//blank zero_pad
        private string otherChargeNumberOneIndicator;
        private string otherChargeNumberOne;
        private string systemReservedFiller6 = " ";//bland fill
        private string systemReservedFiller7;
        private string printedCall;

        // field indicates whether a record contains usage information that may be considered fraudulent.  
        // 00 - no fraud involvement
        // 01-33 - server carrier assigned
        // 24-66 - home carrier assigned
        // 67-99 - selective use
        private string fraudIndicator;

        // 0 = no fraud, 1 = report only, 2 = other
        // indicates whether or not possible fraudulent records should be treated as reports only or should be
        // considered for other applications.
        private string fraudSubIndicator;
        private string specialFeaturesUsed;

        //This field contains the service call descriptor from Section 8, Table 18, or the geographic name of the Called Place. 
        // For international calls outside the NANP area, the Called Place can contain the name of the called country.
        // For MO, calledplace is the place that the mobile subscriber is calling.
        // For MT, called place is the place where the mobile subscriber answered the call.
        private string calledPlace;
        private string calledStateProvince;
        private string calledCountry;
        private string servingPlace;
        private string servingStateProvince;
        private string servingCountry;

        // This field is used to record the time that a mobile unit established toll 
        // resource/network connection to originate or terminate a call.
        //HHMMSS: HH = 00-23, MM = 00-59, SS = 00-59
        private string tollConnectTime;

        // this field records the billable elapsed time used to calculate the toll charge
        // MMMMSS: MMMM = 0000-9999, SS = 00-59
        private string tollChargeableTime;

        // records the elapsed time associated with the toll portion of a call
        // MMMMSS: MMMM = 0000-9999, SS = 00-59
        private string tollElapsedTime;

        // indentifies the tariff that was used to rate the toll charge
        // accepted values are defined in section 8, table 7.
        private string tollTariffDescriptor;

        // indicates the rate period in affect for the respective toll connect time
        // defined by section 8, table 8
        private string tollRatePeriod;

        // this field indicates whether the toll portion of a call was contained in one rate period
        // or spanning multiple rate periods.
        // 0 = N/A, 1 = single rate period, 2 = multi-rate period
        private string tollMultiRatePeriod;

        // this field is used to identify the element of a tariff used for rating the message based
        // on if the message was customer dialed or operator handled
        // accepted values : Section 8 , Table 9
        private string tollRateClass;

        // this field is used to indicate the length of the toll rating point, 00-10
        private string tollRatingPointLengthIndicator;

        // determine the origin to the wired network for toll rating purposes
        // if the toll rating point length indicator equals "00", then the field must be zero filled
        private string tollRatingPoint;

        // records the charges associated with the toll portion of a call.  
        private string tollCharge;
        private string systemReservedFiller8=" ";

        // this field is used to record the state/province taxes associated with the respective toll charge
        // toll local taxes must not be included in this field. 
        private string tollStateProvinceTaxes;
        private string systemReservedFiller9=" ";

        // records the local taxes associated with the toll charge
        // toll state/province taxes must not be included in this field
        // justification : right , zero_padding : zero
        // accepted values :  0000000000 - 9999999999
        private string tollLocalTaxes;
        private string systemReservedFiller10 = " ";

        // this field is used to indicate the landline carrier that was used to complete the call.  
        // these are defined in section 8, table 10 and appendix H
        private string tollNetworkCarrierId;
        private string localCarrierReserved;
        private string systemReservedFiller11;

        public Record22()
        {

            // everything needs to be initialized to values 
            ReturnCode = "0";
            CiberRecordReturnReasonCode = "00";
            systemReservedFiller7 = String.Empty.PadRight(13, this.blank_pad);
            this.localCarrierReserved = String.Empty.PadRight(75, this.blank_pad);
            systemReservedFiller11 = String.Empty.PadRight(75, this.blank_pad);

        }

        /// <summary>
        /// public constructor that takes a string input that represents the record type 01
        /// and parses the fields into the record01 type object
        /// </summary>
        /// <param name="line"></param>
        public Record22(string line)
        {
            returnCode = line.Substring(2, 1);
            this.ciberRecordReturnReasonCode = line.Substring(3, 2);
            this.invalidFieldIdentifier = line.Substring(5, 3);
            this.homeCarrierSIDBID = line.Substring(8, 5);
            this.msidIndicator = line.Substring(13, 1);
            this.msid = line.Substring(14, 15);
            this.msisdnMdnLength = line.Substring(29, 2);
            this.msisdnMdn = line.Substring(31, 15);
            this.esnUimidImeiMeidIndicator = line.Substring(46, 1);
            this.esnUimidImeiMeid = line.Substring(47, 19);
            this.servingCarrierSIDBID = line.Substring(66, 5);
            this.totalChargesAndTaxes = line.Substring(71, 10);
            this.systemReservedFiller1 = line.Substring(81, 1);
            this.totalStateProvinceTaxes = line.Substring(82, 10);
            this.systemReservedFiller2 = line.Substring(92, 1);
            this.totalLocalTaxes = line.Substring(93, 10);
            this.systemReservedFiller3 = line.Substring(103, 1);
            this.callDate = line.Substring(104, 6);
            this.callDirection = line.Substring(110, 1);
            this.callCompletionIndicator = line.Substring(111, 1);
            this.callTerminationIndicator = line.Substring(112, 1);
            this.callerIdLength = line.Substring(113, 2);
            this.callerId = line.Substring(115, 15);
            this.calledNumberLength = line.Substring(130, 2);
            this.calledNumberDigits = line.Substring(132, 15);
            this.locationRoutingNumberLengthIndicator = line.Substring(147, 2);
            this.locationRoutingNumber = line.Substring(149, 15);
            this.tldnLength = line.Substring(164, 2);
            this.tldn = line.Substring(166, 15);
            this.currencyType = line.Substring(181, 2);
            this.systemReservedFiller4 = line.Substring(183, 2);
            this.originalBatchSequenceNumber = line.Substring(185, 3);
            this.initialCellSite = line.Substring(188, 11);
            this.timeZoneIndicator = line.Substring(199, 2);
            this.daylightSavingIndicator = line.Substring(201, 1);
            this.messageAccountingDigits = line.Substring(202, 10);
            this.airConnectTime = line.Substring(212, 6);
            this.airChargeableTime = line.Substring(218, 6);
            this.airElapsedTime = line.Substring(224, 6);
            this.airRatePeriod = line.Substring(230, 2);
            this.airMultiRatePeriod = line.Substring(232, 1);
            this.airCharge = line.Substring(233, 10);
            this.systemReservedFiller5 = line.Substring(243, 1);
            this.otherChargeNumberOneIndicator = line.Substring(244, 2);
            this.otherChargeNumberOne = line.Substring(246, 10);
            this.systemReservedFiller6 = line.Substring(256, 1);
            this.systemReservedFiller7 = line.Substring(257, 13);
            this.printedCall = line.Substring(270, 15);
            this.fraudIndicator = line.Substring(285, 2);
            this.fraudSubIndicator = line.Substring(287, 1);
            this.specialFeaturesUsed = line.Substring(288, 5);
            this.calledPlace = line.Substring(293, 10);
            this.calledStateProvince = line.Substring(303, 2);
            this.calledCountry = line.Substring(305, 3);
            this.servingPlace = line.Substring(308, 10);
            this.servingStateProvince = line.Substring(318, 2);
            this.servingCountry = line.Substring(320, 3);
            this.tollConnectTime = line.Substring(323, 6);
            this.tollChargeableTime = line.Substring(329, 6);
            this.tollElapsedTime = line.Substring(335, 6);
            this.tollTariffDescriptor = line.Substring(341, 2);
            this.tollRatePeriod = line.Substring(343, 2);
            this.tollMultiRatePeriod = line.Substring(345, 1);
            this.tollRateClass = line.Substring(346, 1);
            this.tollRatingPointLengthIndicator = line.Substring(347, 2);
            this.tollRatingPoint = line.Substring(349, 10);
            this.tollCharge = line.Substring(359, 10);
            this.systemReservedFiller8 = line.Substring(369, 1);
            this.tollStateProvinceTaxes = line.Substring(370, 10);
            this.systemReservedFiller9 = line.Substring(380, 1);
            this.tollLocalTaxes = line.Substring(381, 10);
            this.systemReservedFiller10 = line.Substring(391, 1);
            this.tollNetworkCarrierId = line.Substring(392, 5);
            this.localCarrierReserved = line.Substring(397, 75);
            this.systemReservedFiller11 = line.Substring(472, 75);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("RecordType:" + this.recordType + this.m_newline);
            sb.Append("ReturnCode:" + this.returnCode + this.m_newline);
            sb.Append("CIBERRecordReturnReasonCode:" + this.ciberRecordReturnReasonCode + this.m_newline);
            sb.Append("InvalidFieldIdentifier:" + this.invalidFieldIdentifier + this.m_newline);
            sb.Append("HomeCarrierSIDBID:" + this.homeCarrierSIDBID + this.m_newline);
            sb.Append("MSIDIndicator:" + this.msidIndicator + this.m_newline);
            sb.Append("MSID:" + this.msid + this.m_newline);
            sb.Append("MSISDN/MDN Length:" + this.msisdnMdnLength + this.m_newline);
            sb.Append("MSISDN/MDN:" + this.msisdnMdn + this.m_newline);
            sb.Append("ESN/UIMID/IMEI/MEID Indicator:" + this.esnUimidImeiMeidIndicator + this.m_newline);
            sb.Append("ESN/UIMID/IMEI/MEID:" + this.esnUimidImeiMeid + this.m_newline);
            sb.Append("ServingCarrierSID/BID:" + this.servingCarrierSIDBID + this.m_newline);
            sb.Append("TotalChargesAndTaxes:" + this.totalChargesAndTaxes + this.m_newline);
            sb.Append("SystemReservedFiller1:" + this.systemReservedFiller1 + this.m_newline);
            sb.Append("TotalStateProvinceTaxes:" + this.totalStateProvinceTaxes + this.m_newline);
            sb.Append("SystemReservedFiller2:" + this.systemReservedFiller2 + this.m_newline);
            sb.Append("TotalLocalTaxes:" + this.totalLocalTaxes + this.m_newline);
            sb.Append("SystemReservedFiller:" + this.systemReservedFiller3 + this.m_newline);
            sb.Append("CallDate:" + this.callDate + this.m_newline);
            sb.Append("CallDirection:" + this.callDirection + this.m_newline);
            sb.Append("CallCompletionIndicator:" + this.callCompletionIndicator + this.m_newline);
            sb.Append("CallTerminationIndicator:" + this.callTerminationIndicator + this.m_newline);
            sb.Append("CallerIdLength:" + this.callerIdLength + this.m_newline);
            sb.Append("CallerId:" + this.callerId + this.m_newline);
            sb.Append("CalledNumberLength:" + this.calledNumberLength + this.m_newline);
            sb.Append("CalledNumberDigits:" + this.calledNumberDigits + this.m_newline);
            sb.Append("LocationRoutingNumberLengthIndicator:" + this.locationRoutingNumberLengthIndicator + this.m_newline);
            sb.Append("LocationRoutingNumber:" + this.locationRoutingNumber + this.m_newline);
            sb.Append("TLDNLength:" + this.tldnLength + this.m_newline);
            sb.Append("TLDN:" + this.tldn + this.m_newline);
            sb.Append("CurrencyType:" + this.currencyType + this.m_newline);
            sb.Append("SystemReservedFiller:" + this.systemReservedFiller4 + this.m_newline);
            sb.Append("OriginalBatchSequenceNumber:" + this.originalBatchSequenceNumber + this.m_newline);
            sb.Append("InitialCellSite:" + this.initialCellSite + this.m_newline);
            sb.Append("TimeZoneIndicator:" + this.timeZoneIndicator + this.m_newline);
            sb.Append("DaylightSavingsIndicator:" + this.daylightSavingIndicator + this.m_newline);
            sb.Append("MessageAccountingDigits:" + this.messageAccountingDigits + this.m_newline);
            sb.Append("AirConnectTime:" + this.airConnectTime + this.m_newline);
            sb.Append("AirChargeableTime:" + this.airChargeableTime + this.m_newline);
            sb.Append("AirElapsedTime:" + this.airElapsedTime + this.m_newline);
            sb.Append("AirRatePeriod:" + this.airRatePeriod + this.m_newline);
            sb.Append("AirMultiRatePeriod:" + this.airMultiRatePeriod + this.m_newline);
            sb.Append("AirCharge:" + this.airCharge + this.m_newline);
            sb.Append("SystemReservedFiller5:" + this.systemReservedFiller5 + this.m_newline);
            sb.Append("OtherChargeNumberOneIndicator:" + this.otherChargeNumberOneIndicator + this.m_newline);
            sb.Append("OtherChargeNumberOne:" + this.otherChargeNumberOne + this.m_newline);
            sb.Append("SystemReservedFiller6:" + this.systemReservedFiller6 + this.m_newline);
            sb.Append("SystemReservedFiller7:" + this.systemReservedFiller7 + this.m_newline);
            sb.Append("PrintedCall:" + this.printedCall + this.m_newline);
            sb.Append("FraudIndicator:" + this.fraudIndicator + this.m_newline);
            sb.Append("FraudSubIndicator:" + this.fraudSubIndicator + this.m_newline); 
            sb.Append("SpecialFeaturesUsed:" + this.specialFeaturesUsed + this.m_newline);
            sb.Append("CalledPlace:" + this.calledPlace + this.m_newline);
            sb.Append("CalledStateProvince:" + this.calledStateProvince + this.m_newline);
            sb.Append("CalledCountry:" + this.calledCountry + this.m_newline);
            sb.Append("ServingPlace:" + this.servingPlace + this.m_newline);
            sb.Append("ServingStateProvince:" + this.servingStateProvince + this.m_newline);
            sb.Append("ServingCountry:" + this.servingCountry + this.m_newline);
            sb.Append("TollConnectTime:" + this.tollConnectTime + this.m_newline);
            sb.Append("TollChargeableTime:" + this.tollChargeableTime + this.m_newline);
            sb.Append("TollElapsedTime:" + this.tollElapsedTime + this.m_newline);
            sb.Append("TollTariffDescriptor:" + this.tollTariffDescriptor + this.m_newline);
            sb.Append("TollRatePeriod:" + this.tollRatePeriod + this.m_newline);
            sb.Append("TollMultiRatePeriod:" + this.tollMultiRatePeriod + this.m_newline);
            sb.Append("TollRateClass:" + this.tollRateClass + this.m_newline);
            sb.Append("TollRatingPointLengthIndicator:" + this.tollRatingPointLengthIndicator + this.m_newline);
            sb.Append("TollRatingPoint:" + this.tollRatingPoint + this.m_newline);
            sb.Append("TollCharge:" + this.tollCharge + this.m_newline);
            sb.Append("SystemReservedFiller8:" + this.systemReservedFiller8 + this.m_newline);
            sb.Append("TollStateProvinceTaxes:" + this.tollStateProvinceTaxes + this.m_newline);
            sb.Append("SystemReservedFiller9:" + this.systemReservedFiller9 + this.m_newline);
            sb.Append("TollLocalTaxes:" + this.tollLocalTaxes + this.m_newline);
            sb.Append("SystemReservedFiller10:" + this.systemReservedFiller10 + this.m_newline);
            sb.Append("TollNetworkCarrierId:" + this.tollNetworkCarrierId + this.m_newline);
            sb.Append("LocalCarrierReserved:" + this.localCarrierReserved + this.m_newline);
            sb.Append("SystemReservedFiller11:" + this.systemReservedFiller11 + this.m_newline);

            return base.ToString() + sb.ToString();
        
        }//toString()

        /// <summary>
        /// method to convert the object to a string that can be included in the CIBER file
        /// that will be sent in the future
        /// </summary>
        /// <returns></returns>
        public string ToCiberStringFormat()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(this.recordType);
            sb.Append(this.returnCode);
            sb.Append(this.ciberRecordReturnReasonCode);
            sb.Append(this.invalidFieldIdentifier);
            sb.Append(this.homeCarrierSIDBID);
            sb.Append(this.msidIndicator);
            sb.Append(this.msid);
            sb.Append(this.msisdnMdnLength);
            sb.Append(this.msisdnMdn);
            sb.Append(this.esnUimidImeiMeidIndicator);
            sb.Append(this.esnUimidImeiMeid);
            sb.Append(this.servingCarrierSIDBID);
            sb.Append(this.totalChargesAndTaxes);
            sb.Append(this.systemReservedFiller1);
            sb.Append(this.totalStateProvinceTaxes);
            sb.Append(this.systemReservedFiller2);
            sb.Append(this.totalLocalTaxes);
            sb.Append(this.systemReservedFiller3);
            sb.Append(this.callDate);
            sb.Append(this.callDirection);
            sb.Append(this.callCompletionIndicator);
            sb.Append(this.callTerminationIndicator);
            sb.Append(this.callerIdLength);
            sb.Append(this.callerId);
            sb.Append(this.calledNumberLength);
            sb.Append(this.calledNumberDigits);
            sb.Append(this.locationRoutingNumberLengthIndicator);
            sb.Append(this.locationRoutingNumber );
            sb.Append(this.tldnLength);
            sb.Append(this.tldn );
            sb.Append(this.currencyType);
            sb.Append( this.systemReservedFiller4 );
            sb.Append( this.originalBatchSequenceNumber );
            sb.Append( this.initialCellSite );
            sb.Append(this.timeZoneIndicator );
            sb.Append( this.daylightSavingIndicator );
            sb.Append( this.messageAccountingDigits );
            sb.Append(this.airConnectTime );
            sb.Append(this.airChargeableTime );
            sb.Append(this.airElapsedTime );
            sb.Append( this.airRatePeriod );
            sb.Append( this.airMultiRatePeriod );
            sb.Append(this.airCharge );
            sb.Append( this.systemReservedFiller5 );
            sb.Append( this.otherChargeNumberOneIndicator );
            sb.Append( this.otherChargeNumberOne );
            sb.Append( this.systemReservedFiller6 );
            sb.Append(this.systemReservedFiller7); 
            sb.Append(this.printedCall);
            sb.Append(this.fraudIndicator);
            sb.Append( this.fraudSubIndicator );
            sb.Append( this.specialFeaturesUsed );
            sb.Append(this.calledPlace );
            sb.Append( this.calledStateProvince );
            sb.Append(this.calledCountry );
            sb.Append( this.servingPlace );
            sb.Append( this.servingStateProvince );
            sb.Append(this.servingCountry );
            sb.Append(this.tollConnectTime );
            sb.Append(this.tollChargeableTime );
            sb.Append(this.tollElapsedTime );
            sb.Append( this.tollTariffDescriptor );
            sb.Append( this.tollRatePeriod );
            sb.Append( this.tollMultiRatePeriod );
            sb.Append( this.tollRateClass );
            sb.Append( this.tollRatingPointLengthIndicator );
            sb.Append( this.tollRatingPoint );
            sb.Append( this.tollCharge );
            sb.Append( this.systemReservedFiller8 );
            sb.Append(this.tollStateProvinceTaxes );
            sb.Append( this.systemReservedFiller9 );
            sb.Append( this.tollLocalTaxes );
            sb.Append(this.systemReservedFiller10 );
            sb.Append( this.tollNetworkCarrierId );
            sb.Append(this.localCarrierReserved );
            sb.Append(this.systemReservedFiller11 );

            return sb.ToString();

        }
        //accessors
        /// <summary>
        /// Public accessor for batchCreationDate
        /// 
        /// </summary>
        public string HomeCarrierSidBid
        {
            get
            {
                return homeCarrierSIDBID;
            }
            set
            {
                if (value.Length == 5)
                    homeCarrierSIDBID = value;
                else if (value.Length <= 5)
                    homeCarrierSIDBID = value.PadLeft(5, zero_pad);
                else if (value.Length > 5)
                    throw new ApplicationException("InvalidValueForParameter:HomeCarrierSidBid");
            }
        }
        public string ReturnCode
        {
            get
            {
                return returnCode;
            }
            set
            {
                // add intelligence to format the data
                if ( value.Length == 1 )
                    returnCode = value;
                                
                else
                    throw new CiberRecordException("InvalidFieldLengthForParameterReturnCode");
            }
        }
        public string CiberRecordReturnReasonCode
        {
            get
            {
                return this.ciberRecordReturnReasonCode;
            }
            set
            {
                    ciberRecordReturnReasonCode = value.PadLeft(2, zero_pad);
            }
        }
        public string InvalidFieldIndentifier
        {
            get
            {
                return invalidFieldIdentifier;
            }
            set
            {
                // length = 3
                invalidFieldIdentifier = value.PadLeft(3, zero_pad);
            }
        }
        public string MsidIndicator
        {
            get
            {
                return this.msidIndicator;
            }
            set
            {
                    if ( value.Length == 1 )
                        msidIndicator = value;
                    else
                        throw new CiberRecordException("InvalidFieldLengthForParameterMsidIndicator");
            }
        }
        public string Msid
        {
            get
            {
                return this.msid;
            }
            set
            {
                // force this to be our verizon test phone
                // value = "8153490866"; //MIN  SID/BID for this is 133
                
                // add intelligence to format the data
                // the format : positions 9-13, required
                // for now just check the length, not the format.
                if (value.Length == 15)
                    msid = value;
                else
                {
                    this.msid = value.PadRight(15, zero_pad);
                }
            }
        }
        public string MsisdnMdnLength
        {
            get
            {
                return this.msisdnMdnLength;
            }
            set
            {
                    msisdnMdnLength = value.PadLeft(2, zero_pad);
            }
        }
        public string MsisdnMdn
        {
            get
            {
                return this.msisdnMdn;
            }
            set
            {
                // force this to be our verizon test phone
                // value = "6145724180"; //MDN


                if (value.Length == 15)
                    msisdnMdn = value;
                else
                {
                    this.msisdnMdn = value.PadRight(15, zero_pad);
                }


            }
        }
        public string EsnUimidImeiMeid
        {
            get
            {
                return this.esnUimidImeiMeid;
            }
            set
            {
                // force this to be our verizon test phone
                // value = "02811904986"; //ESN
                if (value.Length == 19)
                    esnUimidImeiMeid = value;
                else
                {
                    this.esnUimidImeiMeid = value.PadRight(19, zero_pad);
                }
            }
        }
        public string EsnUimidImeiMeidIndicator
        {
            get
            {
                return this.esnUimidImeiMeidIndicator;
            }
            set
            {
                // valid values are 0=not available, 1=ESN, 2=IMEI, 3=MEID, 4=Pseudo ESN/MEID
                if (value.Length == 1)
                    esnUimidImeiMeidIndicator = value;
                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:EsnUimidImeiMeidIndicator");

            }
        }
        public string ServingCarrierSidBid
        {
            get
            {
                return this.servingCarrierSIDBID;
            }
            set
            {
                if (value.Length == 5)
                    servingCarrierSIDBID = value;
                else if (value.Length <= 5)
                    servingCarrierSIDBID = value.PadLeft(5, zero_pad);
                else if (value.Length > 5)
                    throw new ApplicationException("InvalidValueForParameter:ServingCarrierSidBid");
            }
        }

        public string TotalChargesAndTaxes
        {
            get
            {
                return this.totalChargesAndTaxes;
            }
            set
            {
                totalChargesAndTaxes = value.PadLeft(10, zero_pad);

            }
        }

        public string TotalStateProvinceTaxes
        {
            get
            {
                return this.totalStateProvinceTaxes;
            }
            set
            {
                totalStateProvinceTaxes = value.PadLeft(10, zero_pad);

            }
        }
        public string TotalLocalTaxes
        {
            get
            {
                return this.totalLocalTaxes;
            }
            set
            {
                totalLocalTaxes = value.PadLeft(10, zero_pad);

            }
        }
        public string CallDate
        {
            get
            {
                return this.callDate;
            }
            set
            {
                // verify format is YYMMDD : FORMAT length 6N
                if (value.Length == 6)
                    callDate = value;
                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:CallDate");
            }
        }
        public string CallDirection
        {
            get
            {
                return this.callDirection;
            }
            set
            {
                // one of the following 1-9
                // 
                if ( value.Length == 1 )
                    callDirection = value;
                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:CallDirection");
            }
        }

        /// <summary>
        /// if callcompletion = 1 and the airelapsed time > 5 mins, then the record must be rejected
        /// </summary>
        public string CallCompletionIndicator
        {
            get
            {
                return this.callCompletionIndicator;
            }
            set
            {
                // one of the following 1-3
                // 
                if ( value.Length == 1 )

                    callCompletionIndicator = value;
                    
                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:CallCompletionIndicator");
            }
        }

        public string CallTerminationIndicator
        {
            get
            {
                return this.callTerminationIndicator;
            }
            set
            {
                if ( value.Length == 1 )
                    callTerminationIndicator = value;
                
                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:CallTerminationIndicator");
            }
        }
        public string CallerIdLength
        {
            get
            {
                return this.callerIdLength;
            }
            set
            { // length = 2
                callerIdLength = value.PadLeft(2, zero_pad);
            }
        }

        // MO calls, caller id = MDN
        // MT calls, caller id = calling party
        public string CallerId
        {
            get
            {
                return this.callerId;
            }
            set
            {
                // this will take care of the case where the length is zero, then the field is zero filled
                callerId = value.PadRight(15, zero_pad);
            }
        }
        public string CalledNumberLength
        {
            get
            {
                return this.calledNumberLength;
            }
            set
            { // length = 2
                calledNumberLength = value.PadLeft(2, zero_pad);
            }
        }
        public string CalledNumberDigits
        {
            get
            {
                return this.calledNumberDigits;
            }
            set
            {
                calledNumberDigits = value.PadRight(15, zero_pad);
            }
        }
        public string LocationRoutingNumberLengthIndicator
        {
            get
            {
                return this.locationRoutingNumberLengthIndicator;
            }
            set
            {
                locationRoutingNumberLengthIndicator = value.PadLeft(2, zero_pad);
            }
        }
        public string LocationRoutingNumber
        {
            get
            {
                return this.locationRoutingNumber;
            }
            set
            {  // This field is used to record the routing number associated with a ported or pooled MDN.
                locationRoutingNumber = value.PadRight(15, zero_pad);
            }
        }

        public string TldnLength
        {
            get
            {
                return this.tldnLength;
            }
            set
            { // length = 2
                tldnLength = value.PadLeft(2, zero_pad);
            }
        }
        public string Tldn
        {
            get
            {
                return this.tldn;
            }
            set
            {
                tldn = value.PadRight(15, zero_pad);
            }
        }

        public string CurrencyType
        {
            get
            {
                return this.currencyType;
            }
            set
            {
                // 01 = usdollar, 02 = canadian dollar
                currencyType = value.PadLeft(2, zero_pad);
            }
        }

        public string OriginalBatchSequenceNumber
        {
            get
            {
                return this.originalBatchSequenceNumber;
            }
            set
            {
                if (value.Length == 3)
                    originalBatchSequenceNumber = value;
                else if (value.Length <= 3)
                    originalBatchSequenceNumber = value.PadLeft(3, zero_pad);
                else if (value.Length > 3)
                    throw new ApplicationException("InvalidValueForParameter:OriginalBatchSequenceNumber");
            }
        }
        public string InitialCellSite
        {
            get
            {
                return this.initialCellSite;
            }
            set
            {
                // 01 = usdollar, 02 = canadian dollar
                initialCellSite = value.PadLeft(11, this.blank_pad);
            }
        }
        public string TimeZoneIndicator
        {
            get
            {
                return this.timeZoneIndicator;
            }
            set
            {
                // 01 = usdollar, 02 = canadian dollar
                timeZoneIndicator = value.PadLeft(2, this.zero_pad);
            }
        }

        public string DaylightSavingIndicator
        {
            get
            {
                return this.daylightSavingIndicator;
            }
            set
            {
                if ( value.Length == 1 )
                // 01 = usdollar, 02 = canadian dollar
                    daylightSavingIndicator = value;

                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:DaylightSavingIndicator");
            }
        }

        public string MessageAccountingDigits
        {
            get
            {
                return this.messageAccountingDigits;
            }
            set
            {
                messageAccountingDigits = value.PadRight(10, zero_pad);
            }
        }

        public string AirConnectTime
        {
            get
            {
                return this.airConnectTime;
            }
            set
            {
                if (value.Length == 6)
                    airConnectTime = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:AirConnectTime");
            }
        }
        public string AirChargeableTime
        {
            get
            {
                return this.airChargeableTime;
            }
            set
            {// MMMMSS: MM = 000-9999, SS = 00-59
                if( value.Length == 6 )
                    airChargeableTime = value;
                else if ( value.Length <= 6 )
                    airChargeableTime = value.PadLeft(6, zero_pad);
                else if ( value.Length > 6 )
                    throw new ApplicationException("InvalidValueForParameter:AirChargeableTime");
            }
        }

        public string AirElapsedTime
        {
            get
            {
                return this.airElapsedTime;
            }
            set
            {// MMMMSS: MM = 000-9999, SS = 00-59
                if (value.Length == 6)
                    airElapsedTime = value;
                else if (value.Length <= 6)
                    airElapsedTime = value.PadLeft(6, zero_pad);
                else if (value.Length > 6)
                    throw new ApplicationException("InvalidValueForParameter:AirElapsedTime");
            }
        }
        public string AirRatePeriod
        {
            get
            {
                return this.airRatePeriod;
            }
            set
            {// 00-13 see section 8, table 11 air rate period
                if (value.Length == 2)
                    airRatePeriod = value;
                else if (value.Length == 1)
                    airRatePeriod = value.PadLeft(2, zero_pad);
                else
                    throw new ApplicationException("InvalidValueForParameter:AirRatePeriod");
            }
        }
        public string AirMultiRatePeriod
        {
            get
            {
                return this.airMultiRatePeriod;
            }
            set
            { // valid values are 0, 1=single-rate, 2 = multi-rate period
                if (value.Length == 1)
                    airMultiRatePeriod = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:AirRateMultiPeriod");
            }
        }
        public string AirCharge
        {
            get
            {
                return this.airCharge;
            }
            set
            {
                //least significant two positions of this field represent the “cents” value of the charge. 
                // The most significant eight positions of this field represent the “dollar” value of the charge. 
                // For example, “$$$$$$$$¢¢.” A decimal point “.” is not used in the charge field.              
                airCharge = value.PadLeft(10, zero_pad);
            }
        }

        public string OtherChargeNumberOneIndicator
        {
            get
            {
                return this.otherChargeNumberOneIndicator;
            }
            set
            {
                //
                otherChargeNumberOneIndicator = value.PadLeft(2, this.zero_pad);
            }
        }

        public string OtherChargeNumberOne
        {
            get
            {
                return this.otherChargeNumberOne;
            }
            set
            {
                //
                otherChargeNumberOne = value.PadLeft(10, zero_pad);
            }
        }
        public string FraudSubIndicator
        {
            get
            {
                return this.fraudSubIndicator;
            }
            set
            {
                // see page 10-66, section 10 data dictionary
                if (fraudIndicator.Equals("00"))
                    fraudSubIndicator = "0";
                else
                    fraudSubIndicator = value;
            }
        }
        public string SpecialFeaturesUsed
        {
            get
            {
                return this.specialFeaturesUsed;
            }
            set
            {
                // if no special features are used, then the first position should contain a zero with the
                // four remaining positions filled with blanks, see page 10-106.
                if ( value.Equals("0") )
                    specialFeaturesUsed = value.PadRight(5, this.blank_pad);
                if ( value.Length > 5 )
                    specialFeaturesUsed = value.Substring(0, 5).PadRight(5);
                else
                    specialFeaturesUsed = value.PadRight(5, blank_pad);
            }
        }
        public string PrintedCall
        {
            get
            {
                return this.printedCall;
            }
            set
            {
                if (value.Length > 15)
                    printedCall = value.Substring(0, 15).PadRight(15, blank_pad);
                else
                    printedCall = value.PadRight(15, this.blank_pad);
            }
        }
        public string FraudIndicator
        {
            get
            {
                return this.fraudIndicator;
            }
            set
            {
                fraudIndicator = value.PadLeft(2,zero_pad);
            }
        }
        public string CalledPlace
        {
            get
            {
                return this.calledPlace;
            }
            set
            {
                if (value.Length > 10)
                    calledPlace = value.Substring(0, 10).PadRight(10);
                else
                    calledPlace = value.PadRight(10, this.blank_pad);
            }
        }

        public string CalledStateProvince
        {
            get
            {
                return this.calledStateProvince;
            }
            set
            {
                if (value.Length == 2)
                    calledStateProvince = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:CalledState");

            }
        }
        public string CalledCountry
        {
            get
            {
                return this.calledCountry;
            }
            set
            {
                // see section 8, table 32 for valid country codes
                if (value.Length == 3)
                    calledCountry = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:CalledCountry");

            }
        }
        public string ServingPlace
        {
            get
            {
                return this.servingPlace;
            }
            set
            {
                if ( value.Length > 10 )
                    throw new ApplicationException("InvalidValueForParameter:ServingPlace");
                else
                    // for now we use it and zero_pad it if it is less than 10
                    servingPlace = value.PadRight(10, this.blank_pad);

            }
        }
        public string ServingStateProvince
        {
            get
            {
                return this.servingStateProvince;
            }
            set
            {
                if (value.Length == 2)
                    servingStateProvince = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:ServingStateProvince");

            }
        }
        public string ServingCountry
        {
            get
            {
                return this.servingCountry;
            }
            set
            {
                // see section 8, table 32 for valid country codes
                if (value.Length == 3)
                    servingCountry = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:ServingCountry");

            }
        }

        public string TollConnectTime
        {
            get
            {
                return this.tollConnectTime;
            }
            set
            {
                // HHMMSS: HH=00-23, MM=00-59, SS=00-59
                if (value.Length == 6)
                    tollConnectTime = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:TollConnectTime");
            }
        }

        public string TollChargeableTime
        {
            get
            {
                return this.tollChargeableTime;
            }
            set
            {
                // see restrictions on values on page 10-114 from spec
                // MMMM: MMMM=00-9999, SS=00-59
                if (this.specialFeaturesUsed.Contains("J") || this.specialFeaturesUsed.Contains("K") ||
                    this.specialFeaturesUsed.Contains("X"))
                    tollChargeableTime = m_one.PadLeft(6, this.zero_pad);
                else if (value.Length == 6)
                    tollChargeableTime = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:TollChargeableTime");
            }
        }
        public string TollElapsedTime
        {
            get
            {
                return this.tollElapsedTime;
            }
            set
            {
                // see restrictions on values on page 10-114 from spec
                // MMMM: MMMM=00-9999, SS=00-59
                // value = 00 only when there is no toll associated with this call
                if (this.specialFeaturesUsed.Contains("J") || this.specialFeaturesUsed.Contains("K") ||
                    this.specialFeaturesUsed.Contains("X"))
                    tollElapsedTime = "000001";
                else if (value.Length == 6)
                    tollElapsedTime = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:TollElapsedTime");
            }
        }
        public string TollTariffDescriptor
        {
            get
            {
                return this.tollTariffDescriptor;
            }
            set
            {
                // see section 8 , table 07 for a full description of values
                // 01 = international
                // 02 = interstate InterLATA, 
                // 03 = Intrastate InterLATA, 
                // 04 = Intrastate InterLATA
                // for international calls the toll tariff descriptor must = 01, 15, 17, 18.
                // if the toll tariff descriptor indicates an international call, then the called country and the 
                // serving country can not contain the same values
                if (value.Length == 2)
                    tollTariffDescriptor = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:TollElapsedTime");             
            }
        }


        public string TollMultiRatePeriod
        {
            get
            {
                return this.tollMultiRatePeriod;
            }
            set
            {
                if (value.Length == 1)
                    tollMultiRatePeriod = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:TollMultiRatePeriod");
            }
        }



        /// <summary>
        /// This field is used to indicate the element of a tariff used for rating the message based on if
        /// the message was customer dialed or operator handled
        /// The value of "0" can only be used when there is no toll associated with the call.
        /// </summary>
        public string TollRateClass
        {
            get
            {
                return this.tollRateClass;
            }
            set
            {
                if (value.Length == 1)
                    tollRateClass = value;
                                
                else
                    throw new ApplicationException("InvalidValueForParameter:TollMultiRatePeriod");
            }
        }
        public string TollRatePeriod
        {
            get
            {
                return this.tollRatePeriod;
            }
            set
            {
                // valid values are 00-09                   
                if (this.specialFeaturesUsed.Contains("J") || this.specialFeaturesUsed.Contains("K") ||
                    this.specialFeaturesUsed.Contains("X") )
                    tollRatePeriod = "00";
                else if ( value.Length == 1 )
                    tollRatePeriod = value.PadLeft(2, zero_pad);
                else if (value.Length == 2 )
                    tollRatePeriod = value;
                else
                    throw new ApplicationException("InvalidValueForParameter:TollRatePeriod");
            }
        }
        public string TollRatingPointLengthIndicator
        {
            get
            {
                return this.tollRatingPointLengthIndicator;
            }
            set
            {
                // valid values are 00-10
                tollRatingPointLengthIndicator = value.PadLeft(2, zero_pad);
            }
        }
        public string TollRatingPoint
        {
            get
            {
                return this.tollRatingPoint;
            }
            set
            {
                if (this.tollRatingPointLengthIndicator.Equals("00"))
                    tollRatingPoint = zero.PadRight(10, zero_pad);
                else
                    tollRatingPoint = value.PadRight(10, zero_pad);
            }
        }
        public string TollCharge
        {
            get
            {
                return this.tollCharge;
            }
            set
            {
                if (this.specialFeaturesUsed.Contains("J") || this.specialFeaturesUsed.Contains("K") ||
                    this.specialFeaturesUsed.Contains("X"))
                    tollCharge = zero.PadLeft(10, this.zero_pad);
                else
                    tollCharge = value.PadLeft(10, zero_pad);
            }
        }


        public string TollStateProvinceTaxes
        {
            get
            {
                return this.tollStateProvinceTaxes;
            }
            set
            {
                tollStateProvinceTaxes = value.PadLeft(10, zero_pad);
            }
        }
        public string TollLocalTaxes
        {
            get
            {
                return this.tollLocalTaxes;
            }
            set
            {
                tollLocalTaxes = value.PadLeft(10, zero_pad);
            }
        }
        public string TollNetworkCarrierId
        {
            get
            {
                return this.tollNetworkCarrierId;
            }
            set
            {
                tollNetworkCarrierId = value.PadLeft(5,zero_pad);
            }
        }


    }//class

}//namespace
