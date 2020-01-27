using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.Workers
{
    public class WorkerDto : WorkerInfoDto
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
    }
}