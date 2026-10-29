using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.PerformanceReports.Dto
{
    [AutoMapTo(typeof(Entities.PerformanceReport))]
    public class CreatePerformanceReportDto
    {
        [Required]
        [MaxLength(200)]
        public string ReportName { get; set; }

        public int ReportType { get; set; }

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

        public int Status { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long PortfolioManagerId { get; set; }

        public long TargetPeriodId { get; set; }

    }
}