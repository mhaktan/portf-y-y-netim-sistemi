using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.TargetPeriods.Dto
{
    [AutoMapFrom(typeof(Entities.TargetPeriod))]
    public class TargetPeriodDto : EntityDto<long>
    {
        public string PeriodName { get; set; }

        public int PeriodType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Year { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}