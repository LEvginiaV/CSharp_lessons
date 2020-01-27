using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Calendar;

namespace Market.CustomersAndStaff.Tests.Core.Builders
{
    public class WorkerCalendarDayBuilder<T> where T : BaseCalendarRecord
    {
        public static WorkerCalendarDayBuilder<T> Create(Guid workerId, DateTime date)
        {
            return new WorkerCalendarDayBuilder<T>(workerId, date);
        }

        private WorkerCalendarDayBuilder(Guid workerId, DateTime date)
        {
            workerCalendarDay = new WorkerCalendarDay<T>
                {
                    WorkerId = workerId,
                    Date = date,
                    Records = new T[0],
                };
        }

        public WorkerCalendarDayBuilder<T> AddRecord(T record)
        {
            workerCalendarDay.Records = workerCalendarDay.Records.Append(record).ToArray();
            return this;
        }

        public WorkerCalendarDay<T> Build()
        {
            return workerCalendarDay;
        }

        private readonly WorkerCalendarDay<T> workerCalendarDay;
    }
}