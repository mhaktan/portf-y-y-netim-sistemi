using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.CustomerManagerTransfers.Dto
{
    [AutoMapTo(typeof(Entities.CustomerManagerTransfer))]
    public class CreateCustomerManagerTransferDto
    {
        public DateTime TransferDate { get; set; }

        [MaxLength(500)]
        public string TransferReason { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        public long CustomerId { get; set; }

        public long PortfolioManagerId { get; set; }

    }
}