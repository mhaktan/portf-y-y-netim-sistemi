using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.StateMachine.Dto;
using PortfoyYonetimSistemi.ManagerTargets.Dto;

namespace PortfoyYonetimSistemi.ManagerTargets
{
    public interface IManagerTargetAppService : IAsyncCrudAppService<
        ManagerTargetDto,
        long,
        PagedManagerTargetResultRequestDto,
        CreateManagerTargetDto,
        ManagerTargetDto>
    {
        Task<ManagerTargetDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<ManagerTargetDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(ManagerTargetGroupedCountInput input);
        decimal? GetStats(ManagerTargetStatsInput input);
        Task<ManagerTargetReportDto> GetReportData(long id);
    }
}
