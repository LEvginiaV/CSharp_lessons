using System;

using Kontur.Logging;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class ServiceApiClientSettings
    {
        public ServiceApiClientSettings(Uri[] replicas, TimeSpan timeout, ILog log)
        {
            Log = log;
            Replicas = replicas;
            Timeout = timeout;
        }
        
        public ServiceApiClientSettings(string serviceName, TimeSpan timeout, ILog log, string zone)
        {
            ServiceName = serviceName;
            Timeout = timeout;
            Log = log;
            UseLocalTopologies = false;
            Zone = zone;
        }

        public Uri[] Replicas { get; }
        public TimeSpan Timeout { get; }
        public ILog Log { get; }
        public string ServiceName { get; set; }
        public bool UseLocalTopologies { get; set; }
        public string Zone { get; set; }
    }
}