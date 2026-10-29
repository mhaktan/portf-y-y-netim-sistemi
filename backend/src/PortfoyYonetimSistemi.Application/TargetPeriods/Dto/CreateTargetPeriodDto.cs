using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.TargetPeriods.Dto
{
    [AutoMapTo(typeof(Entities.TargetPeriod))]
    public class CreateTargetPeriodDto
    {
        [Required]
        [MaxLength(100)]
        public string PeriodName { get; set; }

        public int PeriodType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Year { get; set; }

        public bool IsActive { get; set; }

    }
}