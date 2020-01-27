using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes
{
    public class PercentCtorAttribute : AdditionalConstructorArgsAttribute
    {
        public PercentCtorAttribute(bool usePercent = false, string defaultValue = "")
        {
            Args = new object[]
                {
                    new PercentPropTransformation(usePercent, defaultValue),
                };
        }
    }
}