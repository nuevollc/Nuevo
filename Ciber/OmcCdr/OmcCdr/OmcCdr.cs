using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata8.Wireless.Db;

namespace Strata8.Wireless.Cdr
{
    public class OmcCdr
    {
        public string version; // record version, identifies the format of the record.
        public string seq_num; // indicates a unique instance of the record over the switch lifespan.
        public string type; // indicates what type of record it is, 1=CallRecord or a 2=SMS type

        /// calling party number
        /// contains: IMSI or MIN if the party is a mobile subscriber
        /// ANI iof the party if resulting from land origination.
        public string a_party_num;

        /// called party number; contains the following
        /// IMSI or MIN if the party is a mobile subscriber
        /// it will be empty if land termination unless party was found in the ani-sub or card-sub DBs.
        public string b_party_num;

        // type associated with the originating party : MOBILE, LAND, FEATURE, FORWARD, THREEWAYCALL, ...
        // see the CDR spec from Star Solutions for additonal valid formats
        public string a_party_type;

        // type associated with the terminating party
        public string b_party_type;

        // calling party digits
        public string a_party_digits;
        public string b_party_digits;

        // the system port number the originating party used to originate the call
        public string a_party_trunk;
        public string b_party_trunk;
        public string a_party_trkgrp; // the a party trunk group
        public string b_party_trkgrp; // the b party trunk group
        public string seize; // time when an originating event arrives at the MSC
        public string answer; //indicates when the terminating party answered the call
        public string disc; // indicates when the call leg has ended

        private DateTime s;//seize time as datetime
        private DateTime a; // answer time as datetime
        private DateTime d; // disconnect time as datetime

        public string disc_code;  // disconnect code, the reason that a call could not be completed or was eventually disconnected.
        public string disc_reason; // disconnect reason.
        public string msc_id;  // the MSC where the record was created
        public string orig_esn; // for future enhancement
        private string term_esn; // for future enhancement

        /// <summary>
        /// cell identifier : indicates the originating/terminating mobiles cell. 
        /// In CDMA it represents CI value in cell table, 16 bit of cell id including 4-bit sector ID.
        /// </summary>
        public string cell_id;  // cell identifier.  the originating/terminating mobiles cell. 

        public string b_cell_id; //
        public string a_feature_bits; // for an sms record, this field is empty.
        public string b_feature_bits; // for an sms record, this field is empty.
        public string o_msisdn; // identifies the calling party
        public string t_msisdn; // identifies the called party
        public string o_exchange; // 
        public string t_exchange; // identifies the called party's LRN

        /// these three fields identify the billing record id of this CDR is this is
        /// the originating call.  otherwise these 3 fields identify the terminating CDR that corresponds to this CDR
        public string o_market_id;
        public string o_swno;
        public string o_bin;

        /// these three fields identify the billing record id of this CDR is this is
        /// the terminating call.  otherwise these 3 fields identify the originating CDR that corresponds to this CDR
        public string t_market_id;
        public string t_swno;
        public string t_bin;

        /// <summary>
        /// the billing digits associated with the A party mobile.  If this is not a mobile originated call or 
        /// HLR, SCF do not have billing digits it is 0.
        /// </summary>
        public string o_billdgts;
        public string t_billdgts; // B party mobile, if not a mobile terminated call, it is zero.

        public string o_serviceid;
        public string t_serviceid;

        public string crg_charge_info; // charge information passed via the ISUP CRG message.

        /// <summary>
        /// if mobile originating this field contains the A party MDN.  If land originating, this field contains
        /// the A party's number.  If party type is FORWARD, the field contains the original A party's MDN (if mobile) or 
        /// A party's number if landline.
        /// </summary>
        string ocpn;
        /// <summary>
        /// immediate calling party number or redirect number.  
        /// </summary>
        private string icprn;

        /// <summary>
        /// indcates timestamp when the last intermediary CDR was created for this call.
        /// </summary>
        string last_interim;


