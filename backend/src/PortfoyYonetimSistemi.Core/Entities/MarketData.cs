using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    [Table("MarketDatas")]
    public class MarketData : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string InstrumentName { get; set; }

        [Required]
        [MaxLength(50)]
        public string InstrumentCode { get; set; }

        public MarketDataInstrumentType InstrumentType { get; set; }

        public DateTime DataDate { get; set; }

        public decimal? OpenPrice { get; set; }

        public decimal ClosePrice { get; set; }

        public decimal? HighPrice { get; set; }

        public decimal? LowPrice { get; set; }

        public decimal? Volume { get; set; }

        public decimal? NavPerUnit { get; set; }

        public decimal? DailyReturnRate { get; set; }

        public long FundId { get; set; }

        [ForeignKey(nameof(FundId))]
        public virtual Fund Fund { get; set; }

    }
}