using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.PerformanceReports.Dto
{
    public class PagedPerformanceReportResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? PortfolioManagerId { get; set; }
        public long? TargetPeriodId { get; set; }
        public string ReportName { get; set; }
        public int? ReportType { get; set; }
        public DateTime? ReportDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TotalAum { get; set; }
        public decimal? AumChange { get; set; }
        public decimal? AumChangeRate { get; set; }
        public decimal? PortfolioReturnRate { get; set; }
        public decimal? BenchmarkReturnRate { get; set; }
        public decimal? AlphaValue { get; set; }
        public int? CustomerCount { get; set; }
        public int? NewCustomerCount { get; set; }
        public decimal? TargetAchievementRate { get; set; }
        public string Summary { get; set; }
        public int? Status { get; set; }
        public string RevisionNote { get; set; }
        public DateTime? ReportDateFrom { get; set; }
        public DateTime? ReportDateTo { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public decimal? TotalAumFrom { get; set; }
        public decimal? TotalAumTo { get; set; }
        public decimal? AumChangeFrom { get; set; }
        public decimal? AumChangeTo { get; set; }
        public decimal? AumChangeRateFrom { get; set; }
        public decimal? AumChangeRateTo { get; set; }
        public decimal? PortfolioReturnRateFrom { get; set; }
        public decimal? PortfolioReturnRateTo { get; set; }
        public decimal? BenchmarkReturnRateFrom { get; set; }
        public decimal? BenchmarkReturnRateTo { get; set; }
        public decimal? AlphaValueFrom { get; set; }
        public decimal? AlphaValueTo { get; set; }
        public int? CustomerCountFrom { get; set; }
        public int? CustomerCountTo { get; set; }
        public int? NewCustomerCountFrom { get; set; }
        public int? NewCustomerCountTo { get; set; }
        public decimal? TargetAchievementRateFrom { get; set; }
        public decimal? TargetAchievementRateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string ReportTypeIn { get; set; }
        public int? ReportTypeNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
