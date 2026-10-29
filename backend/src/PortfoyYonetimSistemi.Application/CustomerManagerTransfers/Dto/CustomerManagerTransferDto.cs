using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.CustomerManagerTransfers.Dto
{
    [AutoMapFrom(typeof(Entities.CustomerManagerTransfer))]
    public class CustomerManagerTransferDto : EntityDto<long>
    {
        public DateTime TransferDate { get; set; }

        public string TransferReason { get; set; }

        public string Notes { get; set; }

        public long CustomerId { get; set; }

        public string CustomerDisplay { get; set; }

        public long PortfolioManagerId { get; set; }

        public string PortfolioManagerDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}