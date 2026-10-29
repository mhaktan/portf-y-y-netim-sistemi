using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.StateMachine.Dto;
using PortfoyYonetimSistemi.PerformanceReports.Dto;

namespace PortfoyYonetimSistemi.PerformanceReports
{
    public interface IPerformanceReportAppService : IAsyncCrudAppService<
        PerformanceReportDto,
        long,
        PagedPerformanceReportResultRequestDto,
        CreatePerformanceReportDto,
        PerformanceReportDto>
    {
        Task<PerformanceReportDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<PerformanceReportDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(PerformanceReportGroupedCountInput input);
        decimal? GetStats(PerformanceReportStatsInput input);
        Task<PerformanceReportReportDto> GetReportData(long id);
    }
}
