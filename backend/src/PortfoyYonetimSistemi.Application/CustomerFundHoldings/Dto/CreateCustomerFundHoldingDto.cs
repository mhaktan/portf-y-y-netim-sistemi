using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.CustomerFundHoldings.Dto
{
    [AutoMapTo(typeof(Entities.CustomerFundHolding))]
    public class CreateCustomerFundHoldingDto
    {
        public decimal Units { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? CurrentValue { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? Weight { get; set; }

        public decimal? ProfitLoss { get; set; }

        public long CustomerId { get; set; }

        public long FundId { get; set; }

    }
}