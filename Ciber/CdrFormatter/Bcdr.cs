using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata8.Wireless.Cdr;
using Strata8.Wireless.Cdr.Rating;

using Strata8.Wireless.Db;

namespace Strata8.Voip.Cdr
{

// cdr format looks like this:
//line[1]  = version=14.2 encoding=ISO-8859-1
//line[2]  = 460474845c4d1e20090126203000.1380-080000,,Start
//line[cdr]= 460475845c4d1e20090126202952.1930-080000,Strata8_sp,Normal,+14256057933,,Originating,+14256057933,Public,9785354400,20090126202952.193,0-080000,Yes,20090126203015.470,20090126203021.732,016,VoIP,,9785354400,interlat,to,+19785354400,,local,10.0.13.30:5060,BW122952211260109-193381513@10.0.10.10,PCMU/8000,10.0.11.50,0006d725-f08d0016-73cae3cb-263b0e3b@10.4.1.116,,,,Redapt_gp,Name Dialing,,,,,,,,,y,public,,2397541:0,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,4256057933@strata8.net,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,25.828,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
//line[n]  = 460563845c4d1e20090126204500.1160-080000,,End
     
    public enum CdrCallCategory
    {    
        internat,
        interlat,
        national,
        intralat,
    }

    public enum CdrDirection
    {
        Originating = 1,
        Terminating = 2
    }

    public enum CdrReleasingParty
    {
        local=1,
        remote = 2
    }

    public enum CdrVessel
    {
        SeattleTest=0,
        WindSpirit=1,
        Lirica=2,
        Orchestra=3,
        WindStar=4,
        IceLandLab=5      
    }


    public class Bcdr
    {
        // the CDR line to write to the file
        private string[] cdr = new string[196];

        // private database manager 
        private CiberDbMgr m_dbMgr = new CiberDbMgr();

        public Bcdr()
        {
            // initialize params
            Init();
        }
        
        /// <summary>
        /// private method to initialize parameters of the object
        /// </summary>
        private void Init()
        {
            //for (int i = 0; i < 196; i++)
            //{
            //    cdr[i] = ",";
            //}

            // constants
            cdr[1] = "OnWaves_sp";// serviceprovider
            cdr[2] = "Normal";// type
            cdr[7] = "Public";// callingpresentationindicator
            cdr[10] = "0-080000";// time zone
            cdr[11] = "Yes";// answer indicator
            cdr[14] = "016";// terminationCause
            cdr[15] = "VoIP";// networkType
            cdr[18] = "interlat";// callCategory
            cdr[19] = "to";// networkCallType
            cdr[31] = "OnWaves";// group
            cdr[41] = "y";// chargeIndicator

        }

        private void CreateDefaults()
        {
            for (int i = 0; i < 196; i++)
            {
                cdr[i] = ",";
            }
        }
        /// <summary>
        /// public method to construct the bworks cdr from the omc cdr
        /// </summary>
        /// <param name="ocdr"></param>
        /// <returns></returns>
        public Bcdr(OmcCdr ocdr)
        {
            // initialize some params
            Init();
            
            this.RecordId = ocdr.SequenceNumber;
            if ((ocdr.disc_code.Equals("201")) || (ocdr.disc_code.Equals("202")))
            {
                if (ocdr.A_Party_Type.Equals("1") || ocdr.A_Party_Type.Equals("0"))
                {// MO
                    this.Direction = CdrDirection.Originating.ToString();

                    if (ocdr.B_Party_Digits.StartsWith("011"))
                    {
                        DialedDigits = ocdr.B_Party_Digits;
                        CalledNumber = ocdr.B_Party_Digits;
                        NetworkCallType = "in";
                        CallCategory = "internat";
                    }
                    else
                    {
                        DialedDigits = "+1" + ocdr.B_Party_Digits;
                        CalledNumber = "+1" + ocdr.B_Party_Digits;

                    }

                    // create the userid based on cellid here
                    string uId = m_dbMgr.GetUserIdForCellId(ocdr.CellId);
                    if (uId.Equals(String.Empty))
                        // not mapped then we user the MDN
                        UserNumber = "+1" + ocdr.OriginatingMsisdn;
                    else
                        UserNumber = uId; // already with leading +1

                    UserId = UserNumber;
                    CallingNumber = "+1" + ocdr.A_Party_Num;
                }

                else
                { // MT ( not billing On-Waves for mobile termination
                    this.Direction = CdrDirection.Terminating.ToString();
                    // not billing mobile termination, don't create the CDR
                    CreateDefaults();
                }

                // get cell id to 700 number mapping
                // get the calling number
                // called number
                StartTime = ConvertTheTime(ocdr.SeizeTime);
                AnswerTime = ConvertTheTime(ocdr.AnswerTime);
                ReleaseTime = ConvertTheTime(ocdr.DisconnectTime);
            }
            else
            {
                CreateDefaults();
                return;
            }
        }


