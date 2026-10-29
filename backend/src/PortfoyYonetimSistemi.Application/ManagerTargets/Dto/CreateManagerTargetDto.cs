using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.ManagerTargets.Dto
{
    [AutoMapTo(typeof(Entities.ManagerTarget))]
    public class CreateManagerTargetDto
    {
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public int TargetType { get; set; }

        public decimal TargetValue { get; set; }

        public decimal? ActualValue { get; set; }

        [MaxLength(50)]
        public string Unit { get; set; }

        public decimal? AchievementRate { get; set; }

        public int Status { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long TargetPeriodId { get; set; }

        public long PortfolioManagerId { get; set; }

    }
}