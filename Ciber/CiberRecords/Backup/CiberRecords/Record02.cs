using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Cdr.Ciber
{

    /// <summary>
    /// this is the CIBER record type 01
    /// This record is the Batch Header Record
    /// currently there are no value checks on the fields even though there are constraints
    /// on what values the fields can take, this is to be implemented (TBI).
    /// The length of this record is : 200.
    /// </summary>
    public class Record02
    {

        /// example = "01 070521 029 33333 01151 02 1 01 031015 2 00 1 208040784     EX0433";
        /// RecordType =                    01
        /// CreationDate =                  070521
        /// BatchSequenceNumber =           029
        /// SendingCarrierSID/BID :         33333
        /// ReceivingCarrierSID/BID :       01151
        /// CIBER record release number :   02
        /// Original Return Indicator :     1
        /// Currency Type :                 01
        /// SettlePeriod :                  031015
        /// clearinghouseid :               2
        /// CIBER Batch Reject reason code: 00
        /// LocalCarrierReserved :          208040784     EX0433

        // required fields
        private string m_newline = "\r\n";

        // record type this is an 01 CIBER record
        private string recordType = "02";

        // YYMMDD YY=00-99, MM=01-12, DD = 01-31
        private string batchCreationDate;

        private string batchSeqNumber; // the batch sequence number for a batch of records


        /// <summary>
        /// this field is used to identify the market and consequently the carrier that 
        /// provided services to a roaming mobile station after handing off from 
        /// an anchor serving system 
        /// see section 12, table 12-4A, 12-4E
        /// it must contain a value listed in the tables
        /// </summary>
        private string sendingClearingHouseSidBid;

        /// <summary>
        /// used to identify the intended recipient of a batch of CIBER records
        /// see section 12, table 12-4A, 12-4E
        /// it must contain a value listed in the tables
        /// </summary>
        private string originalServingSendingCarrierSidBid;
        private string ciberRecordReleaseNumber = "25";

        /// <summary>
        /// indicates whether the records in the batch are returned records or
        /// records being forwarded for billing
        /// Values : 1, 4, 7 used only for Type 01 Header Records
        /// Values : 3, 5, 8 may be used for Type 01, 02 Header Records
        /// Values : 2, 6, and 9 used only for Type 02 Header Records
        /// Values : A-F see page 10-89 for additional info
        /// or Section 8 . Tables, page 8-22-1 Table 22 Original/Return Indicator
        /// 1 = valid initial issue
        /// ...
        /// </summary>
        private string originalReturnIndicator; 

        private string currencyType;

        /// <summary>
        /// field used to identify the date upon which the settlement period ends.
        /// YYMMDD: YY = 00-99, MM=01-12;DD=15
        /// The DD must always contain "15".  The default intercarrier settlement
        /// period is from the 16th of each month to the 15th of the following month.
        /// Special Conditions:  The field should be zero filled initially and only modified by a sending 
        /// carrier’s clearinghouse when a settlement period is determined.
        /// </summary>
        private string settlementPeriod;

        /// <summary>
        /// this field is used to identify which clearinghouse last handled the batch.
        /// For initial issues batches from the billing vendor, this must equal zero.
        /// Table 20 lists the valid values
        /// 0 - from a billing vendor or a carrier
        /// 1 - syniverse north 31102
        /// 2 - syniverse technologies  31103
        /// 3 - mach usa  26238
        /// 4 - verisign 26240
        /// </summary>
        private string clearinghouseId;

        /// <summary>
        /// field used to inform the sending entity why the batch is being rejected.  The 
        /// Batch Reject Reason Code should be "00" for all initial issue or non-rejected batches.
        /// this field is used in conjunction with the Original/Return Indicator field.
        /// </summary>
        private string batchRejectReasonCode;

        private string originalHomeReceivingCarrierSidBid;


        /// <summary>
        /// field used to identify the CIBER record types contained in the batch
        /// see page 10-8 in the Data Dictionary
        /// Accepted Values include:
        /// 0 = CIBER type 22, 32, and 52 records converted from TAP I
        /// 1 = CIBER type 22, 32, and 52 records
        /// 2 = CIBER type 42 records
        /// </summary>
        private string batchContents;

        // optional fields
        /// <summary>
        /// optional field set aside for local carrier use.  this field may or may not have 
        /// information that is of value to the home carrier.  the field is intended for the
        /// use of carriers who operate several wireless geographic service areas and who wish
        /// to pass certain billing information pertinent to their own billing system.
        /// This field should NOT be used by clearinghouses or billing vendors without the 
        /// express approval of the carrier.
        /// </summary>
        private string localCarrierReserved = String.Empty;

        /// <summary>
        /// these fields have been set aside for future expansion of the CIBER record.  This
        /// space should not be used by carriers, billing vendors or clearinghouses.
        /// accepted values : BLANKS
        /// this field should contain blanks.
        /// for type 01 records positions 57-200 (size 144) are blanks.
        /// </summary>
        private string systemReservedFiller = String.Empty;

        public Record02( )
        {
        }
        /// <summary>
        /// public constructor that takes a string input that represents the record type 01
        /// and parses the fields into the record01 type object
        /// </summary>
        /// <param name="line"></param>
        public Record02( string line )
        {
            batchCreationDate = line.Substring(2, 6);
            batchSeqNumber = line.Substring(8, 3);
            sendingClearingHouseSidBid = line.Substring(11, 5);
            this.originalServingSendingCarrierSidBid = line.Substring(16, 5);
            ciberRecordReleaseNumber = line.Substring(21, 2);
            originalReturnIndicator = line.Substring(23, 1);
            currencyType = line.Substring(24, 2);
            settlementPeriod = line.Substring(26, 6);
            clearinghouseId = line.Substring(32, 1);
            batchRejectReasonCode = line.Substring(33, 2);
            this.originalHomeReceivingCarrierSidBid = line.Substring(35, 5);
            batchContents = line.Substring(40, 1);
            if ( line.Length == 200 )
                systemReservedFiller = line.Substring(41, 159 );

        }// end ctor

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("RecordType:" + this.recordType + this.m_newline);          
            sb.Append("BatchCreationDate:" + this.batchCreationDate + this.m_newline);
            sb.Append("BatchSequenceNumber:" + this.batchSeqNumber + this.m_newline);
            sb.Append("SendingClearingHouseBID:" + this.sendingClearingHouseSidBid + this.m_newline);
            sb.Append("OriginalServingSendingCarrierSIDBID:" + this.originalServingSendingCarrierSidBid + this.m_newline);
            sb.Append("CiberRecordReleaseNumber:" + this.ciberRecordReleaseNumber + this.m_newline);
            sb.Append("OriginalReturnIndicator:" + this.originalReturnIndicator + this.m_newline);
            sb.Append("CurrencyType:" + this.currencyType + this.m_newline);
            sb.Append("SettlementPeriod:" + this.settlementPeriod + this.m_newline);
            sb.Append("ClearinghouseID:" + this.clearinghouseId + this.m_newline);
            sb.Append("CIBER Batch RejectReasonCode:" + this.batchRejectReasonCode + this.m_newline);
            sb.Append("OriginalHomeReceivingCarrierSidBid:" + this.originalHomeReceivingCarrierSidBid + this.m_newline);
            sb.Append("BatchContents:" + this.batchContents + this.m_newline);
            sb.Append("SystemReservedFiller:" + this.systemReservedFiller + this.m_newline);
            

            return base.ToString()+ m_newline + sb.ToString();
        }

        /// <summary>
        /// method to convert the object to a string that can be included in the CIBER file
        /// that will be sent in the future
        /// </summary>
        /// <returns></returns>
        public string ToCiberStringFormat()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(this.recordType);
            sb.Append(this.batchCreationDate);
            //sb.Append(this.batchSeqNumber);
            //sb.Append(this.sendingCarrierSidBid);
            //sb.Append(this.receivingCarrierSidBid);
            //sb.Append(this.ciberRecordReleaseNumber);
            //sb.Append(this.originalReturnIndicator);
            //sb.Append(this.currencyType);
            //sb.Append(this.settlementPeriod);
            //sb.Append(this.clearinghouseId);
            //sb.Append(this.batchRejectReasonCode);
            //sb.Append(this.batchContents);           
            //sb.Append(this.localCarrierReserved);
            sb.Append(this.systemReservedFiller);

            return sb.ToString();

        }

        // public accessor for our params
        // validation of correct format as well


        /// <summary>
        /// Public accessor for batchCreationDate
        /// 
        /// </summary>
        public string BatchCreationDate
        {
            get
            {
                return batchCreationDate;
            }
            set
            {
                // add intelligence to format the data
                // the format : YYMMDD
                // for now just check the length, not the format.
                if (value.Length == 6)
                    batchCreationDate = value;
            }
        }

        public string SettlementPeriod
        {
            get
            {
                return this.settlementPeriod;
            }
            set
            {
                // add intelligence to format the data
                // the format : YYMMDD
                // for now just check the length, not the format.
                if (value.Length == 6)
                    if (value.Substring(2, 2).Equals("15"))
                        settlementPeriod = value;
                    else
                        settlementPeriod = value.Substring(0, 2) + "15" + value.Substring(5, 2);

            }
        }

        public string SendingClearingHouseSidBid
        {
            get
            {
                return this.sendingClearingHouseSidBid;
            }
        }

        public string OriginalServingSendingCarrierSidBid
        {
            get
            {
                return this.originalServingSendingCarrierSidBid;
            }
        }
        public string CurrencyType
        {
            get
            {
                return this.currencyType;
            }
        }
        public string ClearingHouseId
        {
            get
            {
                return this.clearinghouseId;
            }
        }
        public string BatchSequenceNumber
        {
            get
            {
                return this.batchSeqNumber;
            }
        }
        public string CiberRecordReleaseNumber
        {
            get
            {
                return this.ciberRecordReleaseNumber;
            }
        }
        
    }
}
