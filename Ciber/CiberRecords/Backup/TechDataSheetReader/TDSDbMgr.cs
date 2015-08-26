using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;

using System.Data.SqlClient;
using System.Data;

namespace Strata8.Wireless.Data
{
    public class TDSDbMgr
    {
        private string m_connectionString = ConfigurationManager.AppSettings["WirelessTdsSQLConnectString"];

        /// <summary>
        /// method to get the SID/BID for the subscriber based on the subscribers MSID
        /// and using the VZW tech data sheet.
        /// </summary>
        /// <param name="msid">The MSID of the subscriber</param>
        /// <returns></returns>
        public string GetVzwSubscriberSidBid(string msid)
        {
            // make the connection
            string sidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
            string npanxx = msid.Substring(0, 6);
            string lastFour = msid.Substring(5, 4);
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT sidBid from VerizonTechData where mbiMin = '" + npanxx + "' ");
            sb.Append("AND lowLineRange <=  '" + lastFour + "' ");
            sb.Append("AND highLineRange >= '" + lastFour + "' ");

            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), connection);
                    myDaptor.Fill(ds);
                }

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                    return TechDataEnums.SIDBID_NOT_FOUND.ToString();
                else if (ds.Tables.Count == 0)
                    return TechDataEnums.SIDBID_NOT_FOUND.ToString();

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        sidBid = myRow.ItemArray[0].ToString();
                    }
                }

            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            return sidBid;

        }

        /// <summary>
        /// method to get the SID/BID for the subscriber based on the subscribers MSID
        /// and using the Sprint tech data sheet.
        /// </summary>
        /// <param name="msid">The MSID of the subscriber</param>
        /// <returns></returns>
        public string GetSprintSubscriberSidBid(string msid)
        {
            // make the connection
            string sidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
            string npanxx = msid.Substring(0, 6);
            string lastFour = msid.Substring(5, 4);
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT sidBid from SprintTechData where mbiMin = '" + npanxx + "' ");
            sb.Append("AND lowLineRange <=  '" + lastFour + "' ");
            sb.Append("AND highLineRange >= '" + lastFour + "' ");

            DataSet ds = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(m_connectionString))
                {
                    connection.Open();
                    // execute and fill
                    SqlDataAdapter myDaptor = new SqlDataAdapter(sb.ToString(), connection);
                    myDaptor.Fill(ds);
                }

                // if we have too many SID/BIDS or none at all it is not a Verizon customer
                // no need to create the CIBER record for this guy
                if (ds.Tables.Count > 1)
                    return TechDataEnums.SIDBID_NOT_FOUND.ToString();
                else if (ds.Tables.Count == 0)
                    return TechDataEnums.SIDBID_NOT_FOUND.ToString();

                // make sure there is only one SID/BID returned here
                foreach (DataTable myTable in ds.Tables)
                {
                    // only one row with the sidbid
                    foreach (DataRow myRow in myTable.Rows)
                    {
                        sidBid = myRow.ItemArray[0].ToString();
                    }
                }

            }
            catch (System.Exception ex)
            {
                // log the error, what is our logging policy?
            }

            return sidBid;

        }
    }// public class

} // namespace

