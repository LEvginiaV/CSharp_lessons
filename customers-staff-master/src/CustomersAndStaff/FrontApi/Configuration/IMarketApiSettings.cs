using System;

namespace Market.CustomersAndStaff.FrontApi.Configuration
{
    public interface IMarketApiSettings
    {
        string ApiKey { get; }
        Uri[] Urls { get; }
    }
}