using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.Branchs.Dto;

namespace PortfoyYonetimSistemi.Branchs
{
    public interface IBranchAppService : IAsyncCrudAppService<
        BranchDto,
        long,
        PagedBranchResultRequestDto,
        CreateBranchDto,
        BranchDto>
    {
        Task<BranchReportDto> GetReportData(long id);
    }
}
