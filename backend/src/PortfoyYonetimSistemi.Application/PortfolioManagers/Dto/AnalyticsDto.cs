using System;
using PortfoyYonetimSistemi.Analytics.Dto;

namespace PortfoyYonetimSistemi.PortfolioManagers.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class PortfolioManagerGroupedCountInput : PagedPortfolioManagerResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class PortfolioManagerStatsInput : PagedPortfolioManagerResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
