using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.CustomerFundHoldings.Dto
{
    [AutoMapFrom(typeof(Entities.CustomerFundHolding))]
    public class CustomerFundHoldingDto : EntityDto<long>
    {
        public decimal Units { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? CurrentValue { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public decimal? Weight { get; set; }

        public decimal? ProfitLoss { get; set; }

        public long CustomerId { get; set; }

        public string CustomerDisplay { get; set; }

        public long FundId { get; set; }

        public string FundDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}