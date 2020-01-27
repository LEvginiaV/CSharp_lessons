using System.Reflection;

using GroboContainer.NUnitExtensions;
using GroboContainer.NUnitExtensions.Impl.TestContext;

using Market.CustomersAndStaff.Repositories.Configuration;

namespace Market.CustomersAndStaff.Tests.Core.Configuration
{
    [WithGraphite, WithSerilog]
    public class WithRepositories : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            suiteContext.Container.ConfigureRepositories();
        }
    }
}