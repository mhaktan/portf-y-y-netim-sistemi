using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    [Table("CustomerManagerTransfers")]
    public class CustomerManagerTransfer : FullAuditedEntity<long>
    {
        public DateTime TransferDate { get; set; }

        [MaxLength(500)]
        public string TransferReason { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public long CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        public long PortfolioManagerId { get; set; }

        [ForeignKey(nameof(PortfolioManagerId))]
        public virtual PortfolioManager PortfolioManager { get; set; }

    }
}