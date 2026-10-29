using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.Customers.Dto
{
    [AutoMapFrom(typeof(Entities.Customer))]
    public class CustomerDto : EntityDto<long>
    {
        public string FullName { get; set; }

        public string CustomerCode { get; set; }

        public int CustomerType { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int? RiskProfile { get; set; }

        public decimal? TotalAum { get; set; }

        public DateTime? OnboardingDate { get; set; }

        public long PortfolioManagerId { get; set; }

        public string PortfolioManagerDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}