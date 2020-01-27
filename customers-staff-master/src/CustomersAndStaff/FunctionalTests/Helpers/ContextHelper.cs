using System;
using System.Collections.Concurrent;

using Market.Api.Models.Shops;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers
{
    public static class ContextHelper
    {
        public static Guid GetCurrentShopId()
        {
            return shopDictionary[GetTestId()].Id;
        }

        public static Guid GetCurrentOrganizationId()
        {
            return shopDictionary[GetTestId()].OrganizationId;
        }

        public static void SetCurrentShop(Shop shop)
        {
            shopDictionary[GetTestId()] = shop;
        }

        public static void Clear()
        {
            shopDictionary.TryRemove(GetTestId(), out _);
        }

        private static string GetTestId()
        {
            return TestContext.CurrentContext.Test.ID;
        }

        private static readonly ConcurrentDictionary<string, Shop> shopDictionary = new ConcurrentDictionary<string, Shop>();
    }
}