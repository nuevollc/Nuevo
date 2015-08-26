using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruMobility.Services.Fraud
{
    public class AMSData
    {
        private int dataIn;
        private int dataOut;
        private string user;

        public string User
        {
            get
            {
                return this.user;
            }
            set
            {
                user = value;
            }
        }

        public int DataIn
        {
            get
            {
                return this.dataIn;
            }
            set
            {
                dataIn = value;
            }
        }

        public int DataOut
        {
            get
            {
                return this.dataOut;
            }
            set
            {
                dataOut = value;
            }
        }
    }
}
