using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    [Table("CustomerFundHoldings")]
    public class CustomerFundHolding : FullAuditedEntity<long>
    {
        public decimal Units { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? CurrentValue { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? Weight { get; set; }

        public decimal? ProfitLoss { get; set; }

        public long CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        public long FundId { get; set; }

        [ForeignKey(nameof(FundId))]
        public virtual Fund Fund { get; set; }

    }
}