using System;
using System.Collections.Generic;
using PortfoyYonetimSistemi.ManagerTargets.Dto;
using PortfoyYonetimSistemi.PerformanceReports.Dto;

namespace PortfoyYonetimSistemi.TargetPeriods.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class TargetPeriodReportDto
    {
        public TargetPeriodDto Data { get; set; }
        public List<ManagerTargetDto> ManagerTargets { get; set; }
        public List<PerformanceReportDto> PerformanceReports { get; set; }
    }
}
