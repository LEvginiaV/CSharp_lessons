using System;

using Kontur.Selone.Properties;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations
{
    public class DatePropTransformation : IPropTransformation<DateTime?>
    {
        public DateTime? Deserialize(string value)
        {
            return value == "" ? (DateTime?)null : DateTime.ParseExact(value, "dd.MM.yyyy", null);
        }

        public string Serialize(DateTime? value)
        {
            return value == null ? "" : value.Value.ToString("dd.MM.yyyy");
        }
    }
}