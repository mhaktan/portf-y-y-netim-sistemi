using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    // State Machine: status — Assigned → InProgress → PendingApproval → Completed → Cancelled
    // Initial: Assigned | Transitions: Assigned→InProgress[Start], InProgress→PendingApproval[Submit], PendingApproval→Completed[Approve], PendingApproval→InProgress[Revise], *→Cancelled[Cancel]
    [Table("WorkAssignments")]
    public class WorkAssignment : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public WorkAssignmentAssignmentType AssignmentType { get; set; }

        public WorkAssignmentPriority Priority { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public WorkAssignmentStatus Status { get; set; }

        [MaxLength(2000)]
        public string CompletionNote { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long PortfolioManagerId { get; set; }

        [ForeignKey(nameof(PortfolioManagerId))]
        public virtual PortfolioManager PortfolioManager { get; set; }

    }
}