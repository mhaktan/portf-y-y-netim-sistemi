using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    [Table("TargetPeriods")]
    public class TargetPeriod : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(100)]
        public string PeriodName { get; set; }

        public TargetPeriodPeriodType PeriodType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Year { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<ManagerTarget> ManagerTargets { get; set; }

        public virtual ICollection<PerformanceReport> PerformanceReports { get; set; }

    }
}