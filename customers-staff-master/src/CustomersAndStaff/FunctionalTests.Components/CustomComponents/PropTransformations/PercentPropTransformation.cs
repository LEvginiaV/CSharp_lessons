using System.Globalization;

using Kontur.Selone.Properties;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations
{
    public class PercentPropTransformation : IPropTransformation<decimal?>
    {
        public PercentPropTransformation(bool usePercent, string defaultValue)
        {
            this.usePercent = usePercent;
            this.defaultValue = defaultValue;
        }

        public decimal? Deserialize(string value)
        {
            if(usePercent)
            {
                value = value.Replace("%", "");
            }
            return value == defaultValue ? (decimal?)null : decimal.Parse(value);
        }

        public string Serialize(decimal? value)
        {
            return value == null ? defaultValue : (value.Value.ToString("", CultureInfo.GetCultureInfo("ru-RU")) + (usePercent ? "%" : ""));
        }

        private readonly bool usePercent;
        private readonly string defaultValue;
    }
}