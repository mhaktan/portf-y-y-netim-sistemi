using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.TargetPeriods.Dto;

namespace PortfoyYonetimSistemi.TargetPeriods
{
    public interface ITargetPeriodAppService : IAsyncCrudAppService<
        TargetPeriodDto,
        long,
        PagedTargetPeriodResultRequestDto,
        CreateTargetPeriodDto,
        TargetPeriodDto>
    {
        List<GroupCountDto> GetGroupedCount(TargetPeriodGroupedCountInput input);
        decimal? GetStats(TargetPeriodStatsInput input);
        Task<TargetPeriodReportDto> GetReportData(long id);
    }
}
