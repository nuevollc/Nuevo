using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Data
{
    public enum TechDataEnums
    {
        SIDBID_NOT_FOUND = 0
    }

    public class TechData
    {

        private string carrierName;
        private string sidBid;
        private string sidName;
        private string state;
        private string band;
        private string mbiMin;
        private string lowLineRange;
        private string highLineRange;
        private string hlrPointCode;
        private string hlrClliCode;
        private string hlrMscid;
        private string hlrEsid;
        private string transmittedCdmaSid;
        private string gteTsiCarrierCode;
        private string rateCenter;
        private string rateCenterState;
        private string county;
        private string mtasLikeCode;
        private string rbtEmrsGroup;


        /// <summary>
        /// ctor that takes a string array to load the object
        /// </summary>
        /// <param name="d"></param>
        public TechData(string[] d)
        {
            carrierName = d[0];
            sidBid = d[1];
            sidName = d[2];
            state = d[3];
            band = d[4];
            mbiMin = d[5];
            lowLineRange = d[6];
            highLineRange = d[7];
            hlrPointCode = d[8];
            hlrClliCode = d[9];
            hlrMscid = d[10];
            hlrEsid = d[11];
            transmittedCdmaSid = d[12];
            gteTsiCarrierCode = d[13];
            rateCenter = d[14];
            rateCenterState = d[15];
            county = d[16];
            mtasLikeCode = d[17];
            rbtEmrsGroup = d[18];

        }// ctor


        // accessors
        public string CarrierName
        {
            get
            {
                return this.carrierName;
            }
            set
            {
                carrierName = value;
            }
        }
        public string SidBid
        {
            get
            {
                return this.sidBid;
            }
            set
            {
                sidBid = value;
            }
        }
        public string SidName
        {
            get
            {
                return this.sidName;
            }
            set
            {
                sidName = value;
            }
        }
        public string State
        {
            get
            {
                return this.state;
            }
            set
            {
                state = value;
            }
        }
        public string Band
        {
            get
            {
                return this.band;
            }
            set
            {
                band = value;
            }
        }
        public string MbiMin
        {
            get
            {
                return this.mbiMin;
            }
            set
            {
                mbiMin = value;
            }
        }
        public string LowLineRange
        {
            get
            {
                return this.lowLineRange;
            }
            set
            {
                lowLineRange = value;
            }
        }
        public string HighLineRange
        {
            get
            {
                return this.highLineRange;
            }
            set
            {
                highLineRange = value;
            }
        }
        public string HlrPointCode
        {
            get
            {
                return this.hlrPointCode;
            }
            set
            {
                hlrPointCode = value;
            }
        }
        public string HlrClliCode
        {
            get
            {
                return this.hlrClliCode;
            }
            set
            {
                hlrClliCode = value;
            }
        }
        public string HlrMscid
        {
            get
            {
                return this.hlrMscid;
            }
            set
            {
                hlrMscid = value;
            }
        }
        public string HlrEsid
        {
            get
            {
                return this.hlrEsid;
            }
            set
            {
                hlrEsid = value;
            }
        }
        public string TransmittedCdmaSid
        {
            get
            {
                return this.transmittedCdmaSid;
            }
            set
            {
                transmittedCdmaSid = value;
            }
        }
        public string GteTsiCarrierCode
        {
            get
            {
                return this.gteTsiCarrierCode;
            }
            set
            {
                gteTsiCarrierCode = value;
            }
        }
        public string RateCenter
        {
            get
            {
                return this.rateCenter;
            }
            set
            {
                rateCenter = value;
            }
        }
        public string RateCenterState
        {
            get
            {
                return this.rateCenterState;
            }
            set
            {
                rateCenterState = value;
            }
        }
        public string County
        {
            get
            {
                return this.county;
            }
            set
            {
                county = value;
            }
        }
        public string MtasLikeCode
        {
            get
            {
                return this.mtasLikeCode;
            }
            set
            {
                mtasLikeCode = value;
            }
        }
        public string RbtEmrsGroup
        {
            get
            {
                return this.rbtEmrsGroup;
            }
            set
            {
                rbtEmrsGroup = value;
            }
        }
     
    }// class

}//namespace

