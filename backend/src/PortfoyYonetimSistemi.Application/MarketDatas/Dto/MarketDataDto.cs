using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.MarketDatas.Dto
{
    [AutoMapFrom(typeof(Entities.MarketData))]
    public class MarketDataDto : EntityDto<long>
    {
        public string InstrumentName { get; set; }

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

        public string FundDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}