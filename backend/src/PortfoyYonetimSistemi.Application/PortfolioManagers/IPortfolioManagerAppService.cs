using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.PortfolioManagers.Dto;

namespace PortfoyYonetimSistemi.PortfolioManagers
{
    public interface IPortfolioManagerAppService : IAsyncCrudAppService<
        PortfolioManagerDto,
        long,
        PagedPortfolioManagerResultRequestDto,
        CreatePortfolioManagerDto,
        PortfolioManagerDto>
    {
        List<GroupCountDto> GetGroupedCount(PortfolioManagerGroupedCountInput input);
        Task<PortfolioManagerReportDto> GetReportData(long id);
    }
}
