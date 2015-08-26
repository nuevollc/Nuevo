using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Strata8.Wireless;
using Strata8.Wireless.Data;
using Strata8.Wireless.Cdr;
using Strata8.Did;
using Strata8.Voip.Cdr;
using Strata8.Wireless.Utils;

namespace Strata8.Wireless.Cdr.Ciber
{

    /// <summary>
    /// class used to create the CIBER files
    /// </summary>
    public sealed class CiberFileCreator
    {
        private static volatile CiberFileCreator instance;
        private static object syncRoot = new Object();
        
        private CiberFileCreator()
        {
        }

        public static CiberFileCreator Instance
        {
            get
            {
                if ( instance == null )
                {
                    lock(syncRoot)
                    {
                        if ( instance == null )
                            instance = new CiberFileCreator();
                    }
                }
                return instance;

            }
        }

        // object used to create the ciber file
        private FileWriter m_cfw = FileWriter.Instance;
        private CiberRater m_cr = new CiberRater();

        private NpaNxxReader npaNxxRdr = new NpaNxxReader();
        // only one to make sure that we manage our Sid/Bid info
        private static ServingCarrierInfoMgr m_servingCarrierInfoMgr = ServingCarrierInfoMgr.Instance;
        // cdr formatter used to create bworks cdrs
        private CdrFormatter m_cdrFormatter = new CdrFormatter();

        // padding characters
        private char m_zero_pad = Convert.ToChar("0");
        private char m_blank_pad = Convert.ToChar(" ");

        private const string CurrencyType = "01"; // US Dollars
        private const string ClearingHouseId = "0"; // From a Billing Vendor or Carrier
        private const string BatchContents = "1"; // CIBER type 22, 32, and 52 records
        private const string CiberBatchRejectReasonCode = "00"; // initial batch submission
        private const string OriginalReturnIndicator = "1"; // Valid Initial Issue
        private const string CiberRecordReleaseNumber = "25";
        private const string Record01Type = "01";

        // this will be determined by the cell id parameters, per vessel
        // ** add code to determine the cell id
        // private const string SendingCarrierSidBid = "44150"; // 10789 On-Waves SID/BID

        /// <summary>
        /// method that creates CIBER records based on the information processed by the 
        /// processCallRecords() method and stored in the SidBidManager object.  
        /// Information stored includes the CIBER Record 22 and related CIBER info 
        /// stored in the CiberRecordInfo used to create the CIBER record 01 and 98, 
        /// header and trailer records respectively.
        /// </summary>
        //public void CreateCiberRecords( string fileName )
        //{
        //    // ************************************************************************
        //    // ** for now the assumption is that we create new file for every batch
        //    // or in our case for every MSC CDR file that we process.  May change this 
        //    // later
        //    // *************************************************************************

        //    // get the SID/BID info
        //    Hashtable criList = m_sidBidMgr.SidBidInfo;

        //    // for each SID/BID : create the header, record22, and the trailer in the file
        //    foreach (DictionaryEntry de in criList )
        //    {
        //        CiberRecordInfo cri = (CiberRecordInfo) de.Value;
        //        List<Record22> r22List = SidBidManager.Instance.GetRecordsForSidBid(cri.ReceivingCarrierSidBid);

        //        // since we are changing the values of the record stored in the dictionary
        //        // this avoids throwing the exception and having to catch it
        //        CiberRecordInfo ncri = new CiberRecordInfo();
        //        ncri = cri;

        //        // create the batch header and write it to the file
        //        Record01 r1 = CreateBatchHeaderRecord( ref ncri );
        //        m_cfw.WriteToCiberFile(fileName, r1.ToCiberStringFormat());

        //        foreach (Record22 r22 in r22List)
        //        {
        //            // finally write this cdr to the file
        //            m_cfw.WriteToCiberFile(fileName, r22.ToCiberStringFormat());
        //        }

        //        // create the trailer record for this Sid/Bid  based on the new cri
        //        Record98 r98 = CreateBatchTrailerRecord( ncri );
        //        m_cfw.WriteToCiberFile(fileName, r98.ToCiberStringFormat());

        //    }

        //    // clean up our hashtables/lists
        //    m_sidBidMgr.ClearSidBidInfo();
        //}

        /// <summary>
        /// method that creates CIBER records based on the information processed by the 
        /// processCallRecords() method and stored in the SidBidManager object.  
        /// Information stored includes the CIBER Record 22 and related CIBER info 
        /// stored in the CiberRecordInfo.  The CiberRecordInfo is used to create the 
        /// CIBER record type 01 header and type 98 trailer records. 
        ///
        /// </summary>
        public void CreateCiberRecordsNew(string fileName)
        {
            // ************************************************************************
            // ** for now the assumption is that we create new file for every batch
            // or in our case for every MSC CDR file that we process.  May change this 
            // later
            // *************************************************************************

            List<Bcdr> cdrList = new List<Bcdr>();

            // for each pico (servingCarrierSidBid) we will create a batch
            Hashtable sites = ServingCarrierInfoMgr.Instance.SiteData;

            // for each servingSidBid
            foreach (DictionaryEntry de in sites)
            {
                // get the hash table of the ciber records and of the data
                SidBidData siteData = (SidBidData)de.Value;

                Hashtable criHash = siteData.SidBidInfo;

                // for each receiving SID/BID : create the header, record22, and the trailer in the file
                foreach (DictionaryEntry cri in criHash)
                {
                    CiberRecordInfo ciberInfo = (CiberRecordInfo)cri.Value;

                    List<Record22> r22List = siteData.GetRecordsForSidBid(ciberInfo.ReceivingCarrierSidBid);

                    // since we are changing the values of the record stored in the dictionary
                    // this avoids throwing the exception and having to catch it
                    CiberRecordInfo ncri = new CiberRecordInfo();
                    ncri = ciberInfo;

                    // create the batch header and write it to the file
                    Record01 r1 = CreateBatchHeaderRecord(ref ncri);
                    m_cfw.WriteToCiberFile(fileName, r1.ToCiberStringFormat());

                    foreach (Record22 r22 in r22List)
                    {
                        // finally write this cdr to the file
                        m_cfw.WriteToCiberFile(fileName, r22.ToCiberStringFormat());
                    }

                    // create the trailer record for this Sid/Bid  based on the new cri
                    Record98 r98 = CreateBatchTrailerRecord(ncri);
                    m_cfw.WriteToCiberFile(fileName, r98.ToCiberStringFormat());

                }// for each homecarriersidbid

            }// for each site
            // clean up our hashtables/lists
            ServingCarrierInfoMgr.Instance.DeleteDataForServingSidBid();

        }// CreateCiberRecords

