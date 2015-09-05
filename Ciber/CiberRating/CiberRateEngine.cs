using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata8.Wireless.Utils; 
using Strata8.Wireless.Db; 

namespace Strata8.Wireless.Ciber.Rating
{

    /// <summary>
    /// class used to calculate the charge amount
    /// the rates, AirTime, Toll and International are configurable parameteres read at 
    /// the time of object instantiation.
    /// </summary>
    public class CiberRateEngine
    {

        // configurable rates used to rate CDRs for CIBER records
        // we have SPRINT, VZW and USCellular customers on either land or vessel based systems.
        private CiberRate _sprintRates = new CiberRate();
        private CiberRate _vzwRates = new CiberRate();
        private CiberRate _landBasedRates = new CiberRate();
        private double _year1StepDownAirTimeRate = 0;
        private double _year2StepDownAirTimeRate = 0;
        private double _year3StepDownAirTimeRate = 0;

        // our logger
        private FileWriter cfw = FileWriter.Instance;

        public CiberRateEngine()
        {
            try
            {
                // VERIZON
                _vzwRates.AirTimeRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["VZWAirTimeRate"]);
                _vzwRates.USCanadaTollRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["VZWTollRate"]);
                _vzwRates.InternationalRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["VZWInternationalRate"]);

                // SPRINT
                _sprintRates.AirTimeRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["SprintAirTimeRate"]);
                _sprintRates.USCanadaTollRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["SprintTollRate"]);
                _sprintRates.InternationalRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["SprintInternationalRate"]);

                //LAND
                _landBasedRates.AirTimeRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["LandAirTimeRate"]);
                _landBasedRates.USCanadaTollRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["LandTollRate"]);
                _landBasedRates.InternationalRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["LandInternationalRate"]);

                // support 3-tier step down rate
                _year1StepDownAirTimeRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["VZWLandAirTimeRate1"]);
                _year2StepDownAirTimeRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["VZWLandAirTimeRate2"]);
                _year3StepDownAirTimeRate = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["VZWLandAirTimeRate3"]);
            }
            catch (SystemException e)
            {
                cfw.WriteToLogFile("CiberRater::CiberRater():ECaught:" + e.Message + "\r\n" + e.StackTrace);
            }

        }

        public string CalculateInternationalTollChargeAmount( string totalChargeableTime, CarrierInfo ci )
        {
            string chargeAmount = String.Empty;
            double internationalRate = 0;

            if (ci.ServingCarrierType.Equals(Cdr.Rating.ServiceProviderType.Land))
            {
                internationalRate = _landBasedRates.InternationalRate;
            }
            else
            {
                // if not land based, then we check if Sprint, use VZW default 
                if (ci.HomeCarrier.Equals(Cdr.Rating.ServiceProvider.Sprint))
                    internationalRate = _sprintRates.InternationalRate;
                else
                    internationalRate = _vzwRates.InternationalRate;
            }

            chargeAmount = this.CalculateChargeAmount(totalChargeableTime, internationalRate);
            return chargeAmount;
        }

        public string CalculateLocalLdTollChargeAmount( string totalChargeableTime, CarrierInfo ci ) 
        {
            double tollRate = 0;
            string chargeAmount = String.Empty;

            if (ci.ServingCarrierType.Equals(Cdr.Rating.ServiceProviderType.Land))
            {
                tollRate = _landBasedRates.USCanadaTollRate;
            }
            else
            {
                // if not land based, then we check if Sprint, if not we use VZW default rates
                if (ci.HomeCarrier.Equals(Cdr.Rating.ServiceProvider.Sprint))
                    tollRate = _sprintRates.USCanadaTollRate;
                else
                    tollRate = _vzwRates.USCanadaTollRate;
            }
            chargeAmount = this.CalculateChargeAmount(totalChargeableTime, tollRate );
            return chargeAmount;
        }

        public string CalculateAirTimeChargeAmount( string totalChargeableTime, CarrierInfo ci ) 
        {
            double airTimeRate = 0;

            string chargeAmount = String.Empty;
            // get the carrier rate
            if ( ci.ServingCarrierType.Equals(Cdr.Rating.ServiceProviderType.Land) )
            {
                // if  land based, then we check if VZW to apply the step down airtime rates
                if (ci.HomeCarrier.Equals(Cdr.Rating.ServiceProvider.Verizon))
                {
                    // get the appropriate step down rate
                    if (ci.VZWLandAirTimeRateYear.Equals(Convert.ToInt32( Cdr.Rating.VZWLandAirTimeRate.Year1) ) )
                        airTimeRate = _year1StepDownAirTimeRate;
                    else if (ci.VZWLandAirTimeRateYear.Equals(Convert.ToInt32(Cdr.Rating.VZWLandAirTimeRate.Year2)))
                        airTimeRate = _year2StepDownAirTimeRate;
                    else if (ci.VZWLandAirTimeRateYear.Equals(Convert.ToInt32(Cdr.Rating.VZWLandAirTimeRate.Year3)))
                        airTimeRate = _year3StepDownAirTimeRate;
                    else
                    {
                        cfw.WriteToLogFile("CiberRater::CalculateAirTimeChargeAmount():AirTimeRateNotFoundFor: " + ci.ServingCarrierSidBid);
                        airTimeRate = _year1StepDownAirTimeRate;
                    }
                }
                else
                {
                    // apply the normal airtime land rates for both USCell and Sprint users
                    airTimeRate = _landBasedRates.AirTimeRate;
                }

            }
            else
            {
                // if not land based, then we check if Sprint
                if (ci.HomeCarrier.Equals(Cdr.Rating.ServiceProvider.Sprint))
                    airTimeRate = _sprintRates.AirTimeRate;
                else
                    airTimeRate = _vzwRates.AirTimeRate;
            }

            chargeAmount = this.CalculateChargeAmount(totalChargeableTime, airTimeRate);
            return chargeAmount;

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
        private string CalculateChargeAmount( string totalTime, double chargeRate )
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
