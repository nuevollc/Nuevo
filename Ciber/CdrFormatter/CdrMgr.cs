using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Strata8.Voip.Cdr
{


    /// <summary>
    /// singleton class used to store the cdrs that will be written to the
    /// cdr file.
    /// </summary>
    sealed class CdrMgr
    {
        // singleton class
        private static volatile CdrMgr instance;
        private static object syncRoot = new Object();

        // the cdr list 
        private List<Bcdr> m_cdrList = new List<Bcdr>();

        private CdrMgr()
        {
        }

        public static CdrMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CdrMgr();
                    }
                }
                return instance;

            }

        }// CdrMgr Instance

        public void AddCdr(Bcdr b)
        {
            // S8 is not billing On-Waves for MT 
            if ( b.Direction.Equals( CdrDirection.Terminating.ToString()) || b.Direction.Equals(","))
                return;

            // if not already in the list, add it
            // otherwise just return
            if ( !m_cdrList.Contains( b ) )
                m_cdrList.Add(b);
            
            return;
        }

        public void ClearCdrList()
        {
            m_cdrList.Clear();
        }

        // accessors, only can get the list
        public List<Bcdr> CdrList
        {
            get
            {
                return m_cdrList;
            }
        }


    }// CdrMgr

}// namespace
