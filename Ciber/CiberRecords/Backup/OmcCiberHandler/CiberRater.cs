using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata8.Wireless.Utils;

namespace Strata8.Wireless.Cdr.Ciber
{

    /// <summary>
    /// class used to calculate the charge amount
    /// the rates, AirTime, Toll and International are configurable parameteres read at 
    /// the time of object instantiation.
    /// </summary>
    public class CiberRater
    {

        // configurable rates used to rate CDRs for CIBER records
        private double m_airTimeRate;
        private double m_tollRate;
        private double m_internationalRate;

        // our logger
        private FileWriter cfw = FileWriter.Instance;

        public CiberRater()
        {
            try
            {

                m_airTimeRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["AirTimeRate"]);
                m_tollRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["TollRate"]);
                m_internationalRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["InternationalRate"]);

            }
            catch (SystemException e)
            {
                cfw.WriteToLogFile("CiberRater::CiberRater():ECaught:" + e.Message + "\r\n" + e.StackTrace);
            }

        }

        public string CalculateInternationalTollChargeAmount(string totalChargeableTime)
        {
            string chargeAmount = String.Empty;

            chargeAmount = this.CalculateChargeAmount(totalChargeableTime, m_internationalRate);
            return chargeAmount;
        }

        public string CalculateLocalLdTollChargeAmount(string totalChargeableTime )
        {
            string chargeAmount = String.Empty;

            chargeAmount = this.CalculateChargeAmount(totalChargeableTime, m_tollRate);
            return chargeAmount;
        }

        public string CalculateAirTimeChargeAmount(string totalChargeableTime)
        {
            string chargeAmount = String.Empty;

            chargeAmount = this.CalculateChargeAmount(totalChargeableTime, m_airTimeRate);
            return chargeAmount;

        }


        /// <summary>
        /// method that calculates the charge amount
        /// </summary>
        /// <param name="totalTime"></param>
        /// <param name="chargeRate"></param>
        /// <returns></returns>
        private string CalculateChargeAmount(string totalTime, double chargeRate)
        {
            int aMin = 0;
            char zero_pad = Convert.ToChar("0");
            string dollars;
            string cents = "00"; ;
            if (totalTime.Equals("000000"))
                return "000001";

            // billable time format is MMMMSS: MMMM = 0000-9999, SS = 00-59
            // grab the minutes and secs
            int cMins = Convert.ToInt32(totalTime.Substring(0, 4));
            int cSecs = Convert.ToInt32(totalTime.Substring(4, 2));

            // round up to the nearest minute
            if (cSecs > 0 )
                aMin = 1;

            // add the extra minute before calculating the rate

            double act = (aMin + cMins) * chargeRate;
            string actString = act.ToString();

            int dotIndex = actString.IndexOf(".");
            if (dotIndex == -1)
            {
                dollars = actString;
                cents = "00";
            }
            else
            {
                dollars = actString.Substring(0, actString.IndexOf("."));
                
                int i = actString.IndexOf(".") + 1;
                cents = actString.Substring(i, actString.Length - i).PadRight(2, zero_pad);
            }

            string d = dollars + cents;
            return d;

        }

    }
}
