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
using PortfoyYonetimSistemi.Customers.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.CustomerManagerTransfers.Dto;
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Customers
{
    [Abp.Authorization.AbpAuthorize]
    public class CustomerAppService : AsyncCrudAppService<
        Customer,
        CustomerDto,
        long,
        PagedCustomerResultRequestDto,
        CreateCustomerDto,
        CustomerDto>,
        ICustomerAppService
    {
        private readonly IFlowEngine _flowEngine;

        public CustomerAppService(IRepository<Customer, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Customer_Read;
            GetAllPermissionName = PermissionNames.Customer_Read;
            CreatePermissionName = PermissionNames.Customer_Create;
            UpdatePermissionName = PermissionNames.Customer_Update;
            DeletePermissionName = PermissionNames.Customer_Delete;
        }

        protected override IQueryable<Customer> CreateFilteredQuery(PagedCustomerResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.PortfolioManager)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.FullName != null && x.FullName.Contains(input.Keyword)) ||
                    (x.CustomerCode != null && x.CustomerCode.Contains(input.Keyword)) ||
                    (x.Phone != null && x.Phone.Contains(input.Keyword)) ||
                    (x.Email != null && x.Email.Contains(input.Keyword)))
                .WhereIf(!input.FullName.IsNullOrWhiteSpace(), x => x.FullName != null && x.FullName.Contains(input.FullName))
                .WhereIf(!input.CustomerCode.IsNullOrWhiteSpace(), x => x.CustomerCode != null && x.CustomerCode.Contains(input.CustomerCode))
                .WhereIf(!input.Phone.IsNullOrWhiteSpace(), x => x.Phone != null && x.Phone.Contains(input.Phone))
                .WhereIf(!input.Email.IsNullOrWhiteSpace(), x => x.Email != null && x.Email.Contains(input.Email))
                .WhereIf(input.CustomerType.HasValue, x => x.CustomerType == (CustomerCustomerType)input.CustomerType.Value)
                .WhereIf(input.RiskProfile.HasValue, x => x.RiskProfile == (CustomerRiskProfile)input.RiskProfile.Value)
                .WhereIf(input.TotalAum.HasValue, x => x.TotalAum == input.TotalAum.Value)
                .WhereIf(input.OnboardingDate.HasValue, x => x.OnboardingDate == input.OnboardingDate.Value)
                .WhereIf(input.TotalAumFrom.HasValue, x => x.TotalAum >= input.TotalAumFrom.Value)
                .WhereIf(input.TotalAumTo.HasValue, x => x.TotalAum <= input.TotalAumTo.Value)
                .WhereIf(input.OnboardingDateFrom.HasValue, x => x.OnboardingDate >= input.OnboardingDateFrom.Value)
                .WhereIf(input.OnboardingDateTo.HasValue, x => x.OnboardingDate <= input.OnboardingDateTo.Value)
                .WhereIf(!input.CustomerTypeIn.IsNullOrWhiteSpace(), x => input.CustomerTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (CustomerCustomerType?)int.Parse(v.Trim()))
                    .Contains(x.CustomerType))
                .WhereIf(input.CustomerTypeNot.HasValue, x => x.CustomerType != (CustomerCustomerType)input.CustomerTypeNot.Value)
                .WhereIf(!input.RiskProfileIn.IsNullOrWhiteSpace(), x => input.RiskProfileIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (CustomerRiskProfile?)int.Parse(v.Trim()))
                    .Contains(x.RiskProfile))
                .WhereIf(input.RiskProfileNot.HasValue, x => x.RiskProfile != (CustomerRiskProfile)input.RiskProfileNot.Value)
                .WhereIf(input.PortfolioManagerId.HasValue, x => x.PortfolioManagerId == input.PortfolioManagerId.Value);
        }

        public override async Task<CustomerDto> CreateAsync(CreateCustomerDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Customer", result);
            return result;
        }

        public override async Task<CustomerDto> UpdateAsync(CustomerDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Customer", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Customer", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Customer_Read)]
        public List<GroupCountDto> GetGroupedCount(CustomerGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "CustomerType", "RiskProfile", "PortfolioManagerId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "CustomerType":
                    return query
                        .GroupBy(x => x.CustomerType)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "RiskProfile":
                    return query
                        .GroupBy(x => x.RiskProfile)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "PortfolioManagerId":
                    return query
                        .GroupBy(x => new { Key = x.PortfolioManagerId, Label = x.PortfolioManager == null ? null : x.PortfolioManager.FullName })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.Customer_Read)]
        public decimal? GetStats(CustomerStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "OnboardingDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "TotalAum" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "TotalAum": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TotalAum)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TotalAum)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TotalAum)
                            : query.Average(x => (decimal?)x.TotalAum);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Customer_Read)]
        public async Task<CustomerReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.CustomerManagerTransfers)
                .Include(x => x.CustomerFundHoldings)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new CustomerReportDto
            {
                Data = ObjectMapper.Map<CustomerDto>(root),
                CustomerManagerTransfers = ObjectMapper.Map<List<CustomerManagerTransferDto>>(
                    root.CustomerManagerTransfers == null ? new List<CustomerManagerTransfer>() : root.CustomerManagerTransfers.ToList()),
                CustomerFundHoldings = ObjectMapper.Map<List<CustomerFundHoldingDto>>(
                    root.CustomerFundHoldings == null ? new List<CustomerFundHolding>() : root.CustomerFundHoldings.ToList()),
            };
        }

    }
}
