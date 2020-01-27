using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes
{
    public class PhoneCtorAttribute : AdditionalConstructorArgsAttribute
    {
        public PhoneCtorAttribute(bool startWithCode = false, string defaultValue = "")
        {
            Args = new object[]
                {
                    new PhonePropTransformation(startWithCode, defaultValue),
                };
        }
    }
}