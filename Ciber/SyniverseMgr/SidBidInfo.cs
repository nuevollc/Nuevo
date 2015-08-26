using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPCS.SyniverseMgr.Db
{
    public class SidBidInfo
    {
        private string _homeCarrierSidBid;
        private string _servingCarrierSidBid;
        private string _sequenceNumber;

        public string HomeCarrierSidBid
        {
            get
            {
                return this._homeCarrierSidBid;
            }
            set
            {
                _homeCarrierSidBid = value;

            }
        } //HomeCarrierSidBid

        public string ServingCarrierSidBid
        {
            get
            {
                return this._servingCarrierSidBid;
            }
            set
            {
                _servingCarrierSidBid = value;

            }

        } //ServingCarrierSidBid

        public string SequenceNumber
        {
            get
            {
                return this._sequenceNumber;
            }
            set
            {
                _sequenceNumber = value;

            }

        } //SequenceNumber
    }
}
