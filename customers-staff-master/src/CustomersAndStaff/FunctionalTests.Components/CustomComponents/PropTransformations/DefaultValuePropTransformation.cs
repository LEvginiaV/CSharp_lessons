using Kontur.Selone.Properties;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations
{
    public class DefaultValuePropTransformation : IPropTransformation<string>
    {
        public DefaultValuePropTransformation(string defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public string Deserialize(string value)
        {
            return value == defaultValue ? null : value;
        }

        public string Serialize(string value)
        {
            return value ?? defaultValue;
        }

        private readonly string defaultValue;
    }
}