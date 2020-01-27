using System;
using System.Collections.Generic;

using Kontur.Clusterclient.Core.Topology;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    public class ConstantTopologyClusterProvider : IClusterProvider
    {
        public ConstantTopologyClusterProvider(Uri[] addressList)
        {
            this.addressList = addressList;
        }

        public IList<Uri> GetCluster()
        {
            return addressList;
        }

        private readonly Uri[] addressList;
    }
}