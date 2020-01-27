using System;
using System.ComponentModel.DataAnnotations;

using Market.CustomersAndStaff.OnlineApi.Dto.Common;
using Market.CustomersAndStaff.OnlineApi.Dto.Validations;

namespace Market.CustomersAndStaff.OnlineApi.Dto.CreateRecord
{
    public class CreateRecordDto
    {
        public DateTime Date { get; set; }
        [Required]
        [TimePeriod("01:00:00", "01:00:00")]
        public TimePeriodDto Period { get; set; }
        public Guid WorkerId { get; set; }
        public Guid ServiceId { get; set; }
        [Required]
        [StringLength(120)]
        public string CustomerName { get; set; }
        [Required]
        [RegularExpression(@"\d{11}")]
        public string CustomerPhone { get; set; }
    }
}