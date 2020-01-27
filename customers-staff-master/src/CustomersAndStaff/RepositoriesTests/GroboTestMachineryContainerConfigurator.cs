using GroboContainer.Impl;

using Market.CustomersAndStaff.GroboContainer.Core;

using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(2)]

namespace Market.CustomersAndStaff.RepositoriesTests
{
    public static class GroboTestMachineryContainerConfigurator
    {
        public static ContainerConfiguration GetContainerConfiguration(string testSuiteName)
        {
            return new ContainerConfiguration(AssembliesLoader.Load());
        }
    }
}