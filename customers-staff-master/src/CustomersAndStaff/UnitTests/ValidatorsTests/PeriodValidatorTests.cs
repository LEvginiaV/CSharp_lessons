using System;
using System.Collections.Generic;

using FluentAssertions;
using FluentAssertions.Extensions;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.ModelValidators.Periods;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.ValidatorsTests
{
    public class PeriodValidatorTests
    {
        [TestCaseSource(nameof(GenerateValidData))]
        public void ValidTests(DateTime date, BaseCalendarRecord[] records)
        {
            periodValidator.Validate(new WorkerCalendarDay<BaseCalendarRecord> {Date = date, Records = records}).IsSuccess.Should().BeTrue();
        }

        [TestCaseSource(nameof(GenerateInvalidData))]
        public void InvalidTests(DateTime date, BaseCalendarRecord[] records)
        {
            periodValidator.Validate(new WorkerCalendarDay<BaseCalendarRecord> {Date = date, Records = records}).IsSuccess.Should().BeFalse();
        }
        
        private static BaseCalendarRecord GetRecord(TimeSpan from, TimeSpan to)
        {
            return new BaseCalendarRecord
                {
                    Period = new TimePeriod(from, to)
                };
        }

        public static IEnumerable<TestCaseData> GenerateValidData()
        {
            var month = DateTime.UtcNow.Date;
            month = month.AddDays(-month.Day + 1);
            yield return new TestCaseData(month.AddDays(15), new[] {GetRecord(3.Hours(), 5.Hours())}) {TestName = "Simple"};

            yield return new TestCaseData(month, new[] {GetRecord(3.Hours(), 5.Hours()), GetRecord(5.Hours(), 7.Hours())}) {TestName = "Consequent"};
            yield return new TestCaseData(month, new[] {GetRecord(0.Hours(), 24.Hours())}) {TestName = "Borders"};
            yield return new TestCaseData(month, new BaseCalendarRecord[0]) {TestName = "Empty"};
        }

        public static IEnumerable<TestCaseData> GenerateInvalidData()
        {
            var month = DateTime.UtcNow.Date;
            month = month.AddDays(-month.Day + 1);
            yield return new TestCaseData(month, new[] {GetRecord(5.Hours(), 3.Hours())}) {TestName = "StartTime > EndTime"};
            yield return new TestCaseData(month, new[] {GetRecord(4.Hours(), 4.Hours())}) {TestName = "StartTime == EndTime" };
            yield return new TestCaseData(month, new[] {GetRecord(-1.Seconds(), 3.Hours())}) {TestName = "StartTime < 00:00:00" };
            yield return new TestCaseData(month, new[] {GetRecord(5.Hours(), 24.Hours() + 1.Seconds())}) {TestName = "EndTime > 24:00:00" };
            yield return new TestCaseData(month, new[] {GetRecord(3.Hours(), 5.Hours()), GetRecord(4.Hours(), 6.Hours())}) {TestName = "Intersection" };
            yield return new TestCaseData(month + 4.Hours(), new[] {GetRecord(3.Hours(), 5.Hours())}) {TestName = "date with hours" };
        }

        private readonly IPeriodValidator<BaseCalendarRecord> periodValidator = new PeriodValidator<BaseCalendarRecord>();
    }
}