using System.Reflection;

using Alko.Graphite.Core;

using GroboContainer.NUnitExtensions;
using GroboContainer.NUnitExtensions.Impl.TestContext;

using SkbKontur.Graphite.Client;

namespace Market.CustomersAndStaff.Tests.Core.Configuration
{
    [WithApplicationSettings]
    public class WithGraphite : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            suiteContext.Container.Configurator.ForAbstraction<IStatsDClient>().UseInstances(suiteContext.Container.Get<StatsDClientFactory>().Create());
        }
    }
}