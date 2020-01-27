using System.Reflection;

using Alko.Configuration.Serilog;
using Alko.Configuration.Settings;

using GroboContainer.NUnitExtensions;
using GroboContainer.NUnitExtensions.Impl.TestContext;

using Serilog;

namespace Market.CustomersAndStaff.Tests.Core.Configuration
{
    [WithApplicationSettings]
    public class WithSerilog : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            var settings = new BaseServiceSettings(suiteContext.Container.Get<IApplicationSettings>());
            SerilogConfigurator.ConfigureDefault(settings.LogDirectory);
            suiteContext.Container.Configurator.ForAbstraction<ILogger>().UseInstances(Log.Logger);
        }
    }
}