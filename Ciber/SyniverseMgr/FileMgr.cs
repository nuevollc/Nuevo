using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Strata8.Wireless.Utils;

namespace EPCS.SyniverseMgr.Db
{
    public class FileMgr
    {
        private DbMgr dbMgr = new DbMgr();

        public List<SidBidInfo> ParseTheFile(string fileName)
        {
            // make the array , let size grow as we add data to it
            System.Collections.ArrayList theData = new System.Collections.ArrayList();
            List<SidBidInfo> sidBidList = new List<SidBidInfo>();

            char[] sep = new char[] { ',' };
            char[] trim = new char[] { ' ' };
            int lineNumber = 1;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {

                            // parse the line
                            string[] data = line.Split(sep);
                            if (line.Length < 0)
                            {
                                // we have a non-data line -- header or footer... so skip it
                                continue;
                            }
                            else
                            {
                                SidBidInfo sb = new SidBidInfo();
                                sb.ServingCarrierSidBid = line.Substring(0, 5);
                                sb.HomeCarrierSidBid = line.Substring(5, 5);
                                sb.SequenceNumber = line.Substring(10, 3);

                                //CheckSequenceNumber(sb);
                                AddSequenceNumber(sb);

                            }
   

                            lineNumber++;
                        }
                        catch (System.Exception ex)
                        {
                            string errorMsg = "Error in File>" + fileName + " Line>" + lineNumber;
                            if (line != null)
                            {// add the line information if available
                                errorMsg += "Line>" + line;
                            }
                            FileWriter.Instance.WriteToLogFile(errorMsg + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                        }
                    }
                }
            }// try
            catch (Exception e)
            {
                FileWriter.Instance.WriteToLogFile(e.Message + "\r\n" + e.StackTrace);
            }// catch

            return sidBidList;

        }// private void ParseTheFile()

        private void CheckSequenceNumber(SidBidInfo sb)
        {
            char zero_pad = Convert.ToChar("0");
            int seqnum = dbMgr.GetSequenceNumber( sb.HomeCarrierSidBid, sb.ServingCarrierSidBid);
            string sn = seqnum.ToString().PadLeft(3,zero_pad );
 
            if ( sn.Equals( sb.SequenceNumber ) )
                return;
            else
                dbMgr.UpdateSequenceNumber( sb.HomeCarrierSidBid, sb.ServingCarrierSidBid, sb.SequenceNumber );
            return;


        }

        private void AddSequenceNumber(SidBidInfo sb)
        {
            dbMgr.AddSequenceNumber(sb.HomeCarrierSidBid, sb.ServingCarrierSidBid, sb.SequenceNumber);

        }

    }
}
