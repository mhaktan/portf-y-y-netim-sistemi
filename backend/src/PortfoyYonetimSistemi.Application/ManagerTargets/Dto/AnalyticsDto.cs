using System;
using PortfoyYonetimSistemi.Analytics.Dto;

namespace PortfoyYonetimSistemi.ManagerTargets.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class ManagerTargetGroupedCountInput : PagedManagerTargetResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class ManagerTargetStatsInput : PagedManagerTargetResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
