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
using PortfoyYonetimSistemi.TargetPeriods.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.ManagerTargets.Dto;
using PortfoyYonetimSistemi.PerformanceReports.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.TargetPeriods
{
    [Abp.Authorization.AbpAuthorize]
    public class TargetPeriodAppService : AsyncCrudAppService<
        TargetPeriod,
        TargetPeriodDto,
        long,
        PagedTargetPeriodResultRequestDto,
        CreateTargetPeriodDto,
        TargetPeriodDto>,
        ITargetPeriodAppService
    {
        private readonly IFlowEngine _flowEngine;

        public TargetPeriodAppService(IRepository<TargetPeriod, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.TargetPeriod_Read;
            GetAllPermissionName = PermissionNames.TargetPeriod_Read;
            CreatePermissionName = PermissionNames.TargetPeriod_Create;
            UpdatePermissionName = PermissionNames.TargetPeriod_Update;
            DeletePermissionName = PermissionNames.TargetPeriod_Delete;
        }

        protected override IQueryable<TargetPeriod> CreateFilteredQuery(PagedTargetPeriodResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.PeriodName != null && x.PeriodName.Contains(input.Keyword)))
                .WhereIf(!input.PeriodName.IsNullOrWhiteSpace(), x => x.PeriodName != null && x.PeriodName.Contains(input.PeriodName))
                .WhereIf(input.PeriodType.HasValue, x => x.PeriodType == (TargetPeriodPeriodType)input.PeriodType.Value)
                .WhereIf(input.StartDate.HasValue, x => x.StartDate == input.StartDate.Value)
                .WhereIf(input.EndDate.HasValue, x => x.EndDate == input.EndDate.Value)
                .WhereIf(input.Year.HasValue, x => x.Year == input.Year.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value)
                .WhereIf(input.StartDateFrom.HasValue, x => x.StartDate >= input.StartDateFrom.Value)
                .WhereIf(input.StartDateTo.HasValue, x => x.StartDate <= input.StartDateTo.Value)
                .WhereIf(input.EndDateFrom.HasValue, x => x.EndDate >= input.EndDateFrom.Value)
                .WhereIf(input.EndDateTo.HasValue, x => x.EndDate <= input.EndDateTo.Value)
                .WhereIf(input.YearFrom.HasValue, x => x.Year >= input.YearFrom.Value)
                .WhereIf(input.YearTo.HasValue, x => x.Year <= input.YearTo.Value)
                .WhereIf(!input.PeriodTypeIn.IsNullOrWhiteSpace(), x => input.PeriodTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (TargetPeriodPeriodType?)int.Parse(v.Trim()))
                    .Contains(x.PeriodType))
                .WhereIf(input.PeriodTypeNot.HasValue, x => x.PeriodType != (TargetPeriodPeriodType)input.PeriodTypeNot.Value);
        }

        public override async Task<TargetPeriodDto> CreateAsync(CreateTargetPeriodDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "TargetPeriod", result);
            return result;
        }

        public override async Task<TargetPeriodDto> UpdateAsync(TargetPeriodDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "TargetPeriod", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "TargetPeriod", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.TargetPeriod_Read)]
        public List<GroupCountDto> GetGroupedCount(TargetPeriodGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "PeriodType" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "PeriodType":
                    return query
                        .GroupBy(x => x.PeriodType)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.TargetPeriod_Read)]
        public decimal? GetStats(TargetPeriodStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "StartDate", "EndDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "StartDate|EndDate":
                    {
                        var pairsStartDateEndDate = query
                            .Select(x => new { A = x.StartDate, B = x.EndDate })
                            .ToList();
                        if (pairsStartDateEndDate.Count == 0) return null;
                        return (decimal)pairsStartDateEndDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "EndDate|StartDate":
                    {
                        var pairsEndDateStartDate = query
                            .Select(x => new { A = x.EndDate, B = x.StartDate })
                            .ToList();
                        if (pairsEndDateStartDate.Count == 0) return null;
                        return (decimal)pairsEndDateStartDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Year" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Year": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Year)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Year)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Year)
                            : query.Average(x => (decimal?)x.Year);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.TargetPeriod_Read)]
        public async Task<TargetPeriodReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.ManagerTargets)
                .Include(x => x.PerformanceReports)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new TargetPeriodReportDto
            {
                Data = ObjectMapper.Map<TargetPeriodDto>(root),
                ManagerTargets = ObjectMapper.Map<List<ManagerTargetDto>>(
                    root.ManagerTargets == null ? new List<ManagerTarget>() : root.ManagerTargets.ToList()),
                PerformanceReports = ObjectMapper.Map<List<PerformanceReportDto>>(
                    root.PerformanceReports == null ? new List<PerformanceReport>() : root.PerformanceReports.ToList()),
            };
        }

    }
}
