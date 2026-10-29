using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;

namespace PortfoyYonetimSistemi.CustomerFundHoldings
{
    public interface ICustomerFundHoldingAppService : IAsyncCrudAppService<
        CustomerFundHoldingDto,
        long,
        PagedCustomerFundHoldingResultRequestDto,
        CreateCustomerFundHoldingDto,
        CustomerFundHoldingDto>
    {
        List<GroupCountDto> GetGroupedCount(CustomerFundHoldingGroupedCountInput input);
        decimal? GetStats(CustomerFundHoldingStatsInput input);
    }
}
