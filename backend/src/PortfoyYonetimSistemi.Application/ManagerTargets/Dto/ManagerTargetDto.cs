using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.ManagerTargets.Dto
{
    [AutoMapFrom(typeof(Entities.ManagerTarget))]
    public class ManagerTargetDto : EntityDto<long>
    {
        public string Description { get; set; }

        public int TargetType { get; set; }

        public decimal TargetValue { get; set; }

        public decimal? ActualValue { get; set; }

        public string Unit { get; set; }

        public decimal? AchievementRate { get; set; }

        public int Status { get; set; }

        public string Notes { get; set; }

        public string RevisionNote { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long TargetPeriodId { get; set; }

        public string TargetPeriodDisplay { get; set; }

        public long PortfolioManagerId { get; set; }

        public string PortfolioManagerDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}