using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.TargetPeriods.Dto
{
    public class PagedTargetPeriodResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string PeriodName { get; set; }
        public int? PeriodType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Year { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string PeriodTypeIn { get; set; }
        public int? PeriodTypeNot { get; set; }
    }
}
