using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using Strata8.Wireless.Db;


namespace EPCS.Ciber.Tax
{
    public class CiberTaxMgr
    {
        
        private char zero_pad = Convert.ToChar("0");

        public CiberTaxMgr() { }
            
        public string GetTaxAmount( string tollChargeAmount, CarrierInfo ci )
        {
            string taxAmount = String.Empty;
            if ( ci.TaxRate.Equals( 0 ) )
            {               
                return taxAmount.PadLeft(10, zero_pad);
            } 
            else
            {
                taxAmount = this.CalculateTax( tollChargeAmount, ci.TaxRate );

            }

            return taxAmount.PadLeft(10, zero_pad);

        }

        public string GetTaxAmount(string tollChargeAmount, string airChargeAmount, CarrierInfo ci)
        {
            string taxAmount = String.Empty;
            if (ci.TaxRate.Equals(0))
            {
                return taxAmount.PadLeft(10, zero_pad);
            }
            else
            {
                taxAmount = this.CalculateTax(tollChargeAmount, airChargeAmount, ci.TaxRate);

            }

            return taxAmount.PadLeft(10, zero_pad);

        }

        /// <summary>
        /// Private method that calculates the charge amount.  
        /// The total time is in the MMMMSS format, see code below.
        /// The seconds are taken and used to round up to the nearest minute.
        /// The 
        /// </summary>
        /// <param name="totalTime"></param>
        /// <param name="chargeRate"></param>
        /// <returns></returns>
        private string CalculateTax(string tollChargeAmount, string airChargeAmount, double taxRate)
        {
            string dolr;
            string cents;
            char zero_pad = Convert.ToChar("0");

            // toll and air charge amount fields are in the following format
            // $$$$$$$$cc
            // get the toll charge amount
            string tempValueToll = tollChargeAmount.Substring(0, 8) + "." + tollChargeAmount.Substring(8, 2);
            double dollarsCentsToll = Convert.ToDouble( tempValueToll );

            // get the air charge amount
            string tempValueAir = airChargeAmount.Substring(0, 8) + "." + airChargeAmount.Substring(8, 2);
            double dollarsCentsAir = Convert.ToDouble(tempValueAir); 

            double act = Math.Round( (dollarsCentsToll + dollarsCentsAir) * taxRate, 2);
            string actString = act.ToString();

            int dotIndex = actString.IndexOf(".");
            if (dotIndex == -1)
            {
                dolr = actString;
                cents = "00";
            }
            else
            {
                dolr = actString.Substring(0, actString.IndexOf(".")).PadLeft(8, zero_pad);
                int i = actString.IndexOf(".") + 1;
                cents = actString.Substring(i, actString.Length - i).PadRight(2, zero_pad);
            }

            string d = dolr + cents;
            return d;

        }
     
        /// <summary>
        /// Private method that calculates the charge amount.  
        /// The total time is in the MMMMSS format, see code below.
        /// The seconds are taken and used to round up to the nearest minute.
        /// The 
        /// </summary>
        /// <param name="totalTime"></param>
        /// <param name="chargeRate"></param>
        /// <returns></returns>
        private string CalculateTax( string tollChargeAmount, double taxRate )
        {
            string dolr;
            string cents;

            // toll charge amount field is in the following format
            // $$$$$$$$cc
            char zero_pad = Convert.ToChar("0");
            if (tollChargeAmount.Equals("0000000000"))
                return "0000000000";

            string tempValue = tollChargeAmount.Substring(0, 8) + "." + tollChargeAmount.Substring(8, 2);
            double dollarsCents = Convert.ToDouble( tempValue );
             

            // add the extra minute before calculating the rate

            double act = Math.Round( dollarsCents * taxRate, 2 );
            string actString = act.ToString();

            int dotIndex = actString.IndexOf(".");
            if (dotIndex == -1)
            {
                dolr = actString;
                cents = "00";
            }
            else
            {
                dolr = actString.Substring(0, actString.IndexOf(".")).PadLeft(8, zero_pad);                
                int i = actString.IndexOf(".") + 1;
                cents = actString.Substring(i, actString.Length - i).PadRight(2, zero_pad);
            }

            string d = dolr + cents;
            return d;

        }

    }
}
