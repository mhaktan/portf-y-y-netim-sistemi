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
using PortfoyYonetimSistemi.PerformanceReports.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.StateMachine.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.PerformanceReports
{
    [Abp.Authorization.AbpAuthorize]
    public class PerformanceReportAppService : AsyncCrudAppService<
        PerformanceReport,
        PerformanceReportDto,
        long,
        PagedPerformanceReportResultRequestDto,
        CreatePerformanceReportDto,
        PerformanceReportDto>,
        IPerformanceReportAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public PerformanceReportAppService(IRepository<PerformanceReport, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.PerformanceReport_Read;
            GetAllPermissionName = PermissionNames.PerformanceReport_Read;
            CreatePermissionName = PermissionNames.PerformanceReport_Create;
            UpdatePermissionName = PermissionNames.PerformanceReport_Update;
            DeletePermissionName = PermissionNames.PerformanceReport_Delete;
        }

        protected override IQueryable<PerformanceReport> CreateFilteredQuery(PagedPerformanceReportResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.PortfolioManager, x => x.TargetPeriod)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.ReportName != null && x.ReportName.Contains(input.Keyword)) ||
                    (x.Summary != null && x.Summary.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.ReportName.IsNullOrWhiteSpace(), x => x.ReportName != null && x.ReportName.Contains(input.ReportName))
                .WhereIf(!input.Summary.IsNullOrWhiteSpace(), x => x.Summary != null && x.Summary.Contains(input.Summary))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.ReportType.HasValue, x => x.ReportType == (PerformanceReportReportType)input.ReportType.Value)
                .WhereIf(input.ReportDate.HasValue, x => x.ReportDate == input.ReportDate.Value)
                .WhereIf(input.StartDate.HasValue, x => x.StartDate == input.StartDate.Value)
                .WhereIf(input.EndDate.HasValue, x => x.EndDate == input.EndDate.Value)
                .WhereIf(input.TotalAum.HasValue, x => x.TotalAum == input.TotalAum.Value)
                .WhereIf(input.AumChange.HasValue, x => x.AumChange == input.AumChange.Value)
                .WhereIf(input.AumChangeRate.HasValue, x => x.AumChangeRate == input.AumChangeRate.Value)
                .WhereIf(input.PortfolioReturnRate.HasValue, x => x.PortfolioReturnRate == input.PortfolioReturnRate.Value)
                .WhereIf(input.BenchmarkReturnRate.HasValue, x => x.BenchmarkReturnRate == input.BenchmarkReturnRate.Value)
                .WhereIf(input.AlphaValue.HasValue, x => x.AlphaValue == input.AlphaValue.Value)
                .WhereIf(input.CustomerCount.HasValue, x => x.CustomerCount == input.CustomerCount.Value)
                .WhereIf(input.NewCustomerCount.HasValue, x => x.NewCustomerCount == input.NewCustomerCount.Value)
                .WhereIf(input.TargetAchievementRate.HasValue, x => x.TargetAchievementRate == input.TargetAchievementRate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (PerformanceReportStatus)input.Status.Value)
                .WhereIf(input.ReportDateFrom.HasValue, x => x.ReportDate >= input.ReportDateFrom.Value)
                .WhereIf(input.ReportDateTo.HasValue, x => x.ReportDate <= input.ReportDateTo.Value)
                .WhereIf(input.StartDateFrom.HasValue, x => x.StartDate >= input.StartDateFrom.Value)
                .WhereIf(input.StartDateTo.HasValue, x => x.StartDate <= input.StartDateTo.Value)
                .WhereIf(input.EndDateFrom.HasValue, x => x.EndDate >= input.EndDateFrom.Value)
                .WhereIf(input.EndDateTo.HasValue, x => x.EndDate <= input.EndDateTo.Value)
                .WhereIf(input.TotalAumFrom.HasValue, x => x.TotalAum >= input.TotalAumFrom.Value)
                .WhereIf(input.TotalAumTo.HasValue, x => x.TotalAum <= input.TotalAumTo.Value)
                .WhereIf(input.AumChangeFrom.HasValue, x => x.AumChange >= input.AumChangeFrom.Value)
                .WhereIf(input.AumChangeTo.HasValue, x => x.AumChange <= input.AumChangeTo.Value)
                .WhereIf(input.AumChangeRateFrom.HasValue, x => x.AumChangeRate >= input.AumChangeRateFrom.Value)
                .WhereIf(input.AumChangeRateTo.HasValue, x => x.AumChangeRate <= input.AumChangeRateTo.Value)
                .WhereIf(input.PortfolioReturnRateFrom.HasValue, x => x.PortfolioReturnRate >= input.PortfolioReturnRateFrom.Value)
                .WhereIf(input.PortfolioReturnRateTo.HasValue, x => x.PortfolioReturnRate <= input.PortfolioReturnRateTo.Value)
                .WhereIf(input.BenchmarkReturnRateFrom.HasValue, x => x.BenchmarkReturnRate >= input.BenchmarkReturnRateFrom.Value)
                .WhereIf(input.BenchmarkReturnRateTo.HasValue, x => x.BenchmarkReturnRate <= input.BenchmarkReturnRateTo.Value)
                .WhereIf(input.AlphaValueFrom.HasValue, x => x.AlphaValue >= input.AlphaValueFrom.Value)
                .WhereIf(input.AlphaValueTo.HasValue, x => x.AlphaValue <= input.AlphaValueTo.Value)
                .WhereIf(input.CustomerCountFrom.HasValue, x => x.CustomerCount >= input.CustomerCountFrom.Value)
                .WhereIf(input.CustomerCountTo.HasValue, x => x.CustomerCount <= input.CustomerCountTo.Value)
                .WhereIf(input.NewCustomerCountFrom.HasValue, x => x.NewCustomerCount >= input.NewCustomerCountFrom.Value)
                .WhereIf(input.NewCustomerCountTo.HasValue, x => x.NewCustomerCount <= input.NewCustomerCountTo.Value)
                .WhereIf(input.TargetAchievementRateFrom.HasValue, x => x.TargetAchievementRate >= input.TargetAchievementRateFrom.Value)
                .WhereIf(input.TargetAchievementRateTo.HasValue, x => x.TargetAchievementRate <= input.TargetAchievementRateTo.Value)
                .WhereIf(!input.ReportTypeIn.IsNullOrWhiteSpace(), x => input.ReportTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (PerformanceReportReportType?)int.Parse(v.Trim()))
                    .Contains(x.ReportType))
                .WhereIf(input.ReportTypeNot.HasValue, x => x.ReportType != (PerformanceReportReportType)input.ReportTypeNot.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (PerformanceReportStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (PerformanceReportStatus)input.StatusNot.Value)
                .WhereIf(input.PortfolioManagerId.HasValue, x => x.PortfolioManagerId == input.PortfolioManagerId.Value)
                .WhereIf(input.TargetPeriodId.HasValue, x => x.TargetPeriodId == input.TargetPeriodId.Value);
        }

        public override async Task<PerformanceReportDto> CreateAsync(CreatePerformanceReportDto input)
        {
            // Durum is akisina ait: yeni kayit her zaman baslangic durumunda acilir (gonderme ChangeStatus ile)
            input.Status = (int)PerformanceReportStatus.Draft;
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "PerformanceReport", result);
            return result;
        }

        public override async Task<PerformanceReportDto> UpdateAsync(PerformanceReportDto input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Approved" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.");
            if ((int)existing.Status != input.Status)
                throw new Abp.UI.UserFriendlyException("Durum yalnızca iş akışı eylemleriyle değiştirilebilir.");
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "PerformanceReport", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Approved" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; silinemez.");
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "PerformanceReport", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.PerformanceReport_ChangeStatus, PermissionNames.PerformanceReport_Update, RequireAllPermissions = false)]
        public async Task<PerformanceReportDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            // Onay adimlari yalniz atanmis onayci tarafindan, onay servisi uzerinden ilerletilir
            if (input.Action == "Approve")
                throw new Abp.UI.UserFriendlyException("Onay adımları yalnızca onay ekranından ilerletilebilir.");
            // Onay bekleyen kayitta karar (Reject/Revise...) yalniz atanmis onaycinin onay ekranindan verilir; talep sahibi yalniz iptal edebilir
            var current = (await Repository.GetAsync(id)).Status.ToString();
            if (new[] { "PendingApproval" }.Contains(current) && input.Action != "Cancel")
                throw new Abp.UI.UserFriendlyException("Onay bekleyen kayıtta karar yalnızca onay ekranından verilir.");
            return await ApplyStatusChangeAsync(id, input);
        }

        // ABP dinamik API'si attribute'u implementasyonda arar (arayuzdeki yetmez)
        [Abp.Application.Services.RemoteService(false)]
        public async Task<PerformanceReportDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
        {
            // Onay ekranindaki ret karari: bu durumdan Revise yoksa Reject'e (ya da tersi) duser
            if (input.Action == "Revise" || input.Action == "Reject")
            {
                var current = (await Repository.GetAsync(id)).Status.ToString();
                bool Has(string a) => Transitions.Any(t => (t.From == "*" || t.From == current) && t.Action == a);
                var alt = input.Action == "Revise" ? "Reject" : "Revise";
                if (!Has(input.Action) && Has(alt)) input.Action = alt;
            }
            return await ApplyStatusChangeAsync(id, input);
        }

        private static readonly (string From, string To, string Action, bool Readonly)[] Transitions = new (string From, string To, string Action, bool Readonly)[]
        {
            ("Draft", "PendingApproval", "Submit", false),
            ("PendingApproval", "Approved", "Approve", false),
            ("PendingApproval", "Revision", "Revise", false),
            ("Revision", "PendingApproval", "Resubmit", false),
            ("*", "Cancelled", "Cancel", false)
        };

        private async Task<PerformanceReportDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            var transition = Transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Onay ekranindan gelen serbest metni gecisin zorunlu alanlarina tasi
            if (input.ActionData == null) input.ActionData = new Dictionary<string, string>();
            var genericNote = input.ActionData.ContainsKey("comment") && !string.IsNullOrWhiteSpace(input.ActionData["comment"])
                ? input.ActionData["comment"]
                : (input.ActionData.ContainsKey("revisionNote") ? input.ActionData["revisionNote"] : null);
            if (!string.IsNullOrWhiteSpace(genericNote))
            {
                foreach (var rf in new[] { "revisionNote" })
                {
                    if (!input.ActionData.ContainsKey(rf) || string.IsNullOrWhiteSpace(input.ActionData[rf]))
                        input.ActionData[rf] = genericNote;
                }
            }
            // Validate required fields per transition
            if (input.Action == "Revise" && (input.ActionData == null || !input.ActionData.ContainsKey("revisionNote") || string.IsNullOrWhiteSpace(input.ActionData["revisionNote"])))
                throw new Abp.UI.UserFriendlyException("Revise requires: revisionNote");
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (PerformanceReportStatus)Enum.Parse(typeof(PerformanceReportStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "PerformanceReport" && a.EntityId == id.ToString() && a.Status == "Pending")
                    .ToList();
                foreach (var pendingRec in pending)
                {
                    pendingRec.Status = "Cancelled";
                    pendingRec.ActionTaken = "Cancel";
                    pendingRec.ActionDate = DateTime.UtcNow;
                    pendingRec.Comment = "Entity cancelled by submitter.";
                    await _approvalRepo.UpdateAsync(pendingRec);
                }
            }

            // Log status change
            await _statusChangeLogRepo.InsertAsync(new Entities.StatusChangeLog
            {
                EntityType = "PerformanceReport",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "PerformanceReport", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "PerformanceReport", result);
            return result;
        }


        [Abp.Authorization.AbpAuthorize(PermissionNames.PerformanceReport_Read)]
        public List<GroupCountDto> GetGroupedCount(PerformanceReportGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "ReportType", "Status", "PortfolioManagerId", "TargetPeriodId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "ReportType":
                    return query
                        .GroupBy(x => x.ReportType)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "Status":
                    return query
                        .GroupBy(x => x.Status)
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
                case "TargetPeriodId":
                    return query
                        .GroupBy(x => new { Key = x.TargetPeriodId, Label = x.TargetPeriod == null ? null : x.TargetPeriod.PeriodName })
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.PerformanceReport_Read)]
        public decimal? GetStats(PerformanceReportStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "ReportDate", "StartDate", "EndDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "ReportDate|StartDate":
                    {
                        var pairsReportDateStartDate = query
                            .Select(x => new { A = x.ReportDate, B = x.StartDate })
                            .ToList();
                        if (pairsReportDateStartDate.Count == 0) return null;
                        return (decimal)pairsReportDateStartDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "ReportDate|EndDate":
                    {
                        var pairsReportDateEndDate = query
                            .Select(x => new { A = x.ReportDate, B = x.EndDate })
                            .ToList();
                        if (pairsReportDateEndDate.Count == 0) return null;
                        return (decimal)pairsReportDateEndDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "StartDate|ReportDate":
                    {
                        var pairsStartDateReportDate = query
                            .Select(x => new { A = x.StartDate, B = x.ReportDate })
                            .ToList();
                        if (pairsStartDateReportDate.Count == 0) return null;
                        return (decimal)pairsStartDateReportDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "StartDate|EndDate":
                    {
                        var pairsStartDateEndDate = query
                            .Select(x => new { A = x.StartDate, B = x.EndDate })
                            .ToList();
                        if (pairsStartDateEndDate.Count == 0) return null;
                        return (decimal)pairsStartDateEndDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "EndDate|ReportDate":
                    {
                        var pairsEndDateReportDate = query
                            .Select(x => new { A = x.EndDate, B = x.ReportDate })
                            .ToList();
                        if (pairsEndDateReportDate.Count == 0) return null;
                        return (decimal)pairsEndDateReportDate.Average(p => (p.B - p.A).TotalDays);
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

            var allowedNumeric = new[] { "TotalAum", "AumChange", "AumChangeRate", "PortfolioReturnRate", "BenchmarkReturnRate", "AlphaValue", "CustomerCount", "NewCustomerCount", "TargetAchievementRate" };
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
                        case "AumChange": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AumChange)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AumChange)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AumChange)
                            : query.Average(x => (decimal?)x.AumChange);
                        case "AumChangeRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AumChangeRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AumChangeRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AumChangeRate)
                            : query.Average(x => (decimal?)x.AumChangeRate);
                        case "PortfolioReturnRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.PortfolioReturnRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.PortfolioReturnRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.PortfolioReturnRate)
                            : query.Average(x => (decimal?)x.PortfolioReturnRate);
                        case "BenchmarkReturnRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.BenchmarkReturnRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.BenchmarkReturnRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.BenchmarkReturnRate)
                            : query.Average(x => (decimal?)x.BenchmarkReturnRate);
                        case "AlphaValue": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AlphaValue)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AlphaValue)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AlphaValue)
                            : query.Average(x => (decimal?)x.AlphaValue);
                        case "CustomerCount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.CustomerCount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.CustomerCount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.CustomerCount)
                            : query.Average(x => (decimal?)x.CustomerCount);
                        case "NewCustomerCount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.NewCustomerCount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.NewCustomerCount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.NewCustomerCount)
                            : query.Average(x => (decimal?)x.NewCustomerCount);
                        case "TargetAchievementRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TargetAchievementRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TargetAchievementRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TargetAchievementRate)
                            : query.Average(x => (decimal?)x.TargetAchievementRate);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.PerformanceReport_Read)]
        public async Task<PerformanceReportReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()

                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new PerformanceReportReportDto
            {
                Data = ObjectMapper.Map<PerformanceReportDto>(root),

                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "PerformanceReport" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
