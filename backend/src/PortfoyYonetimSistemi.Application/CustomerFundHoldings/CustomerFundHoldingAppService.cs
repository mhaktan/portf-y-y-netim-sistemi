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
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.CustomerFundHoldings
{
    [Abp.Authorization.AbpAuthorize]
    public class CustomerFundHoldingAppService : AsyncCrudAppService<
        CustomerFundHolding,
        CustomerFundHoldingDto,
        long,
        PagedCustomerFundHoldingResultRequestDto,
        CreateCustomerFundHoldingDto,
        CustomerFundHoldingDto>,
        ICustomerFundHoldingAppService
    {
        private readonly IFlowEngine _flowEngine;

        public CustomerFundHoldingAppService(IRepository<CustomerFundHolding, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.CustomerFundHolding_Read;
            GetAllPermissionName = PermissionNames.CustomerFundHolding_Read;
            CreatePermissionName = PermissionNames.CustomerFundHolding_Create;
            UpdatePermissionName = PermissionNames.CustomerFundHolding_Update;
            DeletePermissionName = PermissionNames.CustomerFundHolding_Delete;
        }

        protected override IQueryable<CustomerFundHolding> CreateFilteredQuery(PagedCustomerFundHoldingResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Customer, x => x.Fund)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword))
                .WhereIf(input.Units.HasValue, x => x.Units == input.Units.Value)
                .WhereIf(input.PurchasePrice.HasValue, x => x.PurchasePrice == input.PurchasePrice.Value)
                .WhereIf(input.CurrentValue.HasValue, x => x.CurrentValue == input.CurrentValue.Value)
                .WhereIf(input.PurchaseDate.HasValue, x => x.PurchaseDate == input.PurchaseDate.Value)
                .WhereIf(input.Weight.HasValue, x => x.Weight == input.Weight.Value)
                .WhereIf(input.ProfitLoss.HasValue, x => x.ProfitLoss == input.ProfitLoss.Value)
                .WhereIf(input.UnitsFrom.HasValue, x => x.Units >= input.UnitsFrom.Value)
                .WhereIf(input.UnitsTo.HasValue, x => x.Units <= input.UnitsTo.Value)
                .WhereIf(input.PurchasePriceFrom.HasValue, x => x.PurchasePrice >= input.PurchasePriceFrom.Value)
                .WhereIf(input.PurchasePriceTo.HasValue, x => x.PurchasePrice <= input.PurchasePriceTo.Value)
                .WhereIf(input.CurrentValueFrom.HasValue, x => x.CurrentValue >= input.CurrentValueFrom.Value)
                .WhereIf(input.CurrentValueTo.HasValue, x => x.CurrentValue <= input.CurrentValueTo.Value)
                .WhereIf(input.PurchaseDateFrom.HasValue, x => x.PurchaseDate >= input.PurchaseDateFrom.Value)
                .WhereIf(input.PurchaseDateTo.HasValue, x => x.PurchaseDate <= input.PurchaseDateTo.Value)
                .WhereIf(input.WeightFrom.HasValue, x => x.Weight >= input.WeightFrom.Value)
                .WhereIf(input.WeightTo.HasValue, x => x.Weight <= input.WeightTo.Value)
                .WhereIf(input.ProfitLossFrom.HasValue, x => x.ProfitLoss >= input.ProfitLossFrom.Value)
                .WhereIf(input.ProfitLossTo.HasValue, x => x.ProfitLoss <= input.ProfitLossTo.Value)
                .WhereIf(input.CustomerId.HasValue, x => x.CustomerId == input.CustomerId.Value)
                .WhereIf(input.FundId.HasValue, x => x.FundId == input.FundId.Value);
        }

        public override async Task<CustomerFundHoldingDto> CreateAsync(CreateCustomerFundHoldingDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "CustomerFundHolding", result);
            return result;
        }

        public override async Task<CustomerFundHoldingDto> UpdateAsync(CustomerFundHoldingDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "CustomerFundHolding", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "CustomerFundHolding", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.CustomerFundHolding_Read)]
        public List<GroupCountDto> GetGroupedCount(CustomerFundHoldingGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "CustomerId", "FundId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "CustomerId":
                    return query
                        .GroupBy(x => new { Key = x.CustomerId, Label = x.Customer == null ? null : x.Customer.FullName })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.CustomerFundHolding_Read)]
        public decimal? GetStats(CustomerFundHoldingStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "PurchaseDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "Units", "PurchasePrice", "CurrentValue", "Weight", "ProfitLoss" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "Units": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Units)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Units)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Units)
                            : query.Average(x => (decimal?)x.Units);
                        case "PurchasePrice": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.PurchasePrice)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.PurchasePrice)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.PurchasePrice)
                            : query.Average(x => (decimal?)x.PurchasePrice);
                        case "CurrentValue": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.CurrentValue)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.CurrentValue)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.CurrentValue)
                            : query.Average(x => (decimal?)x.CurrentValue);
                        case "Weight": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Weight)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Weight)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Weight)
                            : query.Average(x => (decimal?)x.Weight);
                        case "ProfitLoss": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ProfitLoss)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ProfitLoss)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ProfitLoss)
                            : query.Average(x => (decimal?)x.ProfitLoss);
                        default: return null;
            }
        }

    }
}
