using System;

using FluentAssertions;

using Market.CustomersAndStaff.Models.WorkOrders;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.Models
{
    public class WorkOrderNumberTests
    {
        [TestCase("ВГ", 123456, 1, "ВГ", 123457)]
        [TestCase("ВГ", 123456, -1, "ВГ", 123455)]
        [TestCase("ВГ", 123456, 0, "ВГ", 123456)]
        [TestCase("ВГ", 123456, 1000000, "ВД", 123457)]
        [TestCase("ВГ", 123456, 10000000, "ВР", 123466)]
        [TestCase("ВГ", 999999, 1, "ВД", 1)]
        [TestCase("ВЕ", 999999, 1, "ВЖ", 1)]
        [TestCase("ВЖ", 999999, 1, "ВИ", 1)]
        [TestCase("ВИ", 999999, 1, "ВК", 1)]
        [TestCase("ВН", 999999, 1, "ВП", 1)]
        [TestCase("ВЦ", 999999, 1, "ВШ", 1)]
        [TestCase("ВЩ", 999999, 1, "ВЭ", 1)]
        [TestCase("ВЯ", 999999, 1, "ГА", 1)]
        [TestCase("ЕЯ", 999999, 1, "ЖА", 1)]
        [TestCase("НЯ", 999999, 1, "ПА", 1)]
        public void AdditionSubtractionTest(string series, int number, int value, string expectedSeries, int expectedNumber)
        {
            (new WorkOrderNumber(series, number) + value).Should().BeEquivalentTo(new WorkOrderNumber(expectedSeries, expectedNumber));
            (new WorkOrderNumber(expectedSeries, expectedNumber) - value).Should().BeEquivalentTo(new WorkOrderNumber(series, number));
        }

        [TestCase("ЯЯ", 999999, 1)]
        [TestCase("АА", 1, -1)]
        public void EdgeTest(string series, int number, int value)
        {
            ((Func<WorkOrderNumber>)(() => new WorkOrderNumber(series, number) + value)).Should().Throw<ArgumentException>();
        }

        [TestCase("ВГ-123456", "ВГ", 123456)]
        [TestCase("АР-000001", "АР", 1)]
        public void ParseTest(string value, string expectedSeries, int expectedNumber)
        {
            WorkOrderNumber.Parse(value).Should().BeEquivalentTo(new WorkOrderNumber(expectedSeries, expectedNumber));
        }

        [TestCase("ВГ", 123456, "ВГ-123456")]
        [TestCase("АР", 1, "АР-000001")]
        public void ToStringTest(string series, int number, string expectedValue)
        {
            new WorkOrderNumber(series, number).ToString().Should().Be(expectedValue);
        }

        [TestCase("ВГ", 123456, "ВГ", 123547)]
        [TestCase("ВГ", 999999, "ВД", 1)]
        [TestCase("ВЯ", 999999, "ГА", 1)]
        [TestCase("АА", 1, "ЯЯ", 999999)]
        public void OrderTest(string lessSeries, int lessNumber, string greaterSeries, int greaterNumber)
        {
            var less = new WorkOrderNumber(lessSeries, lessNumber);
            var greater = new WorkOrderNumber(greaterSeries, greaterNumber);
            (less < greater).Should().BeTrue();
            (greater < less).Should().BeFalse();
            (greater > less).Should().BeTrue();
            (less > greater).Should().BeFalse();
        }
    }
}