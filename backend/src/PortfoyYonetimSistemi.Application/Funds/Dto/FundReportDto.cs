using System;
using System.Collections.Generic;
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;
using PortfoyYonetimSistemi.MarketDatas.Dto;

namespace PortfoyYonetimSistemi.Funds.Dto
{
    /// <summary>
    /// Rapor verisi — kok kayit ve alt koleksiyonlar tek yanitta.
    /// PDF sablonu tek apiBinding kullandigi icin nested donuyoruz.
    /// </summary>
    public class FundReportDto
    {
        public FundDto Data { get; set; }
        public List<CustomerFundHoldingDto> CustomerFundHoldings { get; set; }
        public List<MarketDataDto> MarketDatas { get; set; }
    }
}
