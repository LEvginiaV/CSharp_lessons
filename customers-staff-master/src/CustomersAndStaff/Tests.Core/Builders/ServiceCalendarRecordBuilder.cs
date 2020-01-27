using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;

namespace Market.CustomersAndStaff.Tests.Core.Builders
{
    public class ServiceCalendarRecordBuilder
    {
        public static ServiceCalendarRecordBuilder Create(Guid recordId, TimePeriod period)
        {
            return new ServiceCalendarRecordBuilder(recordId, period);
        }

        private ServiceCalendarRecordBuilder(Guid recordId, TimePeriod period)
        {
            record = new ServiceCalendarRecord
                {
                    Id = recordId,
                    Period = period,
                    CustomerStatus = CustomerStatus.Active,
                    RecordStatus = RecordStatus.Active,
                    ProductIds = new Guid[0],
                    CustomerId = null,
                    Comment = "",
                };
        }

        public ServiceCalendarRecordBuilder WithCustomerStatus(CustomerStatus customerStatus)
        {
            record.CustomerStatus = customerStatus;
            return this;
        }

        public ServiceCalendarRecordBuilder WithRecordStatus(RecordStatus recordStatus)
        {
            record.RecordStatus = recordStatus;
            return this;
        }

        public ServiceCalendarRecordBuilder AddProductId(Guid productId)
        {
            record.ProductIds = record.ProductIds.Append(productId).ToArray();
            return this;
        }

        public ServiceCalendarRecordBuilder WithCustomerId(Guid customerId)
        {
            record.CustomerId = customerId;
            return this;
        }

        public ServiceCalendarRecordBuilder WithComment(string comment)
        {
            record.Comment = comment;
            return this;
        }

        public ServiceCalendarRecord Build()
        {
            return record;
        }

        private readonly ServiceCalendarRecord record;
    }
}