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
using PortfoyYonetimSistemi.CustomerManagerTransfers.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.CustomerManagerTransfers
{
    [Abp.Authorization.AbpAuthorize]
    public class CustomerManagerTransferAppService : AsyncCrudAppService<
        CustomerManagerTransfer,
        CustomerManagerTransferDto,
        long,
        PagedCustomerManagerTransferResultRequestDto,
        CreateCustomerManagerTransferDto,
        CustomerManagerTransferDto>,
        ICustomerManagerTransferAppService
    {
        private readonly IFlowEngine _flowEngine;

        public CustomerManagerTransferAppService(IRepository<CustomerManagerTransfer, long> repository, IFlowEngine flowEngine)
            : base(repository)
        {
            _flowEngine = flowEngine;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.CustomerManagerTransfer_Read;
            GetAllPermissionName = PermissionNames.CustomerManagerTransfer_Read;
            CreatePermissionName = PermissionNames.CustomerManagerTransfer_Create;
            UpdatePermissionName = PermissionNames.CustomerManagerTransfer_Update;
            DeletePermissionName = PermissionNames.CustomerManagerTransfer_Delete;
        }

        protected override IQueryable<CustomerManagerTransfer> CreateFilteredQuery(PagedCustomerManagerTransferResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Customer, x => x.PortfolioManager)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.TransferReason != null && x.TransferReason.Contains(input.Keyword)) ||
                    (x.Notes != null && x.Notes.Contains(input.Keyword)))
                .WhereIf(!input.TransferReason.IsNullOrWhiteSpace(), x => x.TransferReason != null && x.TransferReason.Contains(input.TransferReason))
                .WhereIf(!input.Notes.IsNullOrWhiteSpace(), x => x.Notes != null && x.Notes.Contains(input.Notes))
                .WhereIf(input.TransferDate.HasValue, x => x.TransferDate == input.TransferDate.Value)
                .WhereIf(input.TransferDateFrom.HasValue, x => x.TransferDate >= input.TransferDateFrom.Value)
                .WhereIf(input.TransferDateTo.HasValue, x => x.TransferDate <= input.TransferDateTo.Value)
                .WhereIf(input.CustomerId.HasValue, x => x.CustomerId == input.CustomerId.Value)
                .WhereIf(input.PortfolioManagerId.HasValue, x => x.PortfolioManagerId == input.PortfolioManagerId.Value);
        }

        public override async Task<CustomerManagerTransferDto> CreateAsync(CreateCustomerManagerTransferDto input)
        {
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "CustomerManagerTransfer", result);
            return result;
        }

        public override async Task<CustomerManagerTransferDto> UpdateAsync(CustomerManagerTransferDto input)
        {
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "CustomerManagerTransfer", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "CustomerManagerTransfer", new { Id = input.Id });
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.CustomerManagerTransfer_Read)]
        public List<GroupCountDto> GetGroupedCount(CustomerManagerTransferGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "CustomerId", "PortfolioManagerId" };
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

    }
}
