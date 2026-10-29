using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.MarketDatas.Dto
{
    [AutoMapTo(typeof(Entities.MarketData))]
    public class CreateMarketDataDto
    {
        [Required]
        [MaxLength(200)]
        public string InstrumentName { get; set; }

        [Required]
        [MaxLength(50)]
        public string InstrumentCode { get; set; }

        public int InstrumentType { get; set; }

        public DateTime DataDate { get; set; }

        public decimal? OpenPrice { get; set; }

        public decimal ClosePrice { get; set; }

        public decimal? HighPrice { get; set; }

        public decimal? LowPrice { get; set; }

        public decimal? Volume { get; set; }

        public decimal? NavPerUnit { get; set; }

        public decimal? DailyReturnRate { get; set; }

        public long FundId { get; set; }

    }
}