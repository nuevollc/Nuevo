using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Did
{
    public class NpaNxxData
    {
        private string npanxx = String.Empty;
        private string lata = String.Empty;
        private string city = String.Empty;
        private string state = String.Empty;

        public NpaNxxData()
        { }

        /// <summary>
        /// ctor that takes a string array to load the object
        /// </summary>
        /// <param name="d"></param>
        public NpaNxxData(string[] d)
        {
                    
             char[] sep = new char[] { '"' };

            if (d.Length.Equals(6))
            {
                npanxx = d[0].Trim(sep);
                lata = d[3].Trim(sep);
                city = d[4].Trim(sep);
                state = d[5].Trim(sep);
            }

        }// ctor


        // accessors
        public string NpaNxx
        {
            get
            {
                return this.npanxx;
            }
            set
            {
                npanxx = value;
            }
        }

        public string Lata
        {
            get
            {
                return this.lata;
            }
            set
            {
                lata = value;
            }
        }
        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                city = value;
            }
        }
        public string State
        {
            get
            {
                return this.state;
            }
            set
            {
                state = value;
            }
        }

    }
}
