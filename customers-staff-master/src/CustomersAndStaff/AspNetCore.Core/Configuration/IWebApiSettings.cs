using System;

namespace Market.CustomersAndStaff.AspNetCore.Core.Configuration
{
    public interface IWebApiSettings
    {
        bool IsDevelopment { get; }
        int Port { get; }
        TimeSpan StopTimeout { get; }
        string Prefix { get; }
    }
}