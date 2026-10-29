using System;
using System.Collections.Generic;
using PortfoyYonetimSistemi.PortfolioManagers.Dto;

namespace PortfoyYonetimSistemi.Branchs.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class BranchReportDto
    {
        public BranchDto Data { get; set; }
        public List<PortfolioManagerDto> PortfolioManagers { get; set; }
    }
}
