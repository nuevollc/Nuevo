using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata8.Wireless.Ciber.Rating
{
    public class CiberRate
    {
        private double _airTimeRate;
        private double _usCanadaTollRate;
        private double _internationalRate;

        public double AirTimeRate
        {
            get
            {
                return this._airTimeRate;
            }
            set
            {
                _airTimeRate = value;

            }
        }//_airTimeRate
        public double USCanadaTollRate
        {
            get
            {
                return this._usCanadaTollRate;
            }
            set
            {
                _usCanadaTollRate = value;

            }
        }//_airTimeRate

        public double InternationalRate
        {
            get
            {
                return this._internationalRate;
            }
            set
            {
                _internationalRate = value;

            }
        }//_internationalRate

    }
}
