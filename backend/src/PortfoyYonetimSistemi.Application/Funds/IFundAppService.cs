using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.Funds.Dto;

namespace PortfoyYonetimSistemi.Funds
{
    public interface IFundAppService : IAsyncCrudAppService<
        FundDto,
        long,
        PagedFundResultRequestDto,
        CreateFundDto,
        FundDto>
    {
        List<GroupCountDto> GetGroupedCount(FundGroupedCountInput input);
        decimal? GetStats(FundStatsInput input);
        Task<FundReportDto> GetReportData(long id);
    }
}
