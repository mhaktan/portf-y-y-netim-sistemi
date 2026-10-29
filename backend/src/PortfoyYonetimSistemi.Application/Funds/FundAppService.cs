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
using PortfoyYonetimSistemi.Funds.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;
using PortfoyYonetimSistemi.MarketDatas.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Funds
{
    [Abp.Authorization.AbpAuthorize]
    public class FundAppService : AsyncCrudAppService<
        Fund,
        FundDto,
        long,
        PagedFundResultRequestDto,
        CreateFundDto,
        FundDto>,
        IFundAppService
    {
        private readonly IFlowEngine _flowEngine;

        public FundAppService(IRepository<Fund, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Fund_Read;
            GetAllPermissionName = PermissionNames.Fund_Read;
            CreatePermissionName = PermissionNames.Fund_Create;
            UpdatePermissionName = PermissionNames.Fund_Update;
            DeletePermissionName = PermissionNames.Fund_Delete;
        }

        protected override IQueryable<Fund> CreateFilteredQuery(PagedFundResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.FundCode != null && x.FundCode.Contains(input.Keyword)) ||
                    (x.FundName != null && x.FundName.Contains(input.Keyword)) ||
                    (x.BenchmarkIndex != null && x.BenchmarkIndex.Contains(input.Keyword)))
                .WhereIf(!input.FundCode.IsNullOrWhiteSpace(), x => x.FundCode != null && x.FundCode.Contains(input.FundCode))
                .WhereIf(!input.FundName.IsNullOrWhiteSpace(), x => x.FundName != null && x.FundName.Contains(input.FundName))
                .WhereIf(!input.BenchmarkIndex.IsNullOrWhiteSpace(), x => x.BenchmarkIndex != null && x.BenchmarkIndex.Contains(input.BenchmarkIndex))
                .WhereIf(input.FundType.HasValue, x => x.FundType == (FundFundType)input.FundType.Value)
                .WhereIf(input.Currency.HasValue, x => x.Currency == (FundCurrency)input.Currency.Value)
                .WhereIf(input.ManagementFeeRate.HasValue, x => x.ManagementFeeRate == input.ManagementFeeRate.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value)
                .WhereIf(input.ManagementFeeRateFrom.HasValue, x => x.ManagementFeeRate >= input.ManagementFeeRateFrom.Value)
                .WhereIf(input.ManagementFeeRateTo.HasValue, x => x.ManagementFeeRate <= input.ManagementFeeRateTo.Value)
                .WhereIf(!input.FundTypeIn.IsNullOrWhiteSpace(), x => input.FundTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (FundFundType?)int.Parse(v.Trim()))
                    .Contains(x.FundType))
                .WhereIf(input.FundTypeNot.HasValue, x => x.FundType != (FundFundType)input.FundTypeNot.Value)
                .WhereIf(!input.CurrencyIn.IsNullOrWhiteSpace(), x => input.CurrencyIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (FundCurrency?)int.Parse(v.Trim()))
                    .Contains(x.Currency))
                .WhereIf(input.CurrencyNot.HasValue, x => x.Currency != (FundCurrency)input.CurrencyNot.Value);
        }

        public override async Task<FundDto> CreateAsync(CreateFundDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Fund", result);
            return result;
        }

        public override async Task<FundDto> UpdateAsync(FundDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Fund", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Fund", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Fund_Read)]
        public List<GroupCountDto> GetGroupedCount(FundGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "FundType", "Currency" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "FundType":
                    return query
                        .GroupBy(x => x.FundType)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "Currency":
                    return query
                        .GroupBy(x => x.Currency)
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.Fund_Read)]
        public decimal? GetStats(FundStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new string[0];
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "ManagementFeeRate" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "ManagementFeeRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ManagementFeeRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ManagementFeeRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ManagementFeeRate)
                            : query.Average(x => (decimal?)x.ManagementFeeRate);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Fund_Read)]
        public async Task<FundReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.CustomerFundHoldings)
                .Include(x => x.MarketDatas)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new FundReportDto
            {
                Data = ObjectMapper.Map<FundDto>(root),
                CustomerFundHoldings = ObjectMapper.Map<List<CustomerFundHoldingDto>>(
                    root.CustomerFundHoldings == null ? new List<CustomerFundHolding>() : root.CustomerFundHoldings.ToList()),
                MarketDatas = ObjectMapper.Map<List<MarketDataDto>>(
                    root.MarketDatas == null ? new List<MarketData>() : root.MarketDatas.ToList()),
            };
        }

    }
}
