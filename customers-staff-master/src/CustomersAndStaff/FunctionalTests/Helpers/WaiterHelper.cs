using System;
using System.Diagnostics;

using FluentAssertions.Extensions;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers
{
    public static class WaiterHelper
    {
        public static void WaitUntil(Func<bool> func, TimeSpan? timeout = null)
        {
            timeout = timeout ?? 10.Seconds();
            var stopwatch = Stopwatch.StartNew();
            var timeouted = false;
            while(!timeouted)
            {
                timeouted = stopwatch.Elapsed > timeout;

                try
                {
                    if(func())
                    {
                        return;
                    }
                }
                catch
                {
                }
            }

            throw new TimeoutException($"Timeout after {timeouted}");
        }
    }
}