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
using PortfoyYonetimSistemi.ManagerTargets.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.StateMachine.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.ManagerTargets
{
    [Abp.Authorization.AbpAuthorize]
    public class ManagerTargetAppService : AsyncCrudAppService<
        ManagerTarget,
        ManagerTargetDto,
        long,
        PagedManagerTargetResultRequestDto,
        CreateManagerTargetDto,
        ManagerTargetDto>,
        IManagerTargetAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public ManagerTargetAppService(IRepository<ManagerTarget, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.ManagerTarget_Read;
            GetAllPermissionName = PermissionNames.ManagerTarget_Read;
            CreatePermissionName = PermissionNames.ManagerTarget_Create;
            UpdatePermissionName = PermissionNames.ManagerTarget_Update;
            DeletePermissionName = PermissionNames.ManagerTarget_Delete;
        }

        protected override IQueryable<ManagerTarget> CreateFilteredQuery(PagedManagerTargetResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.TargetPeriod, x => x.PortfolioManager)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.Unit != null && x.Unit.Contains(input.Keyword)) ||
                    (x.Notes != null && x.Notes.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.Unit.IsNullOrWhiteSpace(), x => x.Unit != null && x.Unit.Contains(input.Unit))
                .WhereIf(!input.Notes.IsNullOrWhiteSpace(), x => x.Notes != null && x.Notes.Contains(input.Notes))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.TargetType.HasValue, x => x.TargetType == (ManagerTargetTargetType)input.TargetType.Value)
                .WhereIf(input.TargetValue.HasValue, x => x.TargetValue == input.TargetValue.Value)
                .WhereIf(input.ActualValue.HasValue, x => x.ActualValue == input.ActualValue.Value)
                .WhereIf(input.AchievementRate.HasValue, x => x.AchievementRate == input.AchievementRate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (ManagerTargetStatus)input.Status.Value)
                .WhereIf(input.TargetValueFrom.HasValue, x => x.TargetValue >= input.TargetValueFrom.Value)
                .WhereIf(input.TargetValueTo.HasValue, x => x.TargetValue <= input.TargetValueTo.Value)
                .WhereIf(input.ActualValueFrom.HasValue, x => x.ActualValue >= input.ActualValueFrom.Value)
                .WhereIf(input.ActualValueTo.HasValue, x => x.ActualValue <= input.ActualValueTo.Value)
                .WhereIf(input.AchievementRateFrom.HasValue, x => x.AchievementRate >= input.AchievementRateFrom.Value)
                .WhereIf(input.AchievementRateTo.HasValue, x => x.AchievementRate <= input.AchievementRateTo.Value)
                .WhereIf(!input.TargetTypeIn.IsNullOrWhiteSpace(), x => input.TargetTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ManagerTargetTargetType?)int.Parse(v.Trim()))
                    .Contains(x.TargetType))
                .WhereIf(input.TargetTypeNot.HasValue, x => x.TargetType != (ManagerTargetTargetType)input.TargetTypeNot.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (ManagerTargetStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (ManagerTargetStatus)input.StatusNot.Value)
                .WhereIf(input.TargetPeriodId.HasValue, x => x.TargetPeriodId == input.TargetPeriodId.Value)
                .WhereIf(input.PortfolioManagerId.HasValue, x => x.PortfolioManagerId == input.PortfolioManagerId.Value);
        }

        public override async Task<ManagerTargetDto> CreateAsync(CreateManagerTargetDto input)
        {
            // Durum is akisina ait: yeni kayit her zaman baslangic durumunda acilir (gonderme ChangeStatus ile)
            input.Status = (int)ManagerTargetStatus.Draft;
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "ManagerTarget", result);
            return result;
        }

        public override async Task<ManagerTargetDto> UpdateAsync(ManagerTargetDto input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Active" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.");
            if ((int)existing.Status != input.Status)
                throw new Abp.UI.UserFriendlyException("Durum yalnızca iş akışı eylemleriyle değiştirilebilir.");
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "ManagerTarget", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Active" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; silinemez.");
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "ManagerTarget", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.ManagerTarget_ChangeStatus, PermissionNames.ManagerTarget_Update, RequireAllPermissions = false)]
        public async Task<ManagerTargetDto> ChangeStatusAsync(long id, ChangeStatusInput input)
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
        public async Task<ManagerTargetDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
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
            ("PendingApproval", "Active", "Approve", false),
            ("PendingApproval", "Draft", "Revise", false),
            ("Draft", "PendingApproval", "Resubmit", false),
            ("Active", "Completed", "Complete", false),
            ("*", "Cancelled", "Cancel", false)
        };

        private async Task<ManagerTargetDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
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
            entity.Status = (ManagerTargetStatus)Enum.Parse(typeof(ManagerTargetStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "ManagerTarget" && a.EntityId == id.ToString() && a.Status == "Pending")
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
                EntityType = "ManagerTarget",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "ManagerTarget", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "ManagerTarget", result);
            return result;
        }


        [Abp.Authorization.AbpAuthorize(PermissionNames.ManagerTarget_Read)]
        public List<GroupCountDto> GetGroupedCount(ManagerTargetGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "TargetType", "Status", "TargetPeriodId", "PortfolioManagerId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "TargetType":
                    return query
                        .GroupBy(x => x.TargetType)
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

        [Abp.Authorization.AbpAuthorize(PermissionNames.ManagerTarget_Read)]
        public decimal? GetStats(ManagerTargetStatsInput input)
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

            var allowedNumeric = new[] { "TargetValue", "ActualValue", "AchievementRate" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "TargetValue": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.TargetValue)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.TargetValue)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.TargetValue)
                            : query.Average(x => (decimal?)x.TargetValue);
                        case "ActualValue": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.ActualValue)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.ActualValue)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.ActualValue)
                            : query.Average(x => (decimal?)x.ActualValue);
                        case "AchievementRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.AchievementRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.AchievementRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.AchievementRate)
                            : query.Average(x => (decimal?)x.AchievementRate);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.ManagerTarget_Read)]
        public async Task<ManagerTargetReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()

                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new ManagerTargetReportDto
            {
                Data = ObjectMapper.Map<ManagerTargetDto>(root),

                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "ManagerTarget" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
