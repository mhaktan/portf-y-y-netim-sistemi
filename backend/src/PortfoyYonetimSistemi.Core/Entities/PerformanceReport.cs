using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    // State Machine: status — Draft → PendingApproval → Approved → Revision → Cancelled
    // Initial: Draft | Transitions: Draft→PendingApproval[Submit], PendingApproval→Approved[Approve], PendingApproval→Revision[Revise], Revision→PendingApproval[Resubmit], *→Cancelled[Cancel]
    [Table("PerformanceReports")]
    public class PerformanceReport : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string ReportName { get; set; }

        public PerformanceReportReportType ReportType { get; set; }

        public DateTime ReportDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal? TotalAum { get; set; }

        public decimal? AumChange { get; set; }

        public decimal? AumChangeRate { get; set; }

        public decimal? PortfolioReturnRate { get; set; }

        public decimal? BenchmarkReturnRate { get; set; }

        public decimal? AlphaValue { get; set; }

        public int? CustomerCount { get; set; }

        public int? NewCustomerCount { get; set; }

        public decimal? TargetAchievementRate { get; set; }

        [MaxLength(3000)]
        public string Summary { get; set; }

        public PerformanceReportStatus Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long PortfolioManagerId { get; set; }

        [ForeignKey(nameof(PortfolioManagerId))]
        public virtual PortfolioManager PortfolioManager { get; set; }

        public long TargetPeriodId { get; set; }

        [ForeignKey(nameof(TargetPeriodId))]
        public virtual TargetPeriod TargetPeriod { get; set; }

    }
}