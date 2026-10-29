using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    [Table("PortfolioManagers")]
    public class PortfolioManager : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeCode { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public long? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public long BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public virtual ICollection<CustomerManagerTransfer> CustomerManagerTransfers { get; set; }

        public virtual ICollection<ManagerTarget> ManagerTargets { get; set; }

        public virtual ICollection<WorkAssignment> WorkAssignments { get; set; }

        public virtual ICollection<PerformanceReport> PerformanceReports { get; set; }

    }
}