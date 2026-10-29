using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.CustomerFundHoldings.Dto
{
    public class PagedCustomerFundHoldingResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? CustomerId { get; set; }
        public long? FundId { get; set; }
        public decimal? Units { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CurrentValue { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? ProfitLoss { get; set; }
        public decimal? UnitsFrom { get; set; }
        public decimal? UnitsTo { get; set; }
        public decimal? PurchasePriceFrom { get; set; }
        public decimal? PurchasePriceTo { get; set; }
        public decimal? CurrentValueFrom { get; set; }
        public decimal? CurrentValueTo { get; set; }
        public DateTime? PurchaseDateFrom { get; set; }
        public DateTime? PurchaseDateTo { get; set; }
        public decimal? WeightFrom { get; set; }
        public decimal? WeightTo { get; set; }
        public decimal? ProfitLossFrom { get; set; }
        public decimal? ProfitLossTo { get; set; }
    }
}