        // assessors        
        public string RecordId
        {
            get
            {
                return cdr[0];
            }
            set
            {
                cdr[0] = value;
            }
        }
        public string ServiceProvider
        {
            get
            {
                return cdr[1];
            }
            set
            {
                cdr[1] = value;
            }
        }
        public string Direction
        {
            get
            {
                return cdr[5];
            }
            set
            {
                cdr[5] = value;
            }
        }
        public string UserNumber
        {
            get
            {
                return cdr[3];
            }
            set
            {
                cdr[3] = value;
            }
        }
        public string CallingNumber
        {
            get
            {
                return cdr[6];
            }
            set
            {
                cdr[6] = value;
            }
        }        
        public string CalledNumber
        {
            get
            {
                return cdr[8];
            }
            set
            {
                cdr[8] = value;
            }
        }       
        public string StartTime
        {
            get
            {
                return cdr[9];
            }
            set
            {
                cdr[9] = value;
            }
        }
        public string AnswerTime
        {
            get
            {
                return cdr[12];
            }
            set
            {
                cdr[12] = value;
            }
        }        
        public string ReleaseTime
        {
            get
            {
                return cdr[13];
            }
            set
            {
                cdr[13] = value;
            }
        }       
        public string DialedDigits
        {
            get
            {
                return cdr[17];
            }
            set
            {
                cdr[17] = value;
            }
        }
        public string CallCategory
        {
            get
            {
                return cdr[18];
            }
            set
            {
                cdr[18] = value;
            }
        }       
        public string NetworkCallType
        {
            get
            {
                return cdr[19];
            }

            set
            {
                cdr[19] = value;
            }
        }
        public string ReleasingParty
        {
            get
            {
                return cdr[22];
            }
            set
            {
                cdr[22] = value;
            }
        }
        public string Group
        {
            get
            {
                return cdr[31];
            }
            set
            {
                cdr[31] = value;
            }
        }
        public string ChargeIndicator
        {
            get
            {
                return cdr[41];
            }
            set
            {
                cdr[41] = value;
            }
        }
        public string CallingPartyCategory
        {
            get
            {
                return cdr[46];
            }
            set
            {
                cdr[46] = value;
            }
        }
        public string UserId
        {
            get
            {
                return cdr[120];
            }
            set
            {
                cdr[120] = value;
            }
        }

        private string ConvertTheTime( string mscTimeFormat )
        {
            //time formats HHMMSS: HH = 00-23, MM = 00-59, SS = 00-59
            int y = Convert.ToInt32(mscTimeFormat.Substring(0, 4));
            int m = Convert.ToInt32(mscTimeFormat.Substring(5, 2));
            int d = Convert.ToInt32(mscTimeFormat.Substring(8, 2));
            int hr = Convert.ToInt32(mscTimeFormat.Substring(11, 2));
            int mins = Convert.ToInt32(mscTimeFormat.Substring(14, 2));
            int secs  = Convert.ToInt32(mscTimeFormat.Substring(17, 2));
            DateTime dt = new DateTime( y, m, d, hr, mins, secs );

            // bworks time format
            string dts = dt.ToString( "yyyyMMddHHmmsss.fff" );
            return dts;

        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < cdr.Length; i++)
            {
                if ( i.Equals( cdr.Length-1) )
                    sb.Append(cdr[i]);
                else
                    sb.Append( cdr[i]+"," );
            }
            
            return  sb.ToString();
        }
    }// class

} //namespace
 

