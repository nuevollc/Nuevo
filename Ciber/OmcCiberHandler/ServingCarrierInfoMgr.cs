using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Strata8.Wireless.Db;

namespace Strata8.Wireless.Cdr.Rating
{


    /// <summary>
    /// class used to manage the various HomeCarrier SID/BIDs that may show up on the network
    /// There are two hashtables used, one is for the SID/BID file info that is accessed via
    /// SID/BID.  The file info contains parameters like the batchSequence number, HomeCarrierSidBid,
    /// relevant to the batch header/trailer records.
    /// The second hashTable is used to store the CIBER records for each SID/BID and the CIBER records
    /// for each SID/BID is accessed via the SID/BID.
    /// 
    /// </summary>
    /// 

    public sealed class ServingCarrierInfoMgr
    {
        private static volatile ServingCarrierInfoMgr instance;
        private CiberDbMgr m_dbMgr = new CiberDbMgr();
        private static object syncRoot = new Object();

        private ServingCarrierInfoMgr(){}

        public static ServingCarrierInfoMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ServingCarrierInfoMgr();
                    }
                }
                return instance;

            }

        }// SidBidManager

        // new way to support multiple serving carrier sid/bids and multiple home carrier sid/bids
        private static Hashtable m_picoCiberData = new Hashtable();  // ( [servingcarrier], ( hashtable[homecarrier], r22 ] ))
                                    
        public void AddRecordForServingSidBid(string servingCarrierSidBid, string homeCarrierSidBid, CiberRecordInfo cri,
            Record22 r22)
        {
            // is there already data for this servingCarrierSidBid 
            if (m_picoCiberData.ContainsKey(servingCarrierSidBid))
            {
                // grab the hashtable and add the data for this serving carrier sid bid 
                SidBidData sidBidData = (SidBidData)m_picoCiberData[servingCarrierSidBid];
                sidBidData.AddSidBidRecord(homeCarrierSidBid, r22);
                sidBidData.AddSidBidFileInfo(homeCarrierSidBid, cri);

                // no need to do this:  hashtable is updated from above
                // m_picoCiberData.Add(servingCarrierSidBid, data);
            }
            else
            {
                // create a new hashtable and list for the data
                SidBidData data = new SidBidData();
                data.AddSidBidRecord(homeCarrierSidBid, r22);
                data.AddSidBidFileInfo(homeCarrierSidBid, cri); 
                // add the hashtable with data to our site/pico hashtable
                m_picoCiberData.Add(servingCarrierSidBid, data);

            }

        }

        public void DeleteDataForServingSidBid()
        {
            m_picoCiberData.Clear();
        }

        public SidBidData GetRecordsForServingCarrierSidBid(string sidBid)
        {
            // hashtable of data containing the home carrier sidbids          
            SidBidData d = (SidBidData)m_picoCiberData[sidBid];
            return d;
        }

        public CarrierInfo GetSidBidForCellId( string cellId )
        {
            // leverage our db interface to get the SID/BID for this cellId
            CarrierInfo ci = m_dbMgr.GetSidBidForCellId(cellId);
            return ci;
        }

        public CiberRecordInfo GetFileInfoForSidBid(string servingCarrierSidBid, string homeCarrierSidBid)
        {
            if (m_picoCiberData.ContainsKey(servingCarrierSidBid))
            {
                SidBidData d = (SidBidData)m_picoCiberData[servingCarrierSidBid];
                CiberRecordInfo cri = d.GetFileInfoForSidBid(homeCarrierSidBid, servingCarrierSidBid);
                return cri;
            }
            else
            {
                SidBidData d = new SidBidData();
                CiberRecordInfo cri = d.GetFileInfoForSidBid(homeCarrierSidBid, servingCarrierSidBid);
                return cri;
            }
        }

        // accessors to get the HashTables
        public Hashtable SiteData
        {
            get
            {
                return m_picoCiberData;
            }
        }


    }// class

}//namespace
