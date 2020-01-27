using System;
using System.Globalization;
using System.Linq;
using System.Text;

using Kontur.Selone.Properties;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations
{
    public class MoneyPropTransformation : IPropTransformation<decimal?>
    {
        public MoneyPropTransformation(string defaultValue, bool useRubleSign)
        {
            this.defaultValue = defaultValue;
            this.useRubleSign = useRubleSign;
        }

        public decimal? Deserialize(string value)
        {
            if(string.IsNullOrEmpty(value))
                return null;

            if(useRubleSign)
            {
                value = value.Replace(rubleSign, "");
            }
            value = value.Replace(thinSpace, "");
            value = value.Replace(" ", "");
            value = value.Replace(minusSign, "-");
            return decimal.Parse(value);
        }

        public string Serialize(decimal? value)
        {
            if(value == null)
                return defaultValue;

            var addMinus = value < 0;
            value = Math.Abs(value.Value);
            var valueString = value.Value.ToString("F2", CultureInfo.GetCultureInfo("ru-RU"));

            if(value >= 1000)
            {
                var sb = new StringBuilder();
                for(var i = 0; i < valueString.Length; i++)
                {
                    if(i % 3 == 0 && i >= 6) sb.Append(thinSpace);
                    sb.Append(valueString[valueString.Length - i - 1]);
                }

                valueString = string.Join("", sb.ToString().Reverse());
            }

            if(addMinus)
            {
                valueString = minusSign + valueString;
            }
            
            if(useRubleSign)
            {
                valueString += " ";
                valueString += rubleSign;
            }

            return valueString;
        }

        private readonly string defaultValue;
        private readonly bool useRubleSign;

        private const string rubleSign = "₽";
        private const string thinSpace = "\u2009";
        private const string minusSign = "\u2212";
    }
}