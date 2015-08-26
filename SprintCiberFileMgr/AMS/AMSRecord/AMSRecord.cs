using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Sprint.AMS
{


    /// <summary>
    /// definition of the SPRINT AMS record
    /// </summary>
    public class AMSRecord
    {
        // if true, object is loaded
        private bool loaded = false;

        private string accountStatus;
        private string callingStationId;
        private string ipAddress;
        private string userName;
        private string acctSessionId;
        private string correlationId;
        private string sessionCont;
        private string haipAddress;
        private string nasipAddress;
        private string pcfipAddress;
        private string bsid;
        private string userZone;
        private string fmux;
        private string rmux;
        private string serviceOption;
        private string ftype;
        private string rtype;
        private string fsize;
        private string frc;
        private string rrc;
        private string ipTech;
        private string compFlag;
        private string reasonInd;
        private string g3pp2Fsize;
        private string ipQos;
        private string airQos;
        private string acctOutput;
        private string acctInput;
        private string badFrameCount;
        private string eventTimestamp;
        private string activeTime;
        private string numActive;
        private string sdbInputOctet;
        private string sdbOutputOctet;
        private string sdbNumInput;
        private string sdbNumOutput;
        private string totalBytesRecvd;
        private string sipInboundCount;
        private string sipOutboundCount;
        private string pdsnVendorId;
        private string aaaId;
        private string eamsId;
        private string eamsPreProcessTime;
        private string eamsPostProcessTime;
        private string gmtOffset;
        private string deltaAcctOutputOctets;
        private string deltaAcctInputOctets;
        private string deltaActiveTime;


        public AMSRecord(string[] ams)
        {
            if ( ams.Count().Equals(49) )
            {
                // object is loaded
                Loaded = true;

                AccountStatus = ams[0];
                CallingStationId = ams[1];
                IpAddress = ams[2];
                UserName = ams[3];
                AccountSessionId = ams[4];
                CorrelationId = ams[5];
                SessionCont = ams[6];
                HaIpAddress = ams[7];
                NasIpAddress = ams[8];
                PcfIpAddress = ams[9];
                BSID = ams[10];
                UserZone = ams[11];
                FMUX = ams[12];
                RMUX = ams[13];
                ServiceOption = ams[14];
                Ftype = ams[15];
                Rtype = ams[16];
                Fsize = ams[17];
                Frc = ams[18];
                Rrc = ams[19];
                IpTech = ams[20];
                CompFlag = ams[21];
                ReasonInd = ams[22];
                G3pp2Fsize = ams[23];
                IpQos = ams[24];
                AirQos = ams[25];
                AcctOutput = ams[26];
                AcctInput = ams[27];
                BadFrameCount = ams[28];
                EventTimeStamp = ams[29];
                ActiveTime = ams[30];
                NumActive = ams[31];
                SdbInputOctet = ams[32];
                SdbOutputOctet = ams[33];
                SdbNumInput = ams[34];
                SdbNumOutput = ams[35];
                TotalBytesReceived = ams[36];
                SipInboundCount = ams[37];
                SipOutboundCount = ams[38];
                PdsnVendorId = ams[39];
                AaaId = ams[40];
                EamsId = ams[41];
                EamsPreProcessTime = ams[42];
                EamsPostProcessTime = ams[43];
                GmtOffset = ams[44];
                DeltaAcctOutputOctets = ams[45];
                DeltaAcctInputOctets = ams[46];
                DeltaActiveTime = ams[48];
            }
                
        }

        /// <summary>
        /// assessors
        /// </summary>

        public bool Loaded
        {
            get
            {
                return this.loaded;
            }
            set
            {
                loaded = value;
            }
        }

        public string AccountStatus
        {
            get
            {
                return this.accountStatus;
            }
            set
            {
                accountStatus = value;
            }
        }
        public string CallingStationId
        {
            get
            {
                return this.callingStationId;
            }
            set
            {
                callingStationId = value;
            }
        }
        public string IpAddress
        {
            get
            {
                return this.ipAddress;
            }
            set
            {
                ipAddress = value;
            }
        }
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                userName = value;
            }
        }
        public string AccountSessionId
        {
            get
            {
                return this.acctSessionId;
            }
            set
            {
                acctSessionId = value;
            }
        }
        public string CorrelationId
        {
            get
            {
                return this.correlationId;
            }
            set
            {
                correlationId = value;
            }
        }
        public string SessionCont
        {
            get
            {
                return this.sessionCont;
            }
            set
            {
                sessionCont = value;
            }
        }
        public string HaIpAddress
        {
            get
            {
                return this.haipAddress;
            }
            set
            {
                haipAddress = value;
            }
        }
        public string NasIpAddress
        {
            get
            {
                return this.nasipAddress;
            }
            set
            {
                nasipAddress = value;
            }
        }
        public string PcfIpAddress
        {
            get
            {
                return this.pcfipAddress;
            }
            set
            {
                pcfipAddress = value;
            }
        }
        public string BSID
        {
            get
            {
                return this.bsid;
            }
            set
            {
                bsid = value;
            }
        }
        public string UserZone
        {
            get
            {
                return this.userZone;
            }
            set
            {
                userZone = value;
            }
        }
        public string FMUX
        {
            get
            {
                return this.fmux;
            }
            set
            {
                fmux = value;
            }
        }
        public string RMUX
        {
            get
            {
                return this.rmux;
            }
            set
            {
                rmux = value;
            }
        }
        public string ServiceOption
        {
            get
            {
                return this.serviceOption;
            }
            set
            {
                serviceOption = value;
            }
        }
        public string Ftype
        {
            get
            {
                return this.ftype;
            }
            set
            {
                ftype = value;
            }
        }
        public string Rtype
        {
            get
            {
                return this.rtype;
            }
            set
            {
                rtype = value;
            }
        }
        public string Fsize
        {
            get
            {
                return this.fsize;
            }
            set
            {
                fsize = value;
            }
        }
        public string Frc
        {
            get
            {
                return this.frc;
            }
            set
            {
                frc = value;
            }
        }
        public string Rrc
        {
            get
            {
                return this.rrc;
            }
            set
            {
                rrc = value;
            }
        }
        public string IpTech
        {
            get
            {
                return this.ipTech;
            }
            set
            {
                ipTech = value;
            }
        }
        public string CompFlag
        {
            get
            {
                return this.compFlag;
            }
            set
            {
                compFlag = value;
            }
        }
        public string ReasonInd
        {
            get
            {
                return this.reasonInd;
            }
            set
            {
                reasonInd = value;
            }
        }
        public string G3pp2Fsize
        {
            get
            {
                return this.g3pp2Fsize;
            }
            set
            {
                g3pp2Fsize = value;
            }
        }
        public string IpQos
        {
            get
            {
                return this.ipQos;
            }
            set
            {
                ipQos = value;
            }
        }
        public string AirQos
        {
            get
            {
                return this.airQos;
            }
            set
            {
                airQos = value;
            }
        }
        public string AcctOutput
        {
            get
            {
                return this.acctOutput;
            }
            set
            {
                acctOutput = value;
            }
        }
        public string AcctInput
        {
            get
            {
                return this.acctInput;
            }
            set
            {
                acctInput = value;
            }
        }
        public string BadFrameCount
        {
            get
            {
                return this.badFrameCount;
            }
            set
            {
                badFrameCount = value;
            }
        }
        public string EventTimeStamp
        {
            get
            {
                return this.eventTimestamp;
            }
            set
            {
                eventTimestamp = value;
            }
        }
        public string ActiveTime
        {
            get
            {
                return this.activeTime;
            }
            set
            {
                activeTime = value;
            }
        }
        public string NumActive
        {
            get
            {
                return this.numActive;
            }
            set
            {
                numActive = value;
            }
        }
        public string SdbInputOctet
        {
            get
            {
                return this.sdbInputOctet;
            }
            set
            {
                sdbInputOctet = value;
            }
        }
        public string SdbOutputOctet
        {
            get
            {
                return this.sdbOutputOctet;
            }
            set
            {
                sdbOutputOctet = value;
            }
        }
        public string SdbNumInput
        {
            get
            {
                return this.sdbNumInput;
            }
            set
            {
                sdbNumInput = value;
            }
        }
        public string SdbNumOutput
        {
            get
            {
                return this.sdbNumOutput;
            }
            set
            {
                sdbNumOutput = value;
            }
        }
        public string TotalBytesReceived
        {
            get
            {
                return this.totalBytesRecvd;
            }
            set
            {
                totalBytesRecvd = value;
            }
        }
        public string SipInboundCount
        {
            get
            {
                return this.sipInboundCount;
            }
            set
            {
                sipInboundCount = value;
            }
        }
        public string SipOutboundCount
        {
            get
            {
                return this.sipOutboundCount;
            }
            set
            {
                sipOutboundCount = value;
            }
        }
        public string PdsnVendorId
        {
            get
            {
                return this.pdsnVendorId;
            }
            set
            {
                pdsnVendorId = value;
            }
        }
        public string AaaId
        {
            get
            {
                return this.aaaId;
            }
            set
            {
                aaaId = value;
            }
        }
        public string EamsId
        {
            get
            {
                return this.eamsId;
            }
            set
            {
                eamsId = value;
            }
        }
        public string EamsPreProcessTime
        {
            get
            {
                return this.eamsPreProcessTime;
            }
            set
            {
                eamsPreProcessTime = value;
            }
        }
        public string EamsPostProcessTime
        {
            get
            {
                return this.eamsPostProcessTime;
            }
            set
            {
                eamsPostProcessTime = value;
            }
        }
        public string GmtOffset
        {
            get
            {
                return this.gmtOffset;
            }
            set
            {
                gmtOffset = value;
            }
        }
        public string DeltaAcctOutputOctets
        {
            get
            {
                return this.deltaAcctInputOctets;
            }
            set
            {
                deltaAcctInputOctets = value;
            }
        }
        public string DeltaAcctInputOctets
        {
            get
            {
                return this.deltaAcctOutputOctets;
            }
            set
            {
                deltaAcctOutputOctets = value;
            }
        }
        public string DeltaActiveTime
        {
            get
            {
                return this.deltaActiveTime;
            }
            set
            {
                deltaActiveTime = value;
            }
        }

    }

}
