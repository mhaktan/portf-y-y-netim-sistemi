using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.Funds.Dto
{
    [AutoMapFrom(typeof(Entities.Fund))]
    public class FundDto : EntityDto<long>
    {
        public string FundCode { get; set; }

        public string FundName { get; set; }

        public int FundType { get; set; }

        public int Currency { get; set; }

        public string BenchmarkIndex { get; set; }

        public decimal? ManagementFeeRate { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}