        // accessors
        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                version = value;
            }

        }// Version

        public string SequenceNumber
        {
            get
            {
                return this.seq_num;
            }
            set
            {
                seq_num = value;
            }
        }// Sequence Number

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                type = value;
            }
        }// Type

        
        public string A_Party_Num
        {
            get
            {
                return this.a_party_num;
            }
            set
            {
                a_party_num = value;
            }
        }//a_party_num

        public string B_Party_Num
        {
            get
            {
                return this.b_party_num;
            }
            set
            {
                b_party_num = value;
            }
        }// b_party_num

        public string A_Party_Type
        {
            get
            {
                return this.a_party_type;
            }
            set
            {
                a_party_type = value;
            }
        }// a_party_type

        public string B_Party_Type
        {
            get
            {
                return this.b_party_type;
            }
            set
            {
                b_party_type = value;
            }
        }// b_party_type

        public string A_Party_Digits
        {
            get
            {
                return this.a_party_digits;
            }
            set
            {
                a_party_digits = value;
            }
        }
        public string B_Party_Digits
        {
            get
            {
                return this.b_party_digits;
            }
            set
            {
                b_party_digits = value;
            }
        }

        public string A_Party_Trunk
        {
            get
            {
                return this.a_party_trunk;
            }
            set
            {
                a_party_trunk = value;
            }
        }
        public string B_Party_Trunk
        {
            get
            {
                return this.b_party_trunk;
            }
            set
            {
                b_party_trunk = value;
            }
        }

        // unconverted string times from the MSC
        public string SeizeTime
        {
            get
            {
                return this.seize;
            }
            set
            {
                seize = value;
            }
        }
        public string AnswerTime
        {
            get
            {
                return this.answer;
            }
            set
            {
                answer = value;
            }
        }
        public string DisconnectTime
        {
            get
            {
                return this.disc;
            }
            set
            {
                disc = value;
            }
        }

        public DateTime Seize
        {
            get
            {
                return this.s;
            }
            set
            {
                s = value;
            }
        }
        public DateTime Answer
        {
            get
            {
                return this.a;
            }
            set
            {
                a = value;
            }
        }
        public DateTime Disconnect
        {
            get
            {
                return this.d;
            }
            set
            {
                d = value;
            }
        }

        public string DisconnectCode
        {
            get
            {
                return this.disc_code;
            }
            set
            {
                disc_code = value;
            }
        }
        public string DisconnectReason
        {
            get
            {
                return this.disc_reason;
            }
            set
            {
                disc_reason = value;
            }
        }

        public string MscId
        {
            get
            {
                return this.msc_id;
            }
            set
            {
                msc_id = value;
            }
        }

        public string OriginatingEsn
        {
            get
            {
                return this.orig_esn;
            }
            set
            {
                orig_esn = value;
            }
        }

        public string TerminatingEsn
        {
            get
            {
                return this.term_esn;
            }
            set
            {
                term_esn = value;
            }
        }
        public string B_CellId
        {
            get
            {
                return this.b_cell_id;
            }
            set
            {
                b_cell_id = value;
            }
        }
        public string CellId
        {
            get
            {
                return this.cell_id;
            }
            set
            {
                cell_id = value;
            }
        }

        public string A_Feature_Bits
        {
            get
            {
                return this.a_feature_bits;
            }
            set
            {
                a_feature_bits = value;
            }
        }
        public string B_Feature_Bits
        {
            get
            {
                return this.b_feature_bits;
            }
            set
            {
                b_feature_bits = value;
            }
        }
        public string OriginatingMsisdn
        {
            get
            {
                return this.o_msisdn;
            }
            set
            {
                o_msisdn = value;
            }
        }
        public string TerminatingMsisdn
        {
            get
            {
                return this.t_msisdn;
            }
            set
            {
                t_msisdn = value;
            }
        }

        public string OriginatingExchange
        {
            get
            {
                return this.o_exchange;
            }
            set
            {
                o_exchange = value;
            }
        }
        public string TerminatingExchange
        {
            get
            {
                return this.t_exchange;
            }
            set
            {
                t_exchange = value;
            }
        }
        public string OriginatingMarketId
        {
            get
            {
                return this.o_market_id;
            }
            set
            {
                o_market_id = value;
            }
        }
        public string TerminatingMarketId
        {
            get
            {
                return this.t_market_id;
            }
            set
            {
                t_market_id = value;
            }
        }
        public string OriginatingSwno
        {
            get
            {
                return this.o_swno;
            }
            set
            {
                o_swno = value;
            }
        }
        public string TerminatingSwno
        {
            get
            {
                return this.t_swno;
            }
            set
            {
                t_swno = value;
            }
        }

        public string OriginatingBillingDigits
        {
            get
            {
                return this.o_billdgts;
            }
            set
            {
                o_billdgts = value;
            }
        }

        public string A_Party_TrunkGroup
        {
            get
            {
                return this.a_party_trkgrp;
            }
            set
            {
                a_party_trkgrp = value;
            }
        }
        public string B_Party_TrunkGroup
        {
            get
            {
                return this.b_party_trkgrp;
            }
            set
            {
                b_party_trkgrp = value;
            }
        }

        public string TerminatingBillingDigits
        {
            get
            {
                return this.t_billdgts;
            }
            set
            {
                t_billdgts = value;
            }
        }

        public string OriginatingBillingId
        {
            get
            {
                return this.o_bin;
            }
            set
            {
                o_bin = value;
            }
        }
        public string TerminatingBillingId
        {
            get
            {
                return this.t_bin;
            }
            set
            {
                t_bin = value;
            }
        }

        public string OriginatingServiceId
        {
            get
            {
                return this.o_serviceid;
            }
            set
            {
                o_serviceid = value;
            }
        }
        public string TerminatingServiceId
        {
            get
            {
                return this.t_serviceid;
            }
            set
            {
                t_serviceid = value;
            }
        }

        public string CRG_Charge_Info
        {
            get
            {
                return this.crg_charge_info;
            }
            set
            {
                crg_charge_info = value;
            }
        }

        public string Ocpn
        {
            get
            {
                return this.ocpn;
            }
            set
            {
                ocpn = value;
            }
        }// ocprn        
        
        public string Icprn
        {
            get
            {
                return this.icprn;
            }
            set
            {
                icprn = value;
            }
        }// icprn

        public string ToReportStringFormat()
        {
            // seqNum, siteName, type, discMessage, discCode, APartyNum, BPartyNum,APartyType,BPartyType,APartyDigits,BPartyDigits,
            // SeizeTime, AnswerTime, DisconnectTime, CallDuration, CellId, OMSISDN, 
            string comma = ",";                  
            char zero_pad = Convert.ToChar("0");

            // only convert the fields being used in our report
            StringBuilder sb = new StringBuilder();

            sb.Append(SequenceNumber + ",");
            string recordType = String.Empty;
            int theType = Convert.ToInt32(Type);

            switch ( theType )
            {
                case (int) OmcCdrEnums.RecordType.Call_Record:
                    recordType = OmcCdrEnums.RecordType.Call_Record.ToString();
                    break;
                case (int) OmcCdrEnums.RecordType.SMS_Record:
                    recordType = OmcCdrEnums.RecordType.SMS_Record.ToString();
                    break;
            }

            // vessel id
            CiberDbMgr cDb = new CiberDbMgr();
            string siteName = cDb.GetSiteNameForCellId(CellId);
            sb.Append(siteName + comma);
            sb.Append(recordType + comma);

            DisconnectCodeMgr dMgr = new DisconnectCodeMgr();
            string dMessage = dMgr.GetDisconnectMessage(DisconnectCode);
            sb.Append(dMessage + comma);
            sb.Append(DisconnectCode + comma);

            sb.Append(A_Party_Num + comma );
            sb.Append(B_Party_Num + comma );
            string atype = dMgr.GetPartyType(A_Party_Type);
            string btype = dMgr.GetPartyType(B_Party_Type);
            sb.Append(atype + comma );
            sb.Append(btype + comma);
            sb.Append(A_Party_Digits + comma );
            sb.Append(B_Party_Digits + comma );
            sb.Append(Seize.ToString("s") + comma);
            sb.Append(Answer.ToString("s") + comma);
            sb.Append( Disconnect.ToString("s") + comma );

            // call duration

            TimeSpan callDuration = Disconnect.Subtract( Seize );
            string airChargeTime = callDuration.Hours.ToString().PadLeft(2, zero_pad) + ":" + callDuration.Minutes.ToString().PadLeft(2, zero_pad) + ":" + callDuration.Seconds.ToString("D2") ;
            sb.Append(airChargeTime + comma);

            return sb.ToString();

        }

    }// class

}//namespace
