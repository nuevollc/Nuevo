using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Reporting.Sprint
{
    public class SprintWholesaleUser
    {
        private string firstName = String.Empty;
        private String lastName = String.Empty;
        private String groupId= String.Empty;
        private string rNumber = String.Empty;
        private int smsUsage=0;
        private int voiceUsage=0;
        private int dataUsage=0;


        public string FirstName
        {

            get
            {
                return this.firstName;
            }
            set
            {
                firstName = value;

            }
        }

        public string LastName
        {

            get
            {
                return this.lastName;
            }
            set
            {
                lastName = value;

            }
        }
        public string GroupId
        {

            get
            {
                return this.groupId;
            }
            set
            {
                groupId = value;

            }
        }

        public string UserNumber
        {

            get
            {
                return this.rNumber;
            }
            set
            {
                rNumber = value;

            }

        }//userNumber

        public int SMSUsage
        {

            get
            {
                return this.smsUsage;
            }
            set
            {
                smsUsage = value;

            }

        }//SMSUsage

        public int VoiceUsage
        {

            get
            {
                return this.voiceUsage;
            }
            set
            {
                voiceUsage = value;

            }

        }//voiceUsage

        public int DataUsage
        {

            get
            {
                return this.dataUsage;
            }
            set
            {
                dataUsage = value;

            }

        }//dataUsage
    }
}
