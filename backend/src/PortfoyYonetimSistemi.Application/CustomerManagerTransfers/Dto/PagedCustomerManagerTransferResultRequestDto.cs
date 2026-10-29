using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.CustomerManagerTransfers.Dto
{
    public class PagedCustomerManagerTransferResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? CustomerId { get; set; }
        public long? PortfolioManagerId { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferReason { get; set; }
        public string Notes { get; set; }
        public DateTime? TransferDateFrom { get; set; }
        public DateTime? TransferDateTo { get; set; }
    }
}
