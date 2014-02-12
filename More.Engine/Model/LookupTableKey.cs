using System;

namespace More.Engine.Model
{
    public class LookupTableKey
    {
        public string Name { get; set; }

        public string HighValue { get; set; }

        public string LowValue { get; set; }

        public LookupTableKey()
        {
        }

        public LookupTableKey(string lowValue, string highValue)
        {
            LowValue = lowValue;
            HighValue = highValue;
        }

        public LookupTableKey(string name, string lowValue, string highValue)
        {
            Name = name;
            LowValue = lowValue;
            HighValue = highValue;
        }

        public string ToString(string varName)
        {
            double lowValue;
            double highValue;
            var lowIsNumber = Double.TryParse(LowValue, out lowValue);
            var highIsNumber = Double.TryParse(HighValue, out highValue);

            if (lowIsNumber && highIsNumber)
            {
                return string.Format("(Convert.ToDouble({0}) >= {1} && Convert.ToDouble({0}) <= {2})", varName, lowValue, highValue);
            }
            else if (lowIsNumber && HighValue == null)
            {
                return string.Format("Convert.ToDouble({0}).Equals({1})", varName, lowValue);
            }

            return string.Format("{0}.ToString().ToLower().Equals(\"{1}\".Trim().ToLower())", varName, LowValue);
        }
    }
}