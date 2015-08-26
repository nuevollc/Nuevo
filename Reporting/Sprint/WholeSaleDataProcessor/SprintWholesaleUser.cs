using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruMobility.Reporting.Sprint
{
    class SprintWholesaleUser
    {

        private string rNumber;
        private int smsUsage=0;
        private int voiceUsage=0;
        private int dataUsage=0;

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

        }//SMSUsage
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
