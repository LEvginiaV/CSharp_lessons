using System;

namespace Market.CustomersAndStaff.FunctionalTests.Configuration
{
    public interface IMarketApiSettings
    {
        string ApiKey { get; }
        Uri[] Urls { get; }
    }
}