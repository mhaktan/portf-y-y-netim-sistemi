using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.Customers.Dto
{
    public class PagedCustomerResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? PortfolioManagerId { get; set; }
        public string FullName { get; set; }
        public string CustomerCode { get; set; }
        public int? CustomerType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? RiskProfile { get; set; }
        public decimal? TotalAum { get; set; }
        public DateTime? OnboardingDate { get; set; }
        public decimal? TotalAumFrom { get; set; }
        public decimal? TotalAumTo { get; set; }
        public DateTime? OnboardingDateFrom { get; set; }
        public DateTime? OnboardingDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CustomerTypeIn { get; set; }
        public int? CustomerTypeNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string RiskProfileIn { get; set; }
        public int? RiskProfileNot { get; set; }
    }
}
