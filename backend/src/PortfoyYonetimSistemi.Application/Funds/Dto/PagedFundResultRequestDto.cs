using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.Funds.Dto
{
    public class PagedFundResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string FundCode { get; set; }
        public string FundName { get; set; }
        public int? FundType { get; set; }
        public int? Currency { get; set; }
        public string BenchmarkIndex { get; set; }
        public decimal? ManagementFeeRate { get; set; }
        public bool? IsActive { get; set; }
        public decimal? ManagementFeeRateFrom { get; set; }
        public decimal? ManagementFeeRateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string FundTypeIn { get; set; }
        public int? FundTypeNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string CurrencyIn { get; set; }
        public int? CurrencyNot { get; set; }
    }
}
