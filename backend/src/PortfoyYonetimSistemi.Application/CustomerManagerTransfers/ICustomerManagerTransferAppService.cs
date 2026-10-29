using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.CustomerManagerTransfers.Dto;

namespace PortfoyYonetimSistemi.CustomerManagerTransfers
{
    public interface ICustomerManagerTransferAppService : IAsyncCrudAppService<
        CustomerManagerTransferDto,
        long,
        PagedCustomerManagerTransferResultRequestDto,
        CreateCustomerManagerTransferDto,
        CustomerManagerTransferDto>
    {
        List<GroupCountDto> GetGroupedCount(CustomerManagerTransferGroupedCountInput input);
    }
}
