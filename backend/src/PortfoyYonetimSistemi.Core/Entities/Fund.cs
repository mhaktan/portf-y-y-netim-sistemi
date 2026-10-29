using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    [Table("Funds")]
    public class Fund : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(20)]
        public string FundCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string FundName { get; set; }

        public FundFundType FundType { get; set; }

        public FundCurrency Currency { get; set; }

        [MaxLength(100)]
        public string BenchmarkIndex { get; set; }

        public decimal? ManagementFeeRate { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<CustomerFundHolding> CustomerFundHoldings { get; set; }

        public virtual ICollection<MarketData> MarketDatas { get; set; }

    }
}