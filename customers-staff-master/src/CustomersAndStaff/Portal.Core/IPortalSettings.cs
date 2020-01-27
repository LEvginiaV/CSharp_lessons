using System;

namespace Market.CustomersAndStaff.Portal.Core
{
    public interface IPortalSettings
    {
        string ApiKey { get; }
        Uri[] AuthUrls { get; }
        Uri[] RequisitesUrls { get; }
    }
}