using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.TimePeriodHelpers;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.TimePeriodTests
{
    public class ExtensionTests
    {
        [TestCaseSource(nameof(RoundPeriodByHourCases))]
        public void RoundPeriodByHourTest(TimePeriod[] periods, TimePeriod[] expectedPeriods)
        {
            periods.RoundPeriodsByHours().Should().BeEquivalentTo(expectedPeriods);
        }

        public static IEnumerable<TestCaseData> RoundPeriodByHourCases()
        {
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(13, 00, 00), new TimeSpan(23, 00, 00))})
                {
                    TestName = "Simple"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 00, 01), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(13, 00, 00), new TimeSpan(23, 00, 00))})
                {
                    TestName = "Left border +1"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 00, 00), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(12, 00, 00), new TimeSpan(23, 00, 00))})
                {
                    TestName = "Left border 0"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(11, 59, 59), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(12, 00, 00), new TimeSpan(23, 00, 00))})
                {
                    TestName = "Left border -1"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 00, 01))},
                new[] {new TimePeriod(new TimeSpan(13, 00, 00), new TimeSpan(23, 00, 00))})
                {
                    TestName = "Right border +1"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 00, 00))},
                new[] {new TimePeriod(new TimeSpan(13, 00, 00), new TimeSpan(23, 00, 00))})
                {
                    TestName = "Right border 0"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(22, 59, 59))},
                new[] {new TimePeriod(new TimeSpan(13, 00, 00), new TimeSpan(22, 00, 00))})
                {
                    TestName = "Right border -1"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 30, 00), new TimeSpan(23, 30, 00))},
                new[] {new TimePeriod(new TimeSpan(13, 00, 00), new TimeSpan(23, 00, 00))})
                {
                    TestName = "30 minutes"
                };
            yield return new TestCaseData(
                new[]
                    {
                        new TimePeriod(new TimeSpan(01, 30, 00), new TimeSpan(06, 30, 00)),
                        new TimePeriod(new TimeSpan(06, 30, 00), new TimeSpan(12, 30, 00)),
                    },
                new[]
                    {
                        new TimePeriod(new TimeSpan(02, 00, 00), new TimeSpan(06, 00, 00)),
                        new TimePeriod(new TimeSpan(07, 00, 00), new TimeSpan(12, 00, 00)),
                    })
                {
                    TestName = "Consequent periods"
                };
        }

        [TestCaseSource(nameof(PeriodSubtractionCases))]
        public void PeriodSubtractionTest(TimePeriod[] minuend, TimePeriod[] subtrahend, TimePeriod[] difference)
        {
            minuend.SubtractPeriods(subtrahend).Should().BeEquivalentTo(difference);
        }

        public static IEnumerable<TestCaseData> PeriodSubtractionCases()
        {
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(16, 46, 22), new TimeSpan(19, 22, 05))},
                new[]
                    {
                        new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(16, 46, 22)),
                        new TimePeriod(new TimeSpan(19, 22, 05), new TimeSpan(23, 45, 09)),
                    })
                {
                    TestName = "Subtract from middle"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(19, 22, 05))},
                new[] {new TimePeriod(new TimeSpan(19, 22, 05), new TimeSpan(23, 45, 09))})
                {
                    TestName = "Subtract from left"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(16, 46, 22), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(16, 46, 22))})
                {
                    TestName = "Subtract from right"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(05, 20, 41), new TimeSpan(19, 22, 05))},
                new[] {new TimePeriod(new TimeSpan(19, 22, 05), new TimeSpan(23, 45, 09))})
                {
                    TestName = "Intersect with left"
                };
            yield return new TestCaseData(
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 45, 09))},
                new[] {new TimePeriod(new TimeSpan(16, 46, 22), new TimeSpan(23, 55, 47))},
                new[] {new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(16, 46, 22))})
                {
                    TestName = "Intersect with right"
                };
            yield return new TestCaseData(
                new[]
                    {
                        new TimePeriod(new TimeSpan(03, 18, 54), new TimeSpan(08, 22, 41)),
                        new TimePeriod(new TimeSpan(12, 34, 56), new TimeSpan(23, 45, 09)),
                    },
                new[] {new TimePeriod(new TimeSpan(05, 20, 41), new TimeSpan(19, 22, 05))},
                new[]
                    {
                        new TimePeriod(new TimeSpan(03, 18, 54), new TimeSpan(05, 20, 41)),
                        new TimePeriod(new TimeSpan(19, 22, 05), new TimeSpan(23, 45, 09)),
                    })
                {
                    TestName = "Subtract from two periods"
                };
            yield return new TestCaseData(
                Enumerable.Range(0, 48).Select(x => new TimePeriod(TimeSpan.FromMinutes(5 + x * 30), TimeSpan.FromMinutes(25 + x * 30))).ToArray(),
                Enumerable.Range(0, 49).Select(x => new TimePeriod(TimeSpan.FromMinutes(-10 + x * 30), TimeSpan.FromMinutes(10 + x * 30))).ToArray(),
                Enumerable.Range(0, 48).Select(x => new TimePeriod(TimeSpan.FromMinutes(10 + x * 30), TimeSpan.FromMinutes(20 + x * 30))).ToArray())
                {
                    TestName = "Many periods"
                };
        }
    }
}