        /// <summary>
        /// method used to create the CIBER header Record 01 for a receiving carrier Sid/Bid
        /// </summary>
        /// <returns></returns>
        public Record01 CreateBatchHeaderRecord( ref CiberRecordInfo cri )
        {
            string rcarrierSidBid = String.Empty;

            // for testing, create an example header format          
            //string hdrExample = "010705210290653885493251010310152001208040784     EX0433";
            /// example = "01 070521 029 33333 01151 25 1 01 031015 2 00 1 208040784     EX0433";
            /// RecordType =                    01
            /// CreationDate =                  070521
            /// BatchSequenceNumber =           030
            /// 
            /// this field is used to identify the market and consequently the carrier that provided
            /// services to a roaming mobile station after handing off from an anchor system.
            /// SendingCarrierSID/BID :         10789  //OnWaves SID/BID
            /// 
            /// This field is used to identify the intended recipient of a batch of CIBER records
            /// ReceivingCarrierSID/BID :       00133  // WTF is this??
            /// 
            /// 
            /// CIBER record release number :   25
            /// Original Return Indicator :     1
            /// Currency Type :                 01
            /// SettlePeriod :                  031015
            /// clearinghouseid :               0
            /// CIBER Batch Reject reason code: 00
            /// Batch contents :                1 // 
            /// LocalCarrierReserved :          208040784     EX0433
            /// 

            // add code to validate the receivingCarrierSidBid format of 5 characters
            if ( cri.ReceivingCarrierSidBid.Length == 5)
                rcarrierSidBid = cri.ReceivingCarrierSidBid;
            else if (cri.ReceivingCarrierSidBid.Length <= 5)
                rcarrierSidBid = cri.ReceivingCarrierSidBid.PadLeft(5, m_zero_pad);
            else if (cri.ReceivingCarrierSidBid.Length > 5)
                throw new ApplicationException("InvalidValueForParameter:RecievingCarrierSidBid");

            // create a batch date and settlement date
            DateTime d = DateTime.Now;
            string yy = d.Year.ToString().Substring(2, 2);
            string dd = d.Day.ToString("D2");
            string mm = d.Month.ToString("D2");
            string hour = d.Hour.ToString();
            string batchCreationDate = yy + mm + dd;

            // always settle on the 15th, for now assuming 3 mos for settlement expiration
            int settlementMonth = (d.Month + 3) % 12;
            string sm = String.Format("{0:D2}", settlementMonth);

            // for now user 2009, update this to use modulo as well
            string settlementPeriod = "09" + sm + "15";

            string carrierFiller = String.Empty.PadRight(20 , m_blank_pad);
            string systemFiller = String.Empty.PadRight(144, m_blank_pad);
            string filler = carrierFiller + systemFiller;

            // dynamic fields are the ReceivingCarrierSidBid
            StringBuilder strb = new StringBuilder( Record01Type + batchCreationDate + cri.BatchSequenceNumber + cri.SendingCarrierSidBid );
            strb.Append( rcarrierSidBid + CiberRecordReleaseNumber + OriginalReturnIndicator + CurrencyType );
            strb.Append( settlementPeriod + ClearingHouseId + CiberBatchRejectReasonCode + BatchContents  + filler ) ;

            Ciber.Record01 r1 = new Record01( strb.ToString() );

            //write it to a file for now
            //m_cfw.WriteToFile( r1.ToCiberStringFormat() );

            // update our Ciber Record Info used in the trailer record
            cri.BatchCreationDate = r1.BatchCreationDate;
            cri.ReceivingCarrierSidBid = r1.ReceivingCarrierSIDBID;
            cri.CiberRecordReleaseNumber = r1.CiberRecordReleaseNumber;
            cri.CurrencyType = r1.CurrencyType;
            cri.ClearinghouseId = r1.ClearingHouseId;
            cri.SettlementPeriod = r1.SettlementPeriod;

            //store it for use to create our trailer record
            // m_sidBidMgr.AddSidBidFileInfo(rcarrierSidBid, cri);

            // return the record
            return r1;

        }
               
        /// <summary>
        /// method used to create the CIBER trailer Record type 98
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="cri"></param>
        /// <returns></returns>
        public Record98 CreateBatchTrailerRecord(  CiberRecordInfo cri )
        {                                                                                                          
            /// example = "9807052102906358854930002000000000399031015200   0002000000000399        01208040784                                                                                                            C0000100";
            /// RecordType =                    98
            /// CreationDate =                  070521
            /// BatchSequenceNumber =           029
            /// SendingCarrierSID/BID :         10789  //OnWaves SID/BID
            /// ReceivingCarrierSID/BID :       00133
            /// Total Records in batch  :       0002
            /// Batch total charges & taxes :   000000000399
            /// SettlePeriod :                  031015
            /// clearinghouseid :               0 // from a Billing Vendor or a Carrier
            /// Batch total charges and taxes sign: 0
            /// original total charges and taxes sign : 0
            /// system reserved filler:            3N : Blank fill
            /// original total number of records:  4N : 0002
            /// original total charges and taxes: 12N : 000000000399
            /// system reserved filler : 66-73 8N : CIBERNET USE Blank fill
            /// currency type :     74-75 2N : 01
            /// local carrier reserved          : 20N  ( OPTIONAL )
            /// System reserved filler          : 105N ( OPTIONAL )
            /// 

            // create our filler here
            string carrierFiller = String.Empty.PadRight(20, m_blank_pad);
            string systemFiller = String.Empty.PadRight(105, m_blank_pad);
            string filler = carrierFiller + systemFiller;

            StringBuilder sb = new StringBuilder( "98" + cri.BatchCreationDate + cri.BatchSequenceNumber + cri.SendingCarrierSidBid + cri.ReceivingCarrierSidBid);
            sb.Append( cri.TotalNumberOfRecordsInBatchString + cri.BatchTotalChargesAndTaxesString + cri.SettlementPeriod + cri.ClearinghouseId );
            
            // note the blanks have to be in here, need to make these constants and add them here
            sb.Append("00   " + cri.TotalNumberOfRecordsInBatchString + cri.BatchTotalChargesAndTaxesString + "        01");
            
            sb.Append(filler);
            Ciber.Record98 r98 = new Record98( sb.ToString() );

            //write it to a file for now
            //m_cfw.WriteToFile( r98.ToCiberStringFormat() );
            return r98;

        }

        /// <summary>
        ///  
        /// method to convert the OMC CDR to a CIBER Type 22 record
        ///  need to keep track of the total charges and taxes here
        ///  911, 611 and 411 calls should be blocked at the network level
        ///  if they are not, the CIBER records are not created/submitted at this level
        ///  
        /// </summary>
        /// <param name="cdr"></param>
        public void ProcessCallRecords ( List<OmcCdr> cdrList )
        {
            //  22 0 00 000 01151 2 912223280900000 10 912223280900000 1 0610328563400000000 33333 0000000099 0 00000000000000000000000705022220000000000000000010912223280900000000000000000000000000000000000000001  029000000000189900000000000212127000100000051021000000009900000000000000                            000J    INCOMING  CLUSAJACKSON   OHUSA00000000000000000000000000000000000000000000000000000000000000000000000000208040784PIKE  0188                                                                                                                                   
            // 220000000115129122232809000001091222328090000010610328563400000000333330000000099000000000000000000000000705022220000000000000000010912223280900000000000000000000000000000000000000001  
            // record type : 22
            // return code : 0
            // ciber return reason code 2N : 00
            // invalidfieldidentifier 3N : 000
            // homecarriersidbid 5N : 01151
            // msid indicator 1N = 2 
            // msid 15N : 912223280900000
            // MSISDN/MDN length 2N : 10
            // MSISDN/MDN 15N : 912223280900000
            // ESN/UIMID/EIMIE/MEID indicator 1N :1
            // ESN/UIMID/EIMIE/MEID 19N : 0610328563400000000
            // ServingCarrierSidBid 5N :33333
            // TotalChargesAndTaxes 10N : 0000000099
            /// system reserved filler1 1N : BlankFILL
            /// total state province taxes 10N

            // update our running total
            // cri.BatchTotalChargesAndTaxes
            
            // create a CIBER record
            // for now it is a type 22 record

            foreach (OmcCdr cdr in cdrList)
            {
                Record22 r22 = new Record22();
                r22.ReturnCode = "0";
                r22.CiberRecordReturnReasonCode = "00";
                r22.InvalidFieldIndentifier = "000";
                r22.MsidIndicator = "2";  // MIN baby                
                string servingCarrierSidBid = String.Empty;  // calculated based on the cell id
                string cellId = String.Empty;

                // if not a call record CDR 
                if ( !cdr.Type.Equals("1") )
                {                    // should be blocked, no CIBER to be submitted
                    m_cfw.WriteToLogFile("CiberFileCreator::ConvertToCiber():NonCallRecordCDRDetectedFor:" + cdr.SequenceNumber + "\r\n");
                    continue;
                }

                // if 911 call, don't process it
                if (cdr.A_Feature_Bits.Equals("EMERGENCY"))
                {
                    // should be blocked, no CIBER to be submitted
                    m_cfw.WriteToLogFile("CiberFileCreator::ConvertToCiber():911CallDetectedForCDRSeqNum:" + cdr.SequenceNumber + "\r\n");
                    continue;
                }

                try
                {
                    // if we have a *good* call, then create the CIBER record and for now if it is *bad* call
                    // we log it to the log file.  Figure out what to do with it, create the CIBER or not.
                    if ((cdr.disc_code.Equals("201")) || (cdr.disc_code.Equals("202")) || (cdr.disc_code.Equals("203")) || (cdr.disc_code.Equals("204")))
                    {
                        r22.CallCompletionIndicator = "2";
                        r22.CallTerminationIndicator = "2"; // 2 = Normal
                    }
                    else
                    {
                        // double check this
                        r22.CallCompletionIndicator = "1";
                        r22.CallTerminationIndicator = "3"; // 3 = unknown, 4=incomplete call
                        //m_cfw.WriteToLogFile("CiberFileCreator::ConvertToCiber():InvalidDisconnectCode-CallNotTerminatedForCDRSeqNum:" + cdr.SequenceNumber + "\r\n");
                        continue;
                    }

                    // look for the mobile party in our scenario we will always have one mobile unit and one
                    // landline, we always grab mobile unit parameters.
                    // MO Case:
                    if (cdr.A_Party_Type.Equals("1") || cdr.A_Party_Type.Equals("0"))
                    {
                        // grab the a party mobile params
                        // MIN
                        r22.Msid = cdr.a_party_num;

                        // MDN  ** double check this and might change it to be the cdr.ocpn field **
                        r22.MsisdnMdnLength = cdr.o_msisdn.Length.ToString();
                        r22.MsisdnMdn = cdr.o_msisdn;

                        if (cdr.OriginatingEsn.Length == 0)
                        {   // ESN/UIMID/IMEI/MEID 
                            // If the ESN/IMEI/MEID Indicator equals “0,” 
                            // then the ESN/IMEI/MEID field must be zero filled.
                            // If the ESN/IMEI/MEID Indicator equals 1, 2, or 3, then the 
                            // ESN/IMEI/MEID field must contain a value within the range of accepted values specified above.
                            r22.EsnUimidImeiMeidIndicator = "0"; // ESN
                            r22.EsnUimidImeiMeid = "000000";
                        }
                        else
                        {
                            r22.EsnUimidImeiMeidIndicator = "1"; // ESN

                            // add code to convert this according to Appendix B
                            UInt32 starEsn = Convert.ToUInt32(cdr.OriginatingEsn, 16);
                            string esn = CalculateEsnValue(starEsn);
                            r22.EsnUimidImeiMeid = esn;
                        }

                        //The value of the Caller ID is dependent on the call direction.
                        //•For Mobile Originated Calls (Call Direction equals “1”), the Caller ID is equal to the Mobile Directory Number.
                        // which in our case is the following field in the type 22 record
                        r22.CallerIdLength = r22.MsisdnMdnLength;
                        r22.CallerId = r22.MsisdnMdn;

                        // For Mobile Originated Calls, the Called Number Digits represents the Dialed Digits.
                        r22.CalledNumberLength = cdr.b_party_digits.Length.ToString();
                        r22.CalledNumberDigits = cdr.b_party_digits;

                        // MO no TLDN in our scenario
                        r22.TldnLength = "0";
                        r22.Tldn = "0";

                        // set our call direction : mobile roamer originated = 1, mobile roamer terminated = 2
                        r22.CallDirection = "1";

                        // get the cellid from the cellid field in the CDR
                        cellId = cdr.CellId;
                    }
                    else
                    {
                        // MT Case:
                        // assume that b party is the mobile party and we grab the b party mobile params
                        // grab the b party mobile params
                        // MIN
                        r22.Msid = cdr.b_party_num;

                        // MDN
                        r22.MsisdnMdnLength = cdr.t_msisdn.Length.ToString();
                        r22.MsisdnMdn = cdr.t_msisdn;

                        if (cdr.TerminatingEsn.Length == 0)
                        {   // ESN/UIMID/IMEI/MEID 
                            // If the ESN/IMEI/MEID Indicator equals “0,” 
                            // then the ESN/IMEI/MEID field must be zero filled.
                            // If the ESN/IMEI/MEID Indicator equals 1, 2, or 3, then the 
                            // ESN/IMEI/MEID field must contain a value within the range of accepted values specified above.
                            r22.EsnUimidImeiMeidIndicator = "0"; // ESN
                            r22.EsnUimidImeiMeid = "000000";
                        }
                        else
                        {
                            r22.EsnUimidImeiMeidIndicator = "1"; // ESN

                            // add code to convert this according to Appendix B
                            UInt32 starEsn = Convert.ToUInt32(cdr.TerminatingEsn, 16);
                            string esn = CalculateEsnValue(starEsn);
                            r22.EsnUimidImeiMeid = esn;
                        }

                        //•For Mobile Terminated Calls (Call Direction equals “2”), the Caller ID is the number of the calling party.
                        r22.CallerIdLength = cdr.a_party_num.Length.ToString();
                        r22.CallerId = cdr.a_party_num;

                        // For Mobile Terminated Calls (Call Direction equals “2”), the Called Number Digits value MUST equal 
                        // the Mobile Directory Number value.
                        r22.CalledNumberLength = r22.MsisdnMdnLength;  //cdr.t_msisdn.Length.ToString();
                        r22.CalledNumberDigits = r22.MsisdnMdn; // ** when we have Verizon phones it should be :** cdr.t_msisdn;

                        // landline to Mobile, MT
                        r22.TldnLength = cdr.a_party_digits.Length.ToString();
                        r22.Tldn = cdr.A_Party_Digits;

                        // set our call direction : mobile roamer originated = 1, mobile roamer terminated = 2
                        r22.CallDirection = "2";

                        // get the cellid from the cellid field in the CDR
                        cellId = cdr.B_CellId;

                    }

                    // this field is used to indicate the home carrier of the roaming mobile as indicated by the MSID.
                    // The MSID must be valid for the specified home carrier SID/BID.
                    // look up the HomeCarrierSIDBID based on the MSID
                    string sidBid = GetHomeCarrierSidBid(r22.Msid);
                    if (sidBid.Equals(TechDataEnums.SIDBID_NOT_FOUND.ToString()))
                    {
                        //m_cfw.WriteToLogFile("CiberFileCreator::ProcessCallRecord():SIDBIDNOTFoundForMSID: " + r22.Msid);
                        continue;
                    }
                    else
                    {
                        r22.HomeCarrierSidBid = sidBid;
                        m_cfw.WriteToLogFile("CiberFileCreator::ProcessCallRecord():SIDBIDFoundForMSID: " + r22.Msid + " Seq#:" + cdr.SequenceNumber);
                    }

                    // this is the On-Wave SID/BID, if it is mapped to the Strata8 SID/BID 6538 or not mapped at all, we don't create CIBER
                    servingCarrierSidBid = ServingCarrierInfoMgr.Instance.GetSidBidForCellId( cellId );
                    if ( servingCarrierSidBid.Equals("6538") || servingCarrierSidBid.Equals( String.Empty ) )
                    {
                        // if Strata8=6538 or pico is not mapped in database; do NOT create CIBER
                        m_cfw.WriteToLogFile("CiberFileCreator::ProcessCallRecord():ServingCarrierSIDBIDNotFoundForCellID: " + cellId );
                        continue;
                    }
                    else
                    {
                        r22.ServingCarrierSidBid = servingCarrierSidBid;
                    }

                    // yymmdd
                    r22.CallDate = cdr.answer.Substring(2, 2) + cdr.answer.Substring(5, 2) + cdr.answer.Substring(8, 2);

                    // This field is used to record the routing number associated with a ported or pooled MDN.
                    r22.LocationRoutingNumberLengthIndicator = "0";
                    r22.LocationRoutingNumber = "0";

                    r22.CurrencyType = "1";

                    r22.InitialCellSite = cdr.cell_id;
                    r22.TimeZoneIndicator = "99"; //=08 Pacific, for this test use the unknown value of "99";
                    r22.DaylightSavingIndicator = "0";
                    r22.MessageAccountingDigits = "0";

                    // This field is used to record the time that the mobile unit successfully connected to a wireless system.
                    // HHMMSS: HH = 00-23, MM = 00-59, SS = 00-59
                    r22.AirConnectTime = cdr.answer.Substring(11, 2) + cdr.answer.Substring(14, 2) + cdr.answer.Substring(17, 2);

                    // fill these in : ( disc time - seize time )
                    // This field is used to record the billable elapsed time used to calculate air time charges
                    string ChargeableTime = CalculateAirChargeTime(cdr.DisconnectTime, cdr.SeizeTime);

                    // if chargeableTime is zero then we do not bill it
                    if (ChargeableTime.Equals("000000"))
                        continue;

                    r22.AirChargeableTime = ChargeableTime; //  "0001030"; // contains the billable elapsed time in MMMM=0000-9999, SS=00-59

                    // This field is used to record the elapsed time associated with a call.
                    // MMMMSS: MMMM = 0000-9999, SS = 00-59
                    r22.AirElapsedTime = ChargeableTime; // "000545"; // this should be disconnect - answer

                    // this needs to be determined
                    r22.AirRatePeriod = "02";
                    r22.AirMultiRatePeriod = "1";// 1 = single rate period

                    // This field is used to record the charges associated with the air time portion of a call. 
                    // This field must not contain any taxes; $$$$$$$$¢¢ format.
                    string rAirChargeTime = m_cr.CalculateAirTimeChargeAmount(r22.AirChargeableTime);
                    r22.AirCharge = rAirChargeTime; // "99";

                    // ** calculate this as well
                    r22.OtherChargeNumberOneIndicator = "0";
                    r22.OtherChargeNumberOne = "0";

                    r22.PrintedCall = "0";

                    // determine how we determine if it is fraudulent or not
                    r22.FraudIndicator = "00";
                    r22.FraudSubIndicator = "0";

                    // special condition when it is a zero, properties get/set take care of this
                    r22.SpecialFeaturesUsed = "0";

                    // ** fix this to the following
                    // this field contains the service call descriptor, section 8 table 18 OR the geographic  name of the 
                    // called place.
                    // called place depends on the call direction
                    // for MO, calledDirection=1, the called place is the place that the mobile sub
                    // is calling
                    // for MT, calledDirection=2, the called place is the place where the mobile sub answered the call
                    if (r22.CallDirection.Equals("2"))
                    { // MT CALLs
                        // Mobile terminated calls all MT calls are airtime only charges Rate = 1.49
                        r22.CalledPlace = "INCOMING";
                        r22.CalledStateProvince = "CL";
                        r22.CalledCountry = "USA";  // **check to see if we are going to make this International
                        r22.SpecialFeaturesUsed = "J"; // no toll
                        r22.TollTariffDescriptor = "00"; // na
                        r22.TollRateClass = "0"; // 0-when no toll associated with this call

                        // No TOLL CHARGES for MT
                        r22.TollConnectTime = "000000";
                        r22.TollElapsedTime = "000000";
                        r22.TollChargeableTime = r22.TollElapsedTime;
                        r22.TollRatePeriod = "00"; // not applicable
                        r22.TollMultiRatePeriod = "0"; // not applicable
                        r22.TollRatingPointLengthIndicator = "0";
                        r22.TollRatingPoint = "0";
                        r22.TollCharge = "00"; //
                    }
                    else if (r22.CallDirection.Equals("1"))
                    {   // MO CALLs 
                        // check for 411 call ** add others here or in separate function
                        // see section 8 table 18 
                        if (r22.CalledNumberDigits.Equals("411000000000000"))
                        { // MO:411
                            r22.CalledPlace = "DIR ASST";
                            r22.CalledStateProvince = "CL";
                            r22.CalledCountry = "USA";
                            r22.SpecialFeaturesUsed = "0"; // shows no toll charges applied
                            r22.PrintedCall = "411";

                            // rated as local/ld call including toll
                            r22.TollTariffDescriptor = "02"; // interstate interlata
                            r22.TollRateClass = "1"; // 0-when no toll associated with this call
                            r22.TollConnectTime = r22.AirConnectTime;
                            r22.TollElapsedTime = r22.AirElapsedTime;
                            r22.TollChargeableTime = r22.AirChargeableTime;
                            r22.TollRatePeriod = "01";// day, 02=evening, 03=night, 05=latenight, 06 =weekend
                            r22.TollMultiRatePeriod = "1";
                            r22.TollRatingPointLengthIndicator = "00";
                            r22.TollRatingPoint = "0";  // when TollRatingPointLengthIndicator=0, this = 0
                            string rTollChargeTime = m_cr.CalculateLocalLdTollChargeAmount(r22.TollChargeableTime);
                            r22.TollCharge = rTollChargeTime;

                            // should be blocked, no CIBER to be submitted
                            continue;

                        }
                            // check the toll-free case, 800, 877, 866, 888
                        else if ( this.CheckTollFree( r22.CalledNumberDigits ) )
                        { // MO:TOLL FREE
                            r22.CalledPlace = "TOLL FREE";
                            r22.CalledStateProvince = "CL";
                            r22.CalledCountry = "USA";
                            r22.SpecialFeaturesUsed = "0";
                            r22.PrintedCall = "TOLL FREE";

                            //r22.TollTariffDescriptor = "02"; // interstate interlata
                            //r22.TollRateClass = "0"; // 0-when no toll associated with this call
                            //// *** figure out charges for TOLL FREE
                            //r22.TollConnectTime = "000000";
                            //r22.TollElapsedTime = "000000";
                            //r22.TollChargeableTime = r22.TollElapsedTime;
                            //// no toll 
                            //r22.TollRatePeriod = "00"; // not applicable
                            //r22.TollMultiRatePeriod = "0"; // not applicable
                            //r22.TollRatingPointLengthIndicator = "0";
                            //r22.TollRatingPoint = "0"; // when TollRatingPointLengthIndicator=0, this = 0
                            //r22.TollCharge = "00"; // no toll


                            r22.TollTariffDescriptor = "02"; // interstate interlata
                            r22.TollRateClass = "1"; // 0-when no toll associated with this call

                            // toll charges 
                            r22.TollConnectTime = r22.AirConnectTime;
                            r22.TollElapsedTime = r22.AirElapsedTime;
                            r22.TollChargeableTime = r22.AirChargeableTime;

                            r22.TollRatePeriod = "01";// day, 02=evening, 03=night, 05=latenight, 06 =weekend
                            r22.TollMultiRatePeriod = "1";

                            r22.TollRatingPointLengthIndicator = "00";
                            r22.TollRatingPoint = "0";  // when TollRatingPointLengthIndicator=0, this = 0
                            string rTollChargeTime = m_cr.CalculateLocalLdTollChargeAmount(r22.TollChargeableTime);
                            r22.TollCharge = rTollChargeTime;

                        }
                        else
                        { // MO : International or Local/LD case
                            // this should map to table 31 State/Province mapping
                            NpaNxxData n = null;
                            if ( r22.CalledNumberDigits.StartsWith("1") )
                                n = npaNxxRdr.GetNpaNxxInfo(r22.CalledNumberDigits.Substring(1, 6));
                            else
                                n = npaNxxRdr.GetNpaNxxInfo(r22.CalledNumberDigits.Substring(0, 6));

                            // check International scenario
                            if (n.NpaNxx.Equals(String.Empty))
                            {   // MO:International Rate = 1.49 AirTime + 0.80 Toll
                                // International Call Scenario
                                // add the international tables mapped to table 32
                                if (r22.CalledNumberDigits.Contains("0118522810"))
                                {
                                    r22.CalledPlace = "Hong Kong";
                                    r22.CalledStateProvince = "ZZ";
                                    r22.CalledCountry = "HKG";

                                    r22.SpecialFeaturesUsed = "D"; // International, but not required.
                                    // 01 = International : Originating from a NANP and terminating to Non-NANP
                                    // 15 = International : Originating from a NANP and terminating to NANP
                                    // 17 = International : Originating from a Non-NANP and terminating to NANP
                                    // 18 = International : Originating from and terminating to Non-NANP
                                    r22.TollTariffDescriptor = "01"; // see above
                                    r22.TollRateClass = "1"; // dial station, normal completed, 0-when no toll associated with this call
                                    // toll charges 
                                    r22.TollConnectTime = r22.AirConnectTime;
                                    r22.TollElapsedTime = r22.AirElapsedTime;
                                    r22.TollChargeableTime = r22.AirChargeableTime;

                                    r22.TollRatePeriod = "01";// day, 02=evening, 03=night, 05=latenight, 06 =weekend
                                    r22.TollMultiRatePeriod = "1";

                                    r22.TollRatingPointLengthIndicator = "00";
                                    r22.TollRatingPoint = "0";  // when TollRatingPointLengthIndicator=0, this = 0
                                    string rTollChargeTime = m_cr.CalculateInternationalTollChargeAmount(r22.TollChargeableTime);
                                    r22.TollCharge = rTollChargeTime;
                                }
                                else if (r22.CalledNumberDigits.Contains("01181354"))
                                {
                                    r22.CalledPlace = "Tokyo";
                                    r22.CalledStateProvince = "ZZ";
                                    r22.CalledCountry = "JPN";

                                    r22.SpecialFeaturesUsed = "D"; // International, but not required.
                                    // 01 = International : Originating from a NANP and terminating to Non-NANP
                                    // 15 = International : Originating from a NANP and terminating to NANP
                                    // 17 = International : Originating from a Non-NANP and terminating to NANP
                                    // 18 = International : Originating from and terminating to Non-NANP
                                    r22.TollTariffDescriptor = "01"; // see above
                                    r22.TollRateClass = "1"; // dial station, normal completed, 0-when no toll associated with this call
                                    // toll charges 
                                    r22.TollConnectTime = r22.AirConnectTime;
                                    r22.TollElapsedTime = r22.AirElapsedTime;
                                    r22.TollChargeableTime = r22.AirChargeableTime;

                                    r22.TollRatePeriod = "01";// day, 02=evening, 03=night, 05=latenight, 06 =weekend
                                    r22.TollMultiRatePeriod = "1";

                                    r22.TollRatingPointLengthIndicator = "00";
                                    r22.TollRatingPoint = "0";  // when TollRatingPointLengthIndicator=0, this = 0
                                    string rTollChargeTime = m_cr.CalculateInternationalTollChargeAmount(r22.TollChargeableTime);
                                    r22.TollCharge = rTollChargeTime;
                                }
                                else if (r22.CalledNumberDigits.StartsWith("011861") || r22.CalledNumberDigits.StartsWith("86153") )
                                {
                                    r22.CalledPlace = "Beijing";
                                    r22.CalledStateProvince = "ZZ";
                                    r22.CalledCountry = "CHN";

                                    r22.SpecialFeaturesUsed = "D"; // International, but not required.
                                    // 01 = International : Originating from a NANP and terminating to Non-NANP
                                    // 15 = International : Originating from a NANP and terminating to NANP
                                    // 17 = International : Originating from a Non-NANP and terminating to NANP
                                    // 18 = International : Originating from and terminating to Non-NANP
                                    r22.TollTariffDescriptor = "01"; // see above
                                    r22.TollRateClass = "1"; // dial station, normal completed, 0-when no toll associated with this call
                                    // toll charges 
                                    r22.TollConnectTime = r22.AirConnectTime;
                                    r22.TollElapsedTime = r22.AirElapsedTime;
                                    r22.TollChargeableTime = r22.AirChargeableTime;

                                    r22.TollRatePeriod = "01";// day, 02=evening, 03=night, 05=latenight, 06 =weekend
                                    r22.TollMultiRatePeriod = "1";

                                    r22.TollRatingPointLengthIndicator = "00";
                                    r22.TollRatingPoint = "0";  // when TollRatingPointLengthIndicator=0, this = 0
                                    string rTollChargeTime = m_cr.CalculateInternationalTollChargeAmount(r22.TollChargeableTime);
                                    r22.TollCharge = rTollChargeTime;
                                }
                                else
                                {
                                    m_cfw.WriteToLogFile("CiberFileCreator::ProcessCallRecord():MOIntlCase:NPANXXNotFoundFor: " + r22.CalledNumberDigits);
                                    continue;
                                }

                            }
                            else
                            { // MO:Local/LD case  : Rate = 1.49 AirTime + 0.50 Toll
                                r22.CalledPlace = n.City;
                                r22.CalledStateProvince = n.State;
                                r22.CalledCountry = "USA";
                                r22.SpecialFeaturesUsed = "0"; // none
                                r22.TollTariffDescriptor = "02"; // interstate interlata
                                r22.TollRateClass = "1"; // 0-when no toll associated with this call

                                // toll charges 
                                r22.TollConnectTime = r22.AirConnectTime;
                                r22.TollElapsedTime = r22.AirElapsedTime;
                                r22.TollChargeableTime = r22.AirChargeableTime;

                                r22.TollRatePeriod = "01";// day, 02=evening, 03=night, 05=latenight, 06 =weekend
                                r22.TollMultiRatePeriod = "1";

                                r22.TollRatingPointLengthIndicator = "00";
                                r22.TollRatingPoint = "0";  // when TollRatingPointLengthIndicator=0, this = 0
                                string rTollChargeTime = m_cr.CalculateLocalLdTollChargeAmount(r22.TollChargeableTime);
                                r22.TollCharge = rTollChargeTime;
                            }
                        }
                    }

                    // description designed by the serving carrier and may range from a city name to a cell site or a trunk/circuit name.
                    r22.ServingPlace = "Strata8";

                    // for MO , called direction =1, this is the state that is providing service to the mobile subscriber
                    // for MT, this is the state/province providing service to the called party
                    // must contain a value given in section 8 , table 31
                    r22.ServingStateProvince = "WA";
                    r22.ServingCountry = "USA";

                    r22.TollStateProvinceTaxes = "0";
                    r22.TollLocalTaxes = "0";
                    r22.TollNetworkCarrierId = "00000"; // this is other ** TBD **  see section 8, table 10, and appendix H

                    // let's get the total state/province taxes associated with this record
                    r22.TotalStateProvinceTaxes = r22.TollStateProvinceTaxes;
                    r22.TotalLocalTaxes = r22.TollLocalTaxes;

                    // total charges is the sum of all the charge fields and taxes in this ciber record
                    // r22.TotalChargesAndTaxes = r22.TotalLocalTaxes + r22.TotalStateProvinceTaxes + r22.AirCharge + r22.TollCharge;
                    // need to convert to an int
                    int intTotalLocalTaxes = Convert.ToInt32(r22.TotalLocalTaxes);
                    int intTotalStateProvinceTaxes = Convert.ToInt32(r22.TotalStateProvinceTaxes);
                    int intAirCharge = Convert.ToInt32(r22.AirCharge);
                    int intTollCharge = Convert.ToInt32(r22.TollCharge);

                    int totalChargesAndTaxesThisRecord = intTotalLocalTaxes + intTotalStateProvinceTaxes + intAirCharge + intTollCharge;
                    // update the record total
                    r22.TotalChargesAndTaxes = totalChargesAndTaxesThisRecord.ToString();

                    // increment the record count if a valid record and the batch total charges, store the info
                    // calculate new charges for this file and maintain the total
                    // cri.BatchTotalChargesAndTaxes = cri.BatchTotalChargesAndTaxes + ... for our trailer record

                    // this will also populate the batch sequence number if this is a new HomeCarrierSidBid
                    CiberRecordInfo cri = ServingCarrierInfoMgr.Instance.GetFileInfoForSidBid(servingCarrierSidBid, r22.HomeCarrierSidBid);
                    cri.TotalNumberOfRecordsInBatch++;
                    cri.BatchTotalChargesAndTaxes = cri.BatchTotalChargesAndTaxes + totalChargesAndTaxesThisRecord;
                    // Batch Sequence Number is created if this is a new SID/BID 
                    r22.OriginalBatchSequenceNumber = cri.BatchSequenceNumber;
                    cri.SendingCarrierSidBid = r22.ServingCarrierSidBid;

                    // update the hashtables
                    // stored by Site (pico) and process by ServingCarrier SIDBID
                    
                    ServingCarrierInfoMgr.Instance.AddRecordForServingSidBid(servingCarrierSidBid, r22.HomeCarrierSidBid, cri, r22);
                    m_cfw.WriteToLogFile("CiberFileCreator::ProcessCallRecord():CiberCreatedForServingHome: " + r22.ServingCarrierSidBid + ":"+r22.HomeCarrierSidBid +"CdrSeq:"+ cdr.seq_num);
                    //SidBidManager.Instance.AddSidBidFileInfo(r22.HomeCarrierSidBid, cri);
                    //m_sidBidMgr.AddSidBidRecord(r22.HomeCarrierSidBid, r22);

                    // create the bworks cdr only if we are creating the ciber for it
                    // which means it is a valid VZW subscriber and is on one of the OnWaves picos
                    // store it in the list to write to the file later
                    CreateCdr(cdr);
                }
                catch (ApplicationException ae)
                {
                    m_cfw.WriteToLogFile( ae.Message + "\r\n" + ae.StackTrace);

                }
                catch (IOException ioe)
                {
                    m_cfw.WriteToLogFile( ioe.Message + "\r\n" + ioe.StackTrace);

                }

                // finally write this cdr to the file
                // cfw.WriteToFile(r22.ToCiberStringFormat());  

            }// for each CDR in the list


            
        }

        private string CalculateAirChargeTime(string disconnectTime, string seizeTime)
        {
            //time formats HHMMSS: HH = 00-23, MM = 00-59, SS = 00-59
            int discYear = Convert.ToInt32(disconnectTime.Substring(0,4));
            int discMonth = Convert.ToInt32(disconnectTime.Substring(5,2));
            int discDay = Convert.ToInt32(disconnectTime.Substring(8,2));
            int discHour = Convert.ToInt32(disconnectTime.Substring(11, 2));
            int discMins = Convert.ToInt32(disconnectTime.Substring(14, 2));
            int discSecs = Convert.ToInt32(disconnectTime.Substring(17, 2));
            DateTime dt = new DateTime( discYear, discMonth, discDay, discHour, discMins, discSecs );

            int answerYear = Convert.ToInt32(seizeTime.Substring(0,4));
            int answerMonth = Convert.ToInt32(seizeTime.Substring(5,2));
            int answerDay = Convert.ToInt32(seizeTime.Substring(8,2));
            int answerHour = Convert.ToInt32(seizeTime.Substring(11, 2));
            int answerMins = Convert.ToInt32(seizeTime.Substring(14, 2));
            int answerSecs = Convert.ToInt32(seizeTime.Substring(17, 2));
            DateTime da = new DateTime( answerYear, answerMonth, answerDay, answerHour, answerMins, answerSecs );

            //// billable time format is MMMMSS: MMMM = 0000-9999, SS = 00-59

            TimeSpan callDuration = dt.Subtract(da);
            int mins = callDuration.Hours * 60 + callDuration.Minutes;

            // concatenate the two strings to be in the form: MMMMSS
            string airChargeTime = mins.ToString().PadLeft(4, m_zero_pad) + callDuration.Seconds.ToString().PadLeft(2, m_zero_pad);

            // for debugging purposes
            m_cfw.WriteToLogFile("**CallDurationParams:Hours" + callDuration.Hours.ToString() + ":mins:"+callDuration.Minutes.ToString()
                + ":secs:" + callDuration.Seconds.ToString() );
            string tstr = airChargeTime.PadLeft(6, m_zero_pad);
            m_cfw.WriteToLogFile("**AirChargeTime:" + airChargeTime + ":Mins:" + mins.ToString() + "actwithzeropad:"+ tstr );
            // end debugging code 12/11/2009

            // return the charge time
            return airChargeTime;

        }

        private string CalculateEsnValue(uint starEsnFormat)
        {
            // do it in two parts, 1st part gets the manufacturers code
            // second part gets the manufactures serial number
            uint upper = 0xFF000000;
            uint lower = 0x00FFFFFF;

            long mc = upper & starEsnFormat;
            long temp = mc >> 24;
            long msn = lower & starEsnFormat;

            string mcstr = temp.ToString().PadLeft(3, m_zero_pad);
            string msnstr = msn.ToString().PadLeft(8, m_zero_pad);

            string esn = mcstr + msnstr;
            return esn;

        }

        /// <summary>
        /// method to process the SMS OMC CDRs and create a type 32 CIBER record 
        /// </summary>
        /// <param name="cdr"></param>
        public void ProcessSmsRecord(OmcCdr cdr)
        {

        }

        /// <summary>
        /// private method to get the home carrier based on MSID
        /// </summary>
        /// <param name="msid"></param>
        /// <returns></returns>
        private string GetHomeCarrierSidBid(string msid)
        {
            string sidBid = String.Empty;

            //TechDataSheetProcessor tds = new TechDataSheetProcessor();

            TDSDbMgr tds = new TDSDbMgr();
            
            // try VZW first
            sidBid = tds.GetVzwSubscriberSidBid(msid);

            // try Sprint
            if (sidBid.Equals(TechDataEnums.SIDBID_NOT_FOUND.ToString()))
            {
                sidBid = tds.GetSprintSubscriberSidBid(msid);

                if (sidBid.Equals(TechDataEnums.SIDBID_NOT_FOUND.ToString()))
                    m_cfw.WriteToLogFile("CiberFileCreator::GetHomeCarrierSidBid():SPRINT/VZW User **NOT** FoundFor MSID: " + msid);
                else
                    m_cfw.WriteToLogFile("CiberFileCreator::GetHomeCarrierSidBid():SPRINT UserFoundFor MSID: " + msid);  
            }

            return sidBid;
        }

        /// <summary>
        /// method that creates the cdr and adds it to the cdr list written to file
        /// later on in the process.
        /// </summary>
        /// <param name="omcCdr"></param>
        private void CreateCdr(OmcCdr omcCdr)
        {
            m_cdrFormatter.CreateAndAddCdr(omcCdr);
        }


        /// <summary>
        /// private method to check to see if this is a toll free call 
        /// </summary>
        /// <param name="calledNumber"></param>
        /// <returns></returns>
        private bool CheckTollFree( string calledNumber )
        {
            if ( calledNumber.StartsWith("1800") || calledNumber.StartsWith("800") )
                return true;
            else if ( calledNumber.StartsWith("1888") || calledNumber.StartsWith("888") )
                return true;
            else if ( calledNumber.StartsWith("1877") || calledNumber.StartsWith("877") )
                return true;
            else if ( calledNumber.StartsWith("1866") || calledNumber.StartsWith("866") )
                return true;
            else
                return false;
        }

    }// class

}// namespace
