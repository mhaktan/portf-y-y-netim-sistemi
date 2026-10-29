using System;
using System.Collections.Generic;
using PortfoyYonetimSistemi.CustomerManagerTransfers.Dto;
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;

namespace PortfoyYonetimSistemi.Customers.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class CustomerReportDto
    {
        public CustomerDto Data { get; set; }
        public List<CustomerManagerTransferDto> CustomerManagerTransfers { get; set; }
        public List<CustomerFundHoldingDto> CustomerFundHoldings { get; set; }
    }
}
