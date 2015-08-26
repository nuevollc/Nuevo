using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strata8.Wireless.Data;
using Strata8.Wireless.Cdr.Rating;

namespace Strata8.Wireless.Db
{
    /// <summary>
    /// class that holds the carrier information
    /// for the homeCarrier it holds the home carrier name (Sprint, VZW, USCellular)  
    /// and the SIDBID of the Home Carrier.
    /// for the serving carrier it holds the serving carrier type: land, vessel 
    /// and the serving carrier.
    /// </summary>
    public class CarrierInfo
    {

        // home carrier SIDBID; users home carrier
        private ServiceProvider _homeCarrier;
        private string _sidBidHomeCarrier = TechDataEnums.SIDBID_NOT_FOUND.ToString();

        // to determine whether or not we are land-based or on a ship
        // serving carrier is on-waves or the hosting service provider
        private ServiceProviderType _servingCarrierType;
        private ServiceProvider _servingCarrier;
        private string _sidBidServingCarrier = TechDataEnums.SIDBID_NOT_FOUND.ToString();
        private double _taxRate = 0;
        private int _vzwLandAirTimeRateYear = 0;

        public CarrierInfo(ServiceProvider homeCarrier, string homeCarrierSidBid)
        {
            HomeCarrierSidBid = homeCarrierSidBid;
            HomeCarrier = homeCarrier;
            ServingCarrierSidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
        }

        public CarrierInfo(ServiceProvider homeCarrier)
        {
            HomeCarrier = homeCarrier;
            HomeCarrierSidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
            ServingCarrierSidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
        }

        public CarrierInfo(string sidBid)
        {
            HomeCarrierSidBid = sidBid;
            ServingCarrierSidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
        }

        public CarrierInfo() 
        {
            HomeCarrierSidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
            ServingCarrierSidBid = TechDataEnums.SIDBID_NOT_FOUND.ToString();
        }

        /// <summary>
        /// assesors
        /// </summary>
        public ServiceProvider HomeCarrier
        {
            get
            {
                return this._homeCarrier;
            }
            set
            {
                _homeCarrier = value;

            }
        }//carrier

        public ServiceProvider ServingCarrier
        {
            get
            {
                return this._servingCarrier;
            }
            set
            {
                _servingCarrier = value;

            }
        }//carrier

        public string HomeCarrierSidBid
        {
            get
            {
                return this._sidBidHomeCarrier;
            }
            set
            {
                _sidBidHomeCarrier = value;

            }
        }//_sidBid

        public string ServingCarrierSidBid
        {
            get
            {
                return this._sidBidServingCarrier;
            }
            set
            {
                _sidBidServingCarrier = value;

            }
        }//_sidBid
 
        public ServiceProviderType ServingCarrierType
        {
            get
            {
                return this._servingCarrierType;
            }
            set
            {
                _servingCarrierType = value;

            }
        }//_servingCarrierType

        public double TaxRate
        {
            get
            {
                return this._taxRate;
            }
            set
            {
                _taxRate = value;

            }
        }//tax rate


        public int VZWLandAirTimeRateYear
        {
            get
            {
                return this._vzwLandAirTimeRateYear;
            }
            set
            {
                _vzwLandAirTimeRateYear = value;

            }
        }//tax rate
    }//class

}//namespace
