using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace TruMobility.Reporting.CDR
{

    /// <summary>
    /// class to represent the list of subscribers and the parameters needed to generate the 
    /// call reports, only used to get initial parameters
    /// see the usercallreport for more details of the calls
    /// </summary>
    public class Subscriber
    {
        private string _phoneNumber = string.Empty;
        private string _extension = string.Empty;
        private string _groupId = string.Empty;
        private string _serviceProviderId = string.Empty;

        /// <summary>
        /// method to parse and load the users being processes
        /// configuration parameters delimited by commas
        /// </summary>
        /// <param name="userList"></param>
        private List<string> ParseList(string userList)
        {
            List<string> theList = new List<string>();
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = userList.Trim().Split(delimiter);

            foreach (string s in split)
            {
                theList.Add(s.Trim());

            }
            return theList;

        }//ParseList

        //assesors
        public string PhoneNumber
        {

            get
            {
                return this._phoneNumber;
            }
            set
            {
                this._phoneNumber = value;
            }

        } //PhoneNumber


        public string Extension
        {

            get
            {
                return this._extension;
            }
            set
            {
                this._extension = value;
            }

        } //Extension

        public string Group
        {

            get
            {
                return this._groupId;
            }
            set
            {
                this._groupId = value;
            }

        } //Group
        
        public string ServiceProvider
        {

            get
            {
                return this._serviceProviderId;
            }
            set
            {
                this._serviceProviderId = value;
            }

        } //ServiceProvider



    }
}
