using System;
using System.Collections.Generic;

using PortfoyYonetimSistemi.Approvals.Dto;

namespace PortfoyYonetimSistemi.PerformanceReports.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class PerformanceReportReportDto
    {
        public PerformanceReportDto Data { get; set; }

        public List<ApprovalRecordDto> ApprovalHistory { get; set; }
    }
}
