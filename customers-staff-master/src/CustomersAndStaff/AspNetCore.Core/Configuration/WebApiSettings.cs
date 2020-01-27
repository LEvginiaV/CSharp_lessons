using System;

using Alko.Configuration.Settings;

namespace Market.CustomersAndStaff.AspNetCore.Core.Configuration
{
    public class WebApiSettings : IWebApiSettings
    {
        public WebApiSettings(IApplicationSettings settings)
        {
            IsDevelopment = settings.GetBool("Api.IsDevelopment");
            Port = settings.GetInt("Api.Port");
            StopTimeout = settings.GetTimeSpan("Api.StopTimeout");
            settings.TryGetString("Api.Prefix", out var prefix);
            Prefix = prefix == null ? null : "/" + prefix;
        }

        public bool IsDevelopment { get; }
        public int Port { get; }
        public TimeSpan StopTimeout { get; }
        public string Prefix { get; }
    }
}