using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

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
                // get the user name and group info
                SprintWholesaleUser u = GetUserInfo( theUser );
                u.UserNumber = theUser;
                _users.Add(u);
            }

            // get the voice, sms and data usage for the users
            Hashtable sms = this.GetSMSUsage();
            Hashtable data = GetDataUsage();
            Hashtable lte = this.GetLTEDataUsage();
            Hashtable voice = GetVoiceUsage();

            // generate a temporary report for now            
            foreach (SprintWholesaleUser u in _users)
            {
                double dataCharge = 0.00;
                double smsCharge = 0.00;
                double voiceCharge = 0.00;
                double lteCharge = 0.00;

                if (data[u.UserNumber] != null)
                    dataCharge = GetDataCharge((int)data[u.UserNumber]);
                if (sms[u.UserNumber] != null)
                    smsCharge = GetSMSCharge((int)sms[u.UserNumber]);
                if (voice[u.UserNumber] != null)
                    voiceCharge = GetVoiceCharge((int)voice[u.UserNumber]);
                if ( lte[u.UserNumber] != null)
                    lteCharge = GetLTEDataCharge((int)lte[u.UserNumber]);

                double total = GetTotal( dataCharge, smsCharge, voiceCharge, lteCharge );
                try
                {
                    String thePhone = "Phone # " + u.UserNumber + "\t" + u.FirstName + " " + u.LastName + "\t\t\t\t $" + total.ToString("0.00") + "\r\n";
                    String v = "Voice " + "\t\t\t\t" + voice[u.UserNumber] + "\t\t $" + voiceCharge.ToString("0.00") + "\r\n";
                    String m = "Messaging" + "\t\t\t" + sms[u.UserNumber] + "\t\t $" + smsCharge.ToString("0.00") + "\r\n";
                    String d = "Data" + "\t\t\t\t" + data[u.UserNumber] + "\t\t $" + dataCharge.ToString("0.00") + "\r\n";
                    String l = "LTE" + "\t\t\t\t" + lte[u.UserNumber] + "\t\t $" + lteCharge.ToString("0.00") + "\r\n";
                    String msg = thePhone +  v + m + d + l;
                    String csvmsg = u.UserNumber + "," + sms[u.UserNumber] + "," + data[u.UserNumber] + "," + voice[u.UserNumber] + "," + lte[u.UserNumber] + "," + smsCharge.ToString("0.00") + "," + dataCharge.ToString("0.00") + "," + voiceCharge.ToString("0.00")
                         + "," + lteCharge.ToString("0.00") + "," + total.ToString("0.00") + "," + u.GroupId;
                    // for now 
                    LogFileMgr.Instance.WriteToFile(@"C:\Users\RobertL\Documents\apps\logs\SprintJuly2015_Usage.txt", msg);
                    LogFileMgr.Instance.WriteToFile(@"C:\Users\RobertL\Documents\apps\logs\SprintJuly2015_Usage.csv", csvmsg);
                }
                catch (SystemException se)
                {
                    LogFileMgr.Instance.WriteToLogFile("WholeSaleDataProcessor::CreateWholeSaleReport():ExceptionCaught:"+se.Message);
                }
            }
        }

        // working methods 
        private List<string> GetUsers()
        {
            List<string> _phoneList = _db.GetUsers();
            return _phoneList;
        }

        private SprintWholesaleUser GetUserInfo(string user)
        {
            SprintWholesaleUser u = _db.GetUserInfo(user);
            return u;
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

        /// <summary>
        /// get 3G data usage
        /// </summary>
        /// <returns></returns>
        private Hashtable GetDataUsage()
        {

            Hashtable dataUsage = _db.GetDataUsage();

            return dataUsage;
        }

        private Hashtable GetLTEDataUsage()
        {

            Hashtable lteData = _db.GetLTEDataUsage();

            return lteData;
        }

        private double GetDataCharge(int data)
        {
            //0.02*(data/1024)
            double charge = 0.00;

            charge = (double)(0.02 * (data / 1024));
            return charge;

        }

        private double GetLTEDataCharge(int data)
        {
            //0.0000125/kb
            double charge = 0.00;

            charge = (double)(0.0000125 * data);
            return charge;

        }
       
        private double GetSMSCharge(int sms)
        {
            //0.003 * sms
            double charge = 0.00;
            charge = (double)(0.003 * sms);
            return charge;
        }
 
        private double GetVoiceCharge(int voice)
        {
            // 0.0145*voice

            double charge = 0.00;

            charge = (double)(0.0145 * voice);
            return charge;
        }

        private double GetTotal(double voice, double data, double sms, double lte)
        {

            double total = voice + data + sms + lte;
            return total;

        }

        public void CreateReportFromFile(string filename)
        {

            char[] sep = new char[] { ',' };
            char[] trim = new char[] { ' ' };
            int lineNumber = 1;

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {

                            // parse the line
                            string[] data = line.Split(sep);
                            if (data.GetLength(0) < 0)
                            {
                                // we have a non-data line -- header or footer... so skip it
                                continue;
                            }
                            else
                            {
                                // grab the update date of this file
                                string user = data[0];
                                string s = data[1];
                                string d = data[2];
                                string v = data[3];
                                string lte = data[4];

                                double dataCharge = 0.00;
                                double smsCharge = 0.00;
                                double voiceCharge = 0.00;
                                double lteCharge = 0.00;

                                if ( !d.Equals(string.Empty) )
                                    dataCharge = GetDataCharge(Convert.ToInt32(d));
                                if ( !s.Equals(string.Empty) )
                                    smsCharge = GetSMSCharge(Convert.ToInt32(s));
                                if ( !v.Equals(string.Empty) )
                                    voiceCharge = GetVoiceCharge(Convert.ToInt32(v));
                                if (!lte.Equals(string.Empty))
                                    lteCharge = GetLTEDataCharge(Convert.ToInt32(lte));

                                //double total = GetTotal(dataCharge, smsCharge, voiceCharge);
                                double total = 0.00;

                                String thePhone = "Phone # " + user + "\t\t\t\t\t\t $" + total.ToString("0.00") + "\r\n"; 

                                String vv = "Voice " + "\t\t\t\t" + v + "\t\t $" + voiceCharge.ToString("0.00") + "\r\n";                               
                                String mm = "Messaging" + "\t\t\t" + s + "\t\t $" + smsCharge.ToString("0.00") + "\r\n";
                                String dd = "Data" + "\t\t\t\t" + d + "\t\t $" + dataCharge.ToString("0.00") + "\r\n";
                                String msg = thePhone  + vv + mm + dd;

                                // for now
                                LogFileMgr.Instance.WriteToFile(@"D:\GoogleDrive\Google Drive\TruMobility\Billing\Invoices\2013\06\May2013_Usage2.txt", msg);

                            }

                            lineNumber++;
                        }
                        catch (System.Exception ex)
                        {
                            string errorMsg = "Error in File>" + filename + " Line>" + lineNumber;
                            if (line != null)
                            {// add the line information if available
                                errorMsg += "Line>" + line;
                            }
                            LogFileMgr.Instance.WriteToLogFile(errorMsg + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                        }
                    }
                }
            }// try
            catch (Exception e)
            {
                LogFileMgr.Instance.WriteToLogFile(e.Message + "\r\n" + e.StackTrace);
            }// catch

        }


    }

}
