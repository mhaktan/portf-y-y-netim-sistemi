using System;
using System.Collections.Generic;
using PortfoyYonetimSistemi.Customers.Dto;
using PortfoyYonetimSistemi.CustomerManagerTransfers.Dto;
using PortfoyYonetimSistemi.ManagerTargets.Dto;
using PortfoyYonetimSistemi.WorkAssignments.Dto;
using PortfoyYonetimSistemi.PerformanceReports.Dto;

namespace PortfoyYonetimSistemi.PortfolioManagers.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class PortfolioManagerReportDto
    {
        public PortfolioManagerDto Data { get; set; }
        public List<CustomerDto> Customers { get; set; }
        public List<CustomerManagerTransferDto> CustomerManagerTransfers { get; set; }
        public List<ManagerTargetDto> ManagerTargets { get; set; }
        public List<WorkAssignmentDto> WorkAssignments { get; set; }
        public List<PerformanceReportDto> PerformanceReports { get; set; }
    }
}
