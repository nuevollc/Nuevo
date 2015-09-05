using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Cdr.Rating
{
    public class CiberRecordInfo
    {

        private char zero_pad = Convert.ToChar("0");
        private char blank_pad = Convert.ToChar(" ");

        private string batchCreationDate;
        private string batchSequenceNumber;
        private string sendingCarrierSidBid;
        private string receivingCarrierSidBid;
        private string clearingHouseId;

        //make these configurable params later
        private string currencyType = "01";
        private string ciberRecordReleaseNumber = "25";

        //private string totalNumberOfRecordsInBatch = "0000";
        // private string batchTotalChargesAndTaxes = "000000000000";
        
        private int batchTotalChargesAndTaxes = 0;

        private int totalNumberOfRecordsInBatch = 0;

        private string originalTotalNumberOfRecords;
        private string originalTotalChargesAndTaxes;

        private string settlementPeriod;

        
        public CiberRecordInfo()
        {
        }

        public int TotalNumberOfRecordsInBatch
        {
            get
            {
                return this.totalNumberOfRecordsInBatch;
            }
            set
            {
                // add intelligence to format the data
                this.totalNumberOfRecordsInBatch = value; // String.Format("{0:D4}", value);
            }
        }
        public string TotalNumberOfRecordsInBatchString
        {
            get
            {
                return this.totalNumberOfRecordsInBatch.ToString("D4");
            }
        }

        public int BatchTotalChargesAndTaxes
        {
            get
            {
                return this.batchTotalChargesAndTaxes;
            }
            set
            {
                // add intelligence to format the data
                this.batchTotalChargesAndTaxes = value; // String.Format("{0:D12}", value);
            }
        }

        public string BatchTotalChargesAndTaxesString
        {
           get
            {
                return this.batchTotalChargesAndTaxes.ToString("D12");
            }
        }

        public string ReceivingCarrierSidBid
        {
            get
            {
                return this.receivingCarrierSidBid;
            }
            set
            {
                // add intelligence to format the data
                this.receivingCarrierSidBid = value;
            }
        }


        public string SendingCarrierSidBid
        {
            get
            {
                return this.sendingCarrierSidBid;
            }
            set
            {
                // add intelligence to format the data
                this.sendingCarrierSidBid = value;
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
                this.settlementPeriod = value;
            }
        }
        public string BatchCreationDate
        {
            get
            {
                return this.batchCreationDate;
            }
            set
            {
                // add intelligence to format the data
                this.batchCreationDate = value;
            }
        }
        public string BatchSequenceNumber
        {
            get
            {
                return this.batchSequenceNumber;
            }
            set
            {
                if (value.Length == 3)
                    batchSequenceNumber = value;
                else if (value.Length <= 3)
                    batchSequenceNumber = value.PadLeft(3, zero_pad);
                else if (value.Length > 3)
                    throw new ApplicationException("InvalidValueForParameter:BatchSequenceNumber");
            }
        }
        public string ClearinghouseId
        {
            get
            {
                return this.clearingHouseId;
            }
            set
            {
                // add intelligence to format the data
                this.clearingHouseId = value;
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
                // add intelligence to format the data
                this.currencyType = value;
            }
        }
        public string CiberRecordReleaseNumber
        {
            get
            {
                return this.ciberRecordReleaseNumber;
            }
            set
            {
                // add intelligence to format the data
                this.ciberRecordReleaseNumber = value;
            }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("BatchCreationDate::" + BatchCreationDate + " ");
            s.Append("BatchSeqeunceNumber::" + BatchSequenceNumber + " ");
            s.Append("SendingCarrierSidBid::" + SendingCarrierSidBid + " ");
            s.Append("ReceivingCarrierSidBid::" + ReceivingCarrierSidBid + " ");
            s.Append("BatchTotalChargesAndTaxes::" + BatchTotalChargesAndTaxes + " ");
            s.Append("TotalNumberOfRecordsInBatch::" + TotalNumberOfRecordsInBatch + " ");
            return s.ToString();

        }

    }
}
