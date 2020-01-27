using System;
using System.Linq;

namespace Market.CustomersAndStaff.Tests.Core
{
    public static class RandomStringGenerator
    {
        public static string GenerateRandomCyrillic(int length)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            return new string(Enumerable.Range(0, length).Select(x => (char)('а' + random.Next(32))).ToArray());
        }

        public static string GenerateRandomLatin(int length)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            return new string(Enumerable.Range(0, length).Select(x => (char)('a' + random.Next(24))).ToArray());
        }
    }
}