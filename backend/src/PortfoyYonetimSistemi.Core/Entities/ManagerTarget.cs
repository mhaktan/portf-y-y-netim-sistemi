using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    // State Machine: status — Draft → PendingApproval → Active → Completed → Cancelled
    // Initial: Draft | Transitions: Draft→PendingApproval[Submit], PendingApproval→Active[Approve], PendingApproval→Draft[Revise], Draft→PendingApproval[Resubmit], Active→Completed[Complete], *→Cancelled[Cancel]
    [Table("ManagerTargets")]
    public class ManagerTarget : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public ManagerTargetTargetType TargetType { get; set; }

        public decimal TargetValue { get; set; }

        public decimal? ActualValue { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; }

        public decimal? AchievementRate { get; set; }

        public ManagerTargetStatus Status { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long TargetPeriodId { get; set; }

        [ForeignKey(nameof(TargetPeriodId))]
        public virtual TargetPeriod TargetPeriod { get; set; }

        public long PortfolioManagerId { get; set; }

        [ForeignKey(nameof(PortfolioManagerId))]
        public virtual PortfolioManager PortfolioManager { get; set; }

    }
}