using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.MarketDatas.Dto;

namespace PortfoyYonetimSistemi.MarketDatas
{
    public interface IMarketDataAppService : IAsyncCrudAppService<
        MarketDataDto,
        long,
        PagedMarketDataResultRequestDto,
        CreateMarketDataDto,
        MarketDataDto>
    {
        List<GroupCountDto> GetGroupedCount(MarketDataGroupedCountInput input);
        decimal? GetStats(MarketDataStatsInput input);
    }
}
