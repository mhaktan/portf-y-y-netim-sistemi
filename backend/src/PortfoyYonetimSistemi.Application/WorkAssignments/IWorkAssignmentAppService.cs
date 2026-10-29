using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.StateMachine.Dto;
using PortfoyYonetimSistemi.WorkAssignments.Dto;

namespace PortfoyYonetimSistemi.WorkAssignments
{
    public interface IWorkAssignmentAppService : IAsyncCrudAppService<
        WorkAssignmentDto,
        long,
        PagedWorkAssignmentResultRequestDto,
        CreateWorkAssignmentDto,
        WorkAssignmentDto>
    {
        Task<WorkAssignmentDto> ChangeStatusAsync(long id, ChangeStatusInput input);

        // Yalniz ApprovalAppService cagirir — REST'e acik degil
        [Abp.Application.Services.RemoteService(false)]
        Task<WorkAssignmentDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input);
        List<GroupCountDto> GetGroupedCount(WorkAssignmentGroupedCountInput input);
        decimal? GetStats(WorkAssignmentStatsInput input);
        Task<WorkAssignmentReportDto> GetReportData(long id);
    }
}
