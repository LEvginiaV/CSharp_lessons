using System;
using System.Collections.Generic;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Customers;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests
{
    public class BirthdayModelTest
    {
        [TestCase(1, 2, null, ExpectedResult = "01.02")]
        [TestCase(123, 456, null, ExpectedResult = "123.456")]
        [TestCase(01, 02, 99999, ExpectedResult = "01.02.99999")]
        public string BirthdayToStringTest(int day, int month, int? year)
        {
            return new Birthday(day, month, year).ToString();
        }

        [TestCaseSource(nameof(ValidDates))]
        public void BirtdaySuccessfulParseTest(string birthdayString, Birthday expectedBirthday)
        {
            Birthday.Parse(birthdayString).Should().Be(expectedBirthday);
        }

        [TestCase("1", TestName = "1 date part")]
        [TestCase("1.1.1.1", TestName = "3+ date parts")]
        [TestCase("1.02.2000", TestName = "Common date with shot day")]
        [TestCase("01.2.2000", TestName = "Common date with shot month")]
        [TestCase("01.02.99", TestName = "Year has 2 digits")]
        public void BirtdayFailParseTest(string birthdayString)
        {
            Assert.Throws<FormatException>(() => Birthday.Parse(birthdayString))
                  .Message.Should().Be("Expected dd.mm.yyyy or dd.mm");
        }

        public static IEnumerable<TestCaseData> ValidDates()
        {
            yield return new TestCaseData("23.02.9000", new Birthday(23, 2, 9000))
                    {TestName = "Common date"};
            yield return new TestCaseData("23.02", new Birthday(23, 2))
                    {TestName = "No year"};
        }
    }
}