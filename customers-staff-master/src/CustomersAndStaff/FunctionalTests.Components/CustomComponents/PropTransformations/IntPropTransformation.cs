using Kontur.Selone.Properties;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations
{
    public class IntPropTransformation : IPropTransformation<int?>
    {
        public int? Deserialize(string value)
        {
            return value == "" ? (int?)null : int.Parse(value);
        }

        public string Serialize(int? value)
        {
            return value?.ToString() ?? "";
        }
    }
}