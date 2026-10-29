using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.Customers.Dto
{
    [AutoMapTo(typeof(Entities.Customer))]
    public class CreateCustomerDto
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomerCode { get; set; }

        public int CustomerType { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public int? RiskProfile { get; set; }

        public decimal? TotalAum { get; set; }

        public DateTime? OnboardingDate { get; set; }

        public long PortfolioManagerId { get; set; }

    }
}