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
using PortfoyYonetimSistemi.MarketDatas.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.MarketDatas
{
    [Abp.Authorization.AbpAuthorize]
    public class MarketDataAppService : AsyncCrudAppService<
        MarketData,
        MarketDataDto,
        long,
        PagedMarketDataResultRequestDto,
        CreateMarketDataDto,
        MarketDataDto>,
        IMarketDataAppService
    {
        private readonly IFlowEngine _flowEngine;

        public MarketDataAppService(IRepository<MarketData, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.MarketData_Read;
            GetAllPermissionName = PermissionNames.MarketData_Read;
            CreatePermissionName = PermissionNames.MarketData_Create;
            UpdatePermissionName = PermissionNames.MarketData_Update;
            DeletePermissionName = PermissionNames.MarketData_Delete;
        }

        protected override IQueryable<MarketData> CreateFilteredQuery(PagedMarketDataResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Fund)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.InstrumentName != null && x.InstrumentName.Contains(input.Keyword)) ||
                    (x.InstrumentCode != null && x.InstrumentCode.Contains(input.Keyword)))
                .WhereIf(!input.InstrumentName.IsNullOrWhiteSpace(), x => x.InstrumentName != null && x.InstrumentName.Contains(input.InstrumentName))
                .WhereIf(!input.InstrumentCode.IsNullOrWhiteSpace(), x => x.InstrumentCode != null && x.InstrumentCode.Contains(input.InstrumentCode))
                .WhereIf(input.InstrumentType.HasValue, x => x.InstrumentType == (MarketDataInstrumentType)input.InstrumentType.Value)
                .WhereIf(input.DataDate.HasValue, x => x.DataDate == input.DataDate.Value)
                .WhereIf(input.OpenPrice.HasValue, x => x.OpenPrice == input.OpenPrice.Value)
                .WhereIf(input.ClosePrice.HasValue, x => x.ClosePrice == input.ClosePrice.Value)
                .WhereIf(input.HighPrice.HasValue, x => x.HighPrice == input.HighPrice.Value)
                .WhereIf(input.LowPrice.HasValue, x => x.LowPrice == input.LowPrice.Value)
                .WhereIf(input.Volume.HasValue, x => x.Volume == input.Volume.Value)
                .WhereIf(input.NavPerUnit.HasValue, x => x.NavPerUnit == input.NavPerUnit.Value)
                .WhereIf(input.DailyReturnRate.HasValue, x => x.DailyReturnRate == input.DailyReturnRate.Value)
                .WhereIf(input.DataDateFrom.HasValue, x => x.DataDate >= input.DataDateFrom.Value)
                .WhereIf(input.DataDateTo.HasValue, x => x.DataDate <= input.DataDateTo.Value)
                .WhereIf(input.OpenPriceFrom.HasValue, x => x.OpenPrice >= input.OpenPriceFrom.Value)
                .WhereIf(input.OpenPriceTo.HasValue, x => x.OpenPrice <= input.OpenPriceTo.Value)
                .WhereIf(input.ClosePriceFrom.HasValue, x => x.ClosePrice >= input.ClosePriceFrom.Value)
                .WhereIf(input.ClosePriceTo.HasValue, x => x.ClosePrice <= input.ClosePriceTo.Value)
                .WhereIf(input.HighPriceFrom.HasValue, x => x.HighPrice >= input.HighPriceFrom.Value)
                .WhereIf(input.HighPriceTo.HasValue, x => x.HighPrice <= input.HighPriceTo.Value)
                .WhereIf(input.LowPriceFrom.HasValue, x => x.LowPrice >= input.LowPriceFrom.Value)
                .WhereIf(input.LowPriceTo.HasValue, x => x.LowPrice <= input.LowPriceTo.Value)
                .WhereIf(input.VolumeFrom.HasValue, x => x.Volume >= input.VolumeFrom.Value)
                .WhereIf(input.VolumeTo.HasValue, x => x.Volume <= input.VolumeTo.Value)
                .WhereIf(input.NavPerUnitFrom.HasValue, x => x.NavPerUnit >= input.NavPerUnitFrom.Value)
                .WhereIf(input.NavPerUnitTo.HasValue, x => x.NavPerUnit <= input.NavPerUnitTo.Value)
                .WhereIf(input.DailyReturnRateFrom.HasValue, x => x.DailyReturnRate >= input.DailyReturnRateFrom.Value)
                .WhereIf(input.DailyReturnRateTo.HasValue, x => x.DailyReturnRate <= input.DailyReturnRateTo.Value)
                .WhereIf(!input.InstrumentTypeIn.IsNullOrWhiteSpace(), x => input.InstrumentTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (MarketDataInstrumentType?)int.Parse(v.Trim()))
                    .Contains(x.InstrumentType))
                .WhereIf(input.InstrumentTypeNot.HasValue, x => x.InstrumentType != (MarketDataInstrumentType)input.InstrumentTypeNot.Value)
                .WhereIf(input.FundId.HasValue, x => x.FundId == input.FundId.Value);
        }

        public override async Task<MarketDataDto> CreateAsync(CreateMarketDataDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "MarketData", result);
            return result;
        }

        public override async Task<MarketDataDto> UpdateAsync(MarketDataDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "MarketData", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "MarketData", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.MarketData_Read)]
        public List<GroupCountDto> GetGroupedCount(MarketDataGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "InstrumentType", "FundId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "InstrumentType":
                    return query
                        .GroupBy(x => x.InstrumentType)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "FundId":
                    return query
                        .GroupBy(x => new { Key = x.FundId, Label = x.Fund == null ? null : x.Fund.FundName })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.MarketData_Read)]
        public decimal? GetStats(MarketDataStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "DataDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "OpenPrice", "ClosePrice", "HighPrice", "LowPrice", "Volume", "NavPerUnit", "DailyReturnRate" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "OpenPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.OpenPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.OpenPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.OpenPrice)
                            : query.Average(x => (decimal?)x.OpenPrice);
                        case "ClosePrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ClosePrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ClosePrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ClosePrice)
                            : query.Average(x => (decimal?)x.ClosePrice);
                        case "HighPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.HighPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.HighPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.HighPrice)
                            : query.Average(x => (decimal?)x.HighPrice);
                        case "LowPrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.LowPrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.LowPrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.LowPrice)
                            : query.Average(x => (decimal?)x.LowPrice);
                        case "Volume": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Volume)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Volume)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Volume)
                            : query.Average(x => (decimal?)x.Volume);
                        case "NavPerUnit": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.NavPerUnit)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.NavPerUnit)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.NavPerUnit)
                            : query.Average(x => (decimal?)x.NavPerUnit);
                        case "DailyReturnRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.DailyReturnRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.DailyReturnRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.DailyReturnRate)
                            : query.Average(x => (decimal?)x.DailyReturnRate);
                        default: return null;
            }
        }

    }
}
