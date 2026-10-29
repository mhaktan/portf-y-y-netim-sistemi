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
using PortfoyYonetimSistemi.PortfolioManagers.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.Customers.Dto;
using PortfoyYonetimSistemi.CustomerManagerTransfers.Dto;
using PortfoyYonetimSistemi.ManagerTargets.Dto;
using PortfoyYonetimSistemi.WorkAssignments.Dto;
using PortfoyYonetimSistemi.PerformanceReports.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.PortfolioManagers
{
    [Abp.Authorization.AbpAuthorize]
    public class PortfolioManagerAppService : AsyncCrudAppService<
        PortfolioManager,
        PortfolioManagerDto,
        long,
        PagedPortfolioManagerResultRequestDto,
        CreatePortfolioManagerDto,
        PortfolioManagerDto>,
        IPortfolioManagerAppService
    {
        private readonly IFlowEngine _flowEngine;

        public PortfolioManagerAppService(IRepository<PortfolioManager, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.PortfolioManager_Read;
            GetAllPermissionName = PermissionNames.PortfolioManager_Read;
            CreatePermissionName = PermissionNames.PortfolioManager_Create;
            UpdatePermissionName = PermissionNames.PortfolioManager_Update;
            DeletePermissionName = PermissionNames.PortfolioManager_Delete;
        }

        protected override IQueryable<PortfolioManager> CreateFilteredQuery(PagedPortfolioManagerResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.User, x => x.Branch)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.FullName != null && x.FullName.Contains(input.Keyword)) ||
                    (x.EmployeeCode != null && x.EmployeeCode.Contains(input.Keyword)) ||
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.Phone != null && x.Phone.Contains(input.Keyword)) ||
                    (x.Email != null && x.Email.Contains(input.Keyword)))
                .WhereIf(!input.FullName.IsNullOrWhiteSpace(), x => x.FullName != null && x.FullName.Contains(input.FullName))
                .WhereIf(!input.EmployeeCode.IsNullOrWhiteSpace(), x => x.EmployeeCode != null && x.EmployeeCode.Contains(input.EmployeeCode))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.Phone.IsNullOrWhiteSpace(), x => x.Phone != null && x.Phone.Contains(input.Phone))
                .WhereIf(!input.Email.IsNullOrWhiteSpace(), x => x.Email != null && x.Email.Contains(input.Email))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.BranchId.HasValue, x => x.BranchId == input.BranchId.Value);
        }

        public override async Task<PortfolioManagerDto> CreateAsync(CreatePortfolioManagerDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "PortfolioManager", result);
            return result;
        }

        public override async Task<PortfolioManagerDto> UpdateAsync(PortfolioManagerDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "PortfolioManager", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "PortfolioManager", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.PortfolioManager_Read)]
        public List<GroupCountDto> GetGroupedCount(PortfolioManagerGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "UserId", "BranchId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "UserId":
                    return query
                        .GroupBy(x => new { Key = x.UserId, Label = x.User == null ? null : x.User.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                case "BranchId":
                    return query
                        .GroupBy(x => new { Key = x.BranchId, Label = x.Branch == null ? null : x.Branch.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.PortfolioManager_Read)]
        public async Task<PortfolioManagerReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.Customers)
                .Include(x => x.CustomerManagerTransfers)
                .Include(x => x.ManagerTargets)
                .Include(x => x.WorkAssignments)
                .Include(x => x.PerformanceReports)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new PortfolioManagerReportDto
            {
                Data = ObjectMapper.Map<PortfolioManagerDto>(root),
                Customers = ObjectMapper.Map<List<CustomerDto>>(
                    root.Customers == null ? new List<Customer>() : root.Customers.ToList()),
                CustomerManagerTransfers = ObjectMapper.Map<List<CustomerManagerTransferDto>>(
                    root.CustomerManagerTransfers == null ? new List<CustomerManagerTransfer>() : root.CustomerManagerTransfers.ToList()),
                ManagerTargets = ObjectMapper.Map<List<ManagerTargetDto>>(
                    root.ManagerTargets == null ? new List<ManagerTarget>() : root.ManagerTargets.ToList()),
                WorkAssignments = ObjectMapper.Map<List<WorkAssignmentDto>>(
                    root.WorkAssignments == null ? new List<WorkAssignment>() : root.WorkAssignments.ToList()),
                PerformanceReports = ObjectMapper.Map<List<PerformanceReportDto>>(
                    root.PerformanceReports == null ? new List<PerformanceReport>() : root.PerformanceReports.ToList()),
            };
        }

    }
}
