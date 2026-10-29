using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.MarketDatas.Dto
{
    public class PagedMarketDataResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? FundId { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentCode { get; set; }
        public int? InstrumentType { get; set; }
        public DateTime? DataDate { get; set; }
        public decimal? OpenPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public decimal? HighPrice { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? Volume { get; set; }
        public decimal? NavPerUnit { get; set; }
        public decimal? DailyReturnRate { get; set; }
        public DateTime? DataDateFrom { get; set; }
        public DateTime? DataDateTo { get; set; }
        public decimal? OpenPriceFrom { get; set; }
        public decimal? OpenPriceTo { get; set; }
        public decimal? ClosePriceFrom { get; set; }
        public decimal? ClosePriceTo { get; set; }
        public decimal? HighPriceFrom { get; set; }
        public decimal? HighPriceTo { get; set; }
        public decimal? LowPriceFrom { get; set; }
        public decimal? LowPriceTo { get; set; }
        public decimal? VolumeFrom { get; set; }
        public decimal? VolumeTo { get; set; }
        public decimal? NavPerUnitFrom { get; set; }
        public decimal? NavPerUnitTo { get; set; }
        public decimal? DailyReturnRateFrom { get; set; }
        public decimal? DailyReturnRateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string InstrumentTypeIn { get; set; }
        public int? InstrumentTypeNot { get; set; }
    }
}
