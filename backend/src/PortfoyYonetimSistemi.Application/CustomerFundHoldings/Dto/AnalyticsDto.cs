using System;
using PortfoyYonetimSistemi.Analytics.Dto;

namespace PortfoyYonetimSistemi.CustomerFundHoldings.Dto
{
    /// <summary>GetAll ile ayni filtreleri kabul eder, ustune GroupBy alir.</summary>
    public class CustomerFundHoldingGroupedCountInput : PagedCustomerFundHoldingResultRequestDto
    {
        public string GroupBy { get; set; }
    }

    public class CustomerFundHoldingStatsInput : PagedCustomerFundHoldingResultRequestDto
    {
        /// <summary>avg | sum | min | max | avgDayDiff</summary>
        public string Aggregate { get; set; }
        public string Field { get; set; }
        public string FromField { get; set; }
        public string ToField { get; set; }
    }
}
