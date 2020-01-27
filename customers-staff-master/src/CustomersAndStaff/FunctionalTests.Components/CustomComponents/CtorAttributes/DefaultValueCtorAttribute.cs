using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes
{
    public class DefaultValueCtorAttribute : AdditionalConstructorArgsAttribute
    {
        public DefaultValueCtorAttribute(string defaultValue)
        {
            Args = new object[]
                {
                    new DefaultValuePropTransformation(defaultValue), 
                };
        }
    }
}