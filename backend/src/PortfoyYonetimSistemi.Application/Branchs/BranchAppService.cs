using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.Branchs.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.PortfolioManagers.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Branchs
{
    [Abp.Authorization.AbpAuthorize]
    public class BranchAppService : AsyncCrudAppService<
        Branch,
        BranchDto,
        long,
        PagedBranchResultRequestDto,
        CreateBranchDto,
        BranchDto>,
        IBranchAppService
    {
        private readonly IFlowEngine _flowEngine;

        public BranchAppService(IRepository<Branch, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Branch_Read;
            GetAllPermissionName = PermissionNames.Branch_Read;
            CreatePermissionName = PermissionNames.Branch_Create;
            UpdatePermissionName = PermissionNames.Branch_Update;
            DeletePermissionName = PermissionNames.Branch_Delete;
        }

        protected override IQueryable<Branch> CreateFilteredQuery(PagedBranchResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Name != null && x.Name.Contains(input.Keyword)) ||
                    (x.Code != null && x.Code.Contains(input.Keyword)) ||
                    (x.Region != null && x.Region.Contains(input.Keyword)))
                .WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name != null && x.Name.Contains(input.Name))
                .WhereIf(!input.Code.IsNullOrWhiteSpace(), x => x.Code != null && x.Code.Contains(input.Code))
                .WhereIf(!input.Region.IsNullOrWhiteSpace(), x => x.Region != null && x.Region.Contains(input.Region));
        }

        public override async Task<BranchDto> CreateAsync(CreateBranchDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Branch", result);
            return result;
        }

        public override async Task<BranchDto> UpdateAsync(BranchDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Branch", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Branch", new { Id = input.Id });
        }
        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Branch_Read)]
        public async Task<BranchReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.PortfolioManagers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new BranchReportDto
            {
                Data = ObjectMapper.Map<BranchDto>(root),
                PortfolioManagers = ObjectMapper.Map<List<PortfolioManagerDto>>(
                    root.PortfolioManagers == null ? new List<PortfolioManager>() : root.PortfolioManagers.ToList()),
            };
        }

    }
}
