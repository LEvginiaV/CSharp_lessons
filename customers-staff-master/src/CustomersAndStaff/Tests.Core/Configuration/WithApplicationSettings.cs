using System.Reflection;

using Alko.Configuration.Settings;

using GroboContainer.NUnitExtensions;
using GroboContainer.NUnitExtensions.Impl.TestContext;

namespace Market.CustomersAndStaff.Tests.Core.Configuration
{
    public class WithApplicationSettings : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            var applicationSettings = ApplicationSettings.LoadDefault("settings.csf");
            suiteContext.Container.Configurator.ForAbstraction<IApplicationSettings>().UseInstances(applicationSettings);
        }
    }
}