using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Cdr.Rating
{

    /// <summary>
    /// this is the CIBER record type 98
    /// This record is the Batch Trailer Record
    /// currently there are no value checks on the fields even though there are constraints
    /// on what values the fields can take, this is to be implemented (TBI).
    /// The length of this record is : 200.
    /// </summary>
    public class Record98
    {
        // fields for processing
        private string m_newline = "\r\n";
        private char pad = Convert.ToChar("0");
        private char blank_pad = Convert.ToChar(" ");

        // record type this is an 98 CIBER record type
        private string recordType = "98";
        private string batchCreationDate;  // YYMMDD YY=00-99, MM=01-12, DD = 01-31
        private string batchSeqNumber; // the batch sequence number for a batch of records


        /// <summary>
        /// this field is used to identify the market and consequently the carrier that 
        /// provided services to a roaming mobile station after handing off from 
        /// an anchor serving system 
        /// see section 12, table 12-4A, 12-4E
        /// it must contain a value listed in the tables
        /// </summary>
        private string sendingCarrierSidBid;

        /// <summary>
        /// used to identify the intended recipient of a batch of CIBER records
        /// see section 12, table 12-4A, 12-4E
        /// it must contain a value listed in the tables
        /// </summary>
        private string receivingCarrierSidBid;
        private string totalNumberRecordsInBatch;
        private string batchTotalChargesAndTaxes;

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

        private string batchTotalChargesAndTaxesSign;
        private string originalTotalChargesAndTaxesSign;
        private string systemReservedFiller1="   ";// blank fill of 3
        private string originalTotalNumberOfRecords;
        private string originalTotalChargesAndTaxes;
        private string systemReservedFiller2;

        private string currencyType;

        // optional fields
        /// <summary>
        /// optional field set aside for local carrier use.  this field may or may not have 
        /// information that is of value to the home carrier.  the field is intended for the
        /// use of carriers who operate several wireless geographic service areas and who wish
        /// to pass certain billing information pertinent to their own billing system.
        /// This field should NOT be used by clearinghouses or billing vendors without the 
        /// express approval of the carrier.
        /// fields 76-95 length : 20
        /// </summary>
        private string localCarrierReserved = String.Empty;

        /// <summary>
        /// these fields have been set aside for future expansion of the CIBER record.  This
        /// space should not be used by carriers, billing vendors or clearinghouses.
        /// accepted values : BLANKS
        /// this field should contain blanks.
        /// for type 01 records positions 96-200 (size 105) are blanks.
        /// </summary>
        private string systemReservedFiller3 = String.Empty;

        public Record98()
        {
            systemReservedFiller2 = String.Empty.PadRight(8, this.blank_pad);

        }
        public Record98( Record01 r1 )
        {
            batchCreationDate = r1.BatchCreationDate;
            batchSeqNumber = r1.BatchSequenceNumber; 
            sendingCarrierSidBid = r1.SendingCarrierSIDBID;
            receivingCarrierSidBid = r1.ReceivingCarrierSIDBID;
            settlementPeriod = r1.SettlementPeriod;
            clearinghouseId = r1.ClearingHouseId;
            systemReservedFiller2 = String.Empty.PadRight(8, this.blank_pad);
        }

        /// <summary>
        /// public constructor that takes a string input that represents the record type 01
        /// and parses the fields into the record01 type object
        /// </summary>
        /// <param name="line"></param>
        public Record98(string line)
        {
            // fill this in first, we can overwrite
            systemReservedFiller2 = String.Empty.PadRight(8, this.blank_pad);

            batchCreationDate = line.Substring(2, 6);
            batchSeqNumber = line.Substring(8, 3);
            sendingCarrierSidBid = line.Substring(11, 5);
            receivingCarrierSidBid = line.Substring(16, 5);
            totalNumberRecordsInBatch = line.Substring(21, 4);
            batchTotalChargesAndTaxes = line.Substring(25, 12);
            settlementPeriod = line.Substring(37, 6);
            clearinghouseId = line.Substring(43, 1);
            batchTotalChargesAndTaxesSign = line.Substring(44, 1);
            originalTotalChargesAndTaxesSign = line.Substring(45, 1);
            systemReservedFiller1 = line.Substring(46, 3);
            originalTotalNumberOfRecords = line.Substring(49, 4);
            originalTotalChargesAndTaxes = line.Substring(53, 12);
            systemReservedFiller2 = line.Substring(65,8);
            currencyType = line.Substring(73, 2);

            if (line.Length > 76)
                localCarrierReserved = line.Substring(75, 20);
            if (line.Length == 200)
                systemReservedFiller3 = line.Substring(95, 105);

        }// end ctor

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("RecordType:" + this.recordType + this.m_newline);
            sb.Append("BatchCreationDate:" + this.batchCreationDate + this.m_newline);
            sb.Append("BatchSequenceNumber:" + this.batchSeqNumber + this.m_newline);
            sb.Append("SendingCarrierSIDBID:" + this.sendingCarrierSidBid + this.m_newline);
            sb.Append("ReceivingCarrierSIDBID:" + this.receivingCarrierSidBid + this.m_newline);
            sb.Append("TotalNumberOfRecordsInBatch:" + this.totalNumberRecordsInBatch + this.m_newline);
            sb.Append("BatchTotalChargesAndTaxes:" + this.batchTotalChargesAndTaxes + this.m_newline);
            sb.Append("SettlementPeriod:" + this.settlementPeriod + this.m_newline);
            sb.Append("ClearingHouseID:" + this.clearinghouseId + this.m_newline);
            sb.Append("BatchTotalChargesAndTaxesSign:" + this.batchTotalChargesAndTaxesSign + this.m_newline);
            sb.Append("SystemReservedFiller1:" + this.systemReservedFiller1 + this.m_newline);
            sb.Append("OriginalTotalNumberOfRecords:" + this.originalTotalNumberOfRecords + this.m_newline);
            sb.Append("OriginalTotalChargesAndTaxes:" + this.originalTotalChargesAndTaxes + this.m_newline);
            sb.Append("SystemReservedFiller2:" + this.systemReservedFiller2 + this.m_newline);
            sb.Append("CurrencyType:" + this.currencyType + this.m_newline);
            sb.Append("LocalCarrierReserved:" + this.localCarrierReserved + this.m_newline);
            sb.Append("SystemReservedFiller3:" + this.systemReservedFiller3 + this.m_newline);


            return base.ToString() + m_newline + sb.ToString();
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
            sb.Append(this.batchSeqNumber);
            sb.Append(this.sendingCarrierSidBid);
            sb.Append(this.receivingCarrierSidBid);
            sb.Append(this.totalNumberRecordsInBatch);
            sb.Append(this.batchTotalChargesAndTaxes);
            sb.Append(this.settlementPeriod);
            sb.Append(this.clearinghouseId);
            sb.Append(this.batchTotalChargesAndTaxesSign);
            sb.Append(this.originalTotalChargesAndTaxesSign);
            sb.Append(this.systemReservedFiller1);
            sb.Append(this.originalTotalNumberOfRecords);
            sb.Append(this.originalTotalChargesAndTaxes);
            sb.Append(this.systemReservedFiller2);
            sb.Append(this.currencyType);
            sb.Append(this.localCarrierReserved);
            sb.Append(this.systemReservedFiller3);

            return sb.ToString();

        }

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

        public string ReceivingCarrierSIDBID
        {
            get
            {
                return this.receivingCarrierSidBid;
            }
        }

        public string SendingCarrierSIDBID
        {
            get
            {
                return this.sendingCarrierSidBid;
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

        public string TotalNumberRecordsInBatch
        {
            get
            {
                return this.totalNumberRecordsInBatch;
            }
            set
            {
                // accepted values = 0000-9999
                if (value.Length == 4)
                    totalNumberRecordsInBatch = value.PadLeft(4, this.pad);

                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:TotalNumberRecordsInBatch");
            }
        }

        public string BatchTotalChargesAndTaxes
        {
            get
            {
                return this.batchTotalChargesAndTaxes;
            }
            set
            {
                // $$$$$$$$$$CC where C = cents sign
                if (value.Length == 12)
                    batchTotalChargesAndTaxes = value.PadLeft(12, this.pad);

                else
                    throw new CiberRecordException("InvalidFieldLengthForParameter:BatchTotalChargesAndTaxes");
            }
        }
    }
}
