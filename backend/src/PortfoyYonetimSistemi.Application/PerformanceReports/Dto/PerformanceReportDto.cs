using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.PerformanceReports.Dto
{
    [AutoMapFrom(typeof(Entities.PerformanceReport))]
    public class PerformanceReportDto : EntityDto<long>
    {
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

        public string Summary { get; set; }

        public int Status { get; set; }

        public string RevisionNote { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long PortfolioManagerId { get; set; }

        public string PortfolioManagerDisplay { get; set; }

        public long TargetPeriodId { get; set; }

        public string TargetPeriodDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}