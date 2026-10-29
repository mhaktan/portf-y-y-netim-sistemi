using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.Customers.Dto;

namespace PortfoyYonetimSistemi.Customers
{
    public interface ICustomerAppService : IAsyncCrudAppService<
        CustomerDto,
        long,
        PagedCustomerResultRequestDto,
        CreateCustomerDto,
        CustomerDto>
    {
        List<GroupCountDto> GetGroupedCount(CustomerGroupedCountInput input);
        decimal? GetStats(CustomerStatsInput input);
        Task<CustomerReportDto> GetReportData(long id);
    }
}
