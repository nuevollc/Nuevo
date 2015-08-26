using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using TruMobility.Utils.Logging;

namespace TruMobility.Reporting.Sprint
{
    public class WholeSaleDataProcessor
    {
        private List<string> _phoneList = new List<string>();  // list of users
        private DbProcessor _db = new DbProcessor();

        private HashSet<SprintWholesaleUser> _users = new HashSet<SprintWholesaleUser>(); 

        public void CreateWholeSaleReport()
        {

            _phoneList = GetUsers();

            foreach (string theUser in _phoneList)
            {
                SprintWholesaleUser u = new SprintWholesaleUser();
                u.UserNumber = theUser;
                _users.Add(u);
            }

            // get the voice, sms and data usage for the users
            Hashtable sms =  this.GetSMSUsage();
            Hashtable data = GetDataUsage(  );
            Hashtable voice = GetVoiceUsage( );

            // generate a temporary report for now            
            foreach ( SprintWholesaleUser u in _users )
            { 
                            // get the sms data
                //StringBuilder theMsg = new StringBuilder("UsageFor::" + u.UserNumber);
                //theMsg.Append(" \t\t**SMS::" + sms[u.UserNumber]);
                //theMsg.Append(" \t\t**Data::" + data[u.UserNumber]);
                //theMsg.Append(" \t\t**Voice::" + voice[u.UserNumber] + "\r\n");
                StringBuilder theMsg = new StringBuilder(u.UserNumber + "," + sms[u.UserNumber] + "," + data[u.UserNumber] + "," + voice[u.UserNumber] );
                // for now
                LogFileMgr.Instance.WriteToFile(@"d:/apps/logs/SprintMayUsage.txt", theMsg.ToString() ); 
            }
        }

        // working methods 
        private List<string> GetUsers()
        {
            List<string> _phoneList = _db.GetUsers();
            return _phoneList;
        }

        private Hashtable GetSMSUsage()
        {
            Hashtable sms = _db.GetSMSUsage();
            return sms;
        }

        private Hashtable GetVoiceUsage()
        {

            Hashtable voiceUsage = _db.GetVoiceUsage();

            return voiceUsage;

        }

        private Hashtable GetDataUsage()
        {

            Hashtable dataUsage = _db.GetDataUsage();

            return dataUsage;
        }


    }
}
