using System.Linq;
using System.Reflection;

using GroboContainer.NUnitExtensions;
using GroboContainer.NUnitExtensions.Impl.TestContext;

using Kontur.Market.FakeMarkerApi.ConfigurationClient;

using Market.Api.Client;
using Market.CustomersAndStaff.Tests.Core.Configuration;

namespace Market.CustomersAndStaff.FunctionalTests.Configuration
{
    [WithApplicationSettings]
    public class WithMarketApiClients : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            var container = suiteContext.Container;
            var marketApiSettings = container.Get<IMarketApiSettings>();
            var marketApiClient = new MarketApiClient(marketApiSettings.Urls, marketApiSettings.ApiKey);
            container.Configurator.ForAbstraction<IMarketApiClient>().UseInstances(marketApiClient);
            var fakeMarketApiClient = new FakeMarketApiClient(marketApiSettings.Urls.First());
            container.Configurator.ForAbstraction<IFakeMarketApiClient>().UseInstances(fakeMarketApiClient);
        }
    }
}