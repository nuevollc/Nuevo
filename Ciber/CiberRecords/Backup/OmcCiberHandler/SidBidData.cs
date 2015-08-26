using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Strata8.Wireless.Db;

namespace Strata8.Wireless.Cdr.Ciber
{


    /// <summary>
    /// class used to manage the various HomeCarrier SID/BIDs that may show up on the network
    /// There are two hashtables used, one is for the SID/BID file info that is accessed via
    /// SID/BID.  The file info contains parameters like the batchSequence number, HomeCarrierSidBid,
    /// relevant and used to create the batch header/trailer records.
    /// The second hashTable is used to store the CIBER records for each SID/BID, the CIBER records
    /// for each SID/BID is accessed via the SID/BID.
    /// For each HomeCarrier SID/BID there is a CiberRecordInfo object and Ciber records of 
    /// type 22 created and stored in the hash tables.
    /// The ServingCarrierInfoMgr contains a hashtable of this object so that for each Serving Carrier SID/BID
    /// there there could be many HomeCarrier SID/BIDs.
    /// 
    /// </summary>
    /// 
    
    public sealed class SidBidData
    {

        private CiberDbMgr m_dbMgr = new CiberDbMgr();

        public SidBidData()
        {
            //ctor logic here
        }

        private Hashtable m_sidBidRecords = new Hashtable();
        private Hashtable m_sidBidFileInfo = new Hashtable();

        /// <summary>
        /// method to add a list of record type 22 for a specific SID/BID
        /// </summary>
        /// <param name="sidBid"></param>
        /// <param name="record22List"></param>
        public void AddSidBidRecords( string sidBid, List<Record22> record22List )
        {
            m_sidBidRecords.Add(sidBid, record22List);
        }

        public void AddSidBidRecord(string sidBid, Record22 r22)
        {
            // if our sidbid exists, add to the list
            if (m_sidBidRecords.ContainsKey(sidBid))
            {
                List<Record22> rList = (System.Collections.Generic.List<Record22>)m_sidBidRecords[sidBid];
                rList.Add(r22);
            }
            else
            {
                List<Record22> rList = new List<Record22>();
                rList.Add( r22 );
                m_sidBidRecords.Add(sidBid, rList);
            }

        }

        public void AddSidBidFileInfo(string sidBid, CiberRecordInfo cri)
        {
                m_sidBidFileInfo[sidBid] = cri;

        }

        public void RemoveSidBidInfo(string sidBid)
        {
            m_sidBidFileInfo.Remove(sidBid);
        }

        public void ClearSidBidInfo()
        {
            m_sidBidFileInfo.Clear();
            m_sidBidRecords.Clear();

        }

        public void RemoveSidBidRecords(string sidBid)
        {
            m_sidBidRecords.Remove(sidBid);
        }

        /// <summary>
        /// returns a null lis
        /// </summary>
        /// <param name="sidBid"></param>
        /// <returns></returns>
        public List<Record22> GetRecordsForSidBid(string sidBid)
        {
            List<Record22> rList = null;

            if (m_sidBidRecords.ContainsKey(sidBid))
            {
                // return the list for the sidBid
                rList = (System.Collections.Generic.List<Record22>)m_sidBidRecords[sidBid];
            }

            return rList;

        }
        /// <summary>
        /// returns the ciber record info object populated with the latest data
        /// if this is a newly created SID/BID, then a new sequence number is generated
        /// based on the latest sequence number being used for the home carrier SID/BID. 
        /// the sequence number is stored in the database for each home carrier SID/BID
        /// in the database and incremented when a new one is used.
        /// </summary>
        /// <param name="sidBid"></param>
        /// <returns></returns>
        public CiberRecordInfo GetFileInfoForSidBid(string homeCarrierSidBid, string servingCarrierSidBid)
        {
            CiberRecordInfo rInfo;

            if (m_sidBidFileInfo.ContainsKey( homeCarrierSidBid ))
            {
                // return the list for the sidBid
                rInfo = (CiberRecordInfo)m_sidBidFileInfo[ homeCarrierSidBid ];

                return rInfo;
            }
            else
            {
                // new entry 
                rInfo = new CiberRecordInfo();

                // get a new sequence number for our batch based on what has been submitted
                // previously and we update the sequence number for the next time around
                // this is a critical piece used to keep in synch with what the clearinghouse
                // expects.  If this is out of synch, then the CIBER batch will be rejected.
                int seqNum = m_dbMgr.GetSequenceNumber( homeCarrierSidBid, servingCarrierSidBid );
                rInfo.BatchSequenceNumber = seqNum.ToString();
                m_dbMgr.UpdateSequenceNumber( homeCarrierSidBid , servingCarrierSidBid );

                rInfo.ReceivingCarrierSidBid = homeCarrierSidBid;
                return rInfo;
            }

        }

        public string GetSidBidForCellId(string cellId)
        {
            // leverage our db interface to get the SID/BID for this cellId
            string sidBid = m_dbMgr.GetSidBidForCellId(cellId);
            return sidBid;
        }


        // accessors to get the HashTables
        public Hashtable SidBidRecords
        {
            get
            {
                return m_sidBidRecords;
            }
            set
            {
                m_sidBidRecords = value;
            }
        }

        public Hashtable SidBidInfo
        {
            get
            {
                return m_sidBidFileInfo;
            }
            set
            {
                m_sidBidFileInfo = value;
            }
        }


    }// class

}//namespace
