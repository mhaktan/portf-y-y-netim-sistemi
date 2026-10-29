using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.ManagerTargets.Dto
{
    public class PagedManagerTargetResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? TargetPeriodId { get; set; }
        public long? PortfolioManagerId { get; set; }
        public string Description { get; set; }
        public int? TargetType { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public string Unit { get; set; }
        public decimal? AchievementRate { get; set; }
        public int? Status { get; set; }
        public string Notes { get; set; }
        public string RevisionNote { get; set; }
        public decimal? TargetValueFrom { get; set; }
        public decimal? TargetValueTo { get; set; }
        public decimal? ActualValueFrom { get; set; }
        public decimal? ActualValueTo { get; set; }
        public decimal? AchievementRateFrom { get; set; }
        public decimal? AchievementRateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string TargetTypeIn { get; set; }
        public int? TargetTypeNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
