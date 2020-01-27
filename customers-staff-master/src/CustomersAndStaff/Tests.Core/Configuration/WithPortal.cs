using System.Reflection;

using GroboContainer.NUnitExtensions;
using GroboContainer.NUnitExtensions.Impl.TestContext;

using Market.CustomersAndStaff.Portal.Core;

namespace Market.CustomersAndStaff.Tests.Core.Configuration
{
    [WithApplicationSettings]
    public class WithPortal : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            suiteContext.Container.ConfigurePortal();
        }
    }
}