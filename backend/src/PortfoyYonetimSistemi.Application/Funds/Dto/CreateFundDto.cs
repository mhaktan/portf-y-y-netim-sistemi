using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.Funds.Dto
{
    [AutoMapTo(typeof(Entities.Fund))]
    public class CreateFundDto
    {
        [Required]
        [MaxLength(20)]
        public string FundCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string FundName { get; set; }

        public int FundType { get; set; }

        public int Currency { get; set; }

        [MaxLength(100)]
        public string BenchmarkIndex { get; set; }

        public decimal? ManagementFeeRate { get; set; }

        public bool IsActive { get; set; }

    }
}