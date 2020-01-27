using GroboContainer.NUnitExtensions;

namespace Market.CustomersAndStaff.Tests.Core.Configuration
{
    [GroboTestSuite("MainSuite"), WithRepositories, WithCustomizedFixture]
    public interface IMainSuite
    {
    }
}