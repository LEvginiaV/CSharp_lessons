using System;

namespace Market.CustomersAndStaff.OnlineApi.Configuration
{
    public interface IMarketApiSettings
    {
        string ApiKey { get; }
        Uri[] Urls { get; }
    }
}