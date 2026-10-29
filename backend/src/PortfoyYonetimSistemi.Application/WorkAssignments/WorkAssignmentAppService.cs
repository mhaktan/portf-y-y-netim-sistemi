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
using PortfoyYonetimSistemi.WorkAssignments.Dto;
using PortfoyYonetimSistemi.Analytics.Dto;
using PortfoyYonetimSistemi.StateMachine.Dto;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Authorization;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.WorkAssignments
{
    [Abp.Authorization.AbpAuthorize]
    public class WorkAssignmentAppService : AsyncCrudAppService<
        WorkAssignment,
        WorkAssignmentDto,
        long,
        PagedWorkAssignmentResultRequestDto,
        CreateWorkAssignmentDto,
        WorkAssignmentDto>,
        IWorkAssignmentAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IRepository<ApprovalRecord, Guid> _approvalRepo;
        private readonly IFlowEngine _flowEngine;

        public WorkAssignmentAppService(IRepository<WorkAssignment, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo, IRepository<ApprovalRecord, Guid> approvalRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            _approvalRepo = approvalRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.WorkAssignment_Read;
            GetAllPermissionName = PermissionNames.WorkAssignment_Read;
            CreatePermissionName = PermissionNames.WorkAssignment_Create;
            UpdatePermissionName = PermissionNames.WorkAssignment_Update;
            DeletePermissionName = PermissionNames.WorkAssignment_Delete;
        }

        protected override IQueryable<WorkAssignment> CreateFilteredQuery(PagedWorkAssignmentResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.PortfolioManager)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.Title != null && x.Title.Contains(input.Keyword)) ||
                    (x.Description != null && x.Description.Contains(input.Keyword)) ||
                    (x.CompletionNote != null && x.CompletionNote.Contains(input.Keyword)) ||
                    (x.RevisionNote != null && x.RevisionNote.Contains(input.Keyword)))
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), x => x.Title != null && x.Title.Contains(input.Title))
                .WhereIf(!input.Description.IsNullOrWhiteSpace(), x => x.Description != null && x.Description.Contains(input.Description))
                .WhereIf(!input.CompletionNote.IsNullOrWhiteSpace(), x => x.CompletionNote != null && x.CompletionNote.Contains(input.CompletionNote))
                .WhereIf(!input.RevisionNote.IsNullOrWhiteSpace(), x => x.RevisionNote != null && x.RevisionNote.Contains(input.RevisionNote))
                .WhereIf(input.AssignmentType.HasValue, x => x.AssignmentType == (WorkAssignmentAssignmentType)input.AssignmentType.Value)
                .WhereIf(input.Priority.HasValue, x => x.Priority == (WorkAssignmentPriority)input.Priority.Value)
                .WhereIf(input.DueDate.HasValue, x => x.DueDate == input.DueDate.Value)
                .WhereIf(input.CompletedDate.HasValue, x => x.CompletedDate == input.CompletedDate.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (WorkAssignmentStatus)input.Status.Value)
                .WhereIf(input.DueDateFrom.HasValue, x => x.DueDate >= input.DueDateFrom.Value)
                .WhereIf(input.DueDateTo.HasValue, x => x.DueDate <= input.DueDateTo.Value)
                .WhereIf(input.CompletedDateFrom.HasValue, x => x.CompletedDate >= input.CompletedDateFrom.Value)
                .WhereIf(input.CompletedDateTo.HasValue, x => x.CompletedDate <= input.CompletedDateTo.Value)
                .WhereIf(!input.AssignmentTypeIn.IsNullOrWhiteSpace(), x => input.AssignmentTypeIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (WorkAssignmentAssignmentType?)int.Parse(v.Trim()))
                    .Contains(x.AssignmentType))
                .WhereIf(input.AssignmentTypeNot.HasValue, x => x.AssignmentType != (WorkAssignmentAssignmentType)input.AssignmentTypeNot.Value)
                .WhereIf(!input.PriorityIn.IsNullOrWhiteSpace(), x => input.PriorityIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (WorkAssignmentPriority?)int.Parse(v.Trim()))
                    .Contains(x.Priority))
                .WhereIf(input.PriorityNot.HasValue, x => x.Priority != (WorkAssignmentPriority)input.PriorityNot.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (WorkAssignmentStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (WorkAssignmentStatus)input.StatusNot.Value)
                .WhereIf(input.PortfolioManagerId.HasValue, x => x.PortfolioManagerId == input.PortfolioManagerId.Value);
        }

        public override async Task<WorkAssignmentDto> CreateAsync(CreateWorkAssignmentDto input)
        {
            // Durum is akisina ait: yeni kayit her zaman baslangic durumunda acilir (gonderme ChangeStatus ile)
            input.Status = (int)WorkAssignmentStatus.Assigned;
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "WorkAssignment", result);
            return result;
        }

        public override async Task<WorkAssignmentDto> UpdateAsync(WorkAssignmentDto input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Completed" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; düzenlenemez.");
            if ((int)existing.Status != input.Status)
                throw new Abp.UI.UserFriendlyException("Durum yalnızca iş akışı eylemleriyle değiştirilebilir.");
            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "WorkAssignment", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var existing = await Repository.GetAsync(input.Id);
            if (new[] { "PendingApproval", "Completed" }.Contains(existing.Status.ToString()))
                throw new Abp.UI.UserFriendlyException("Bu kayıt onay sürecinde ya da onaylanmış; silinemez.");
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "WorkAssignment", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.WorkAssignment_ChangeStatus, PermissionNames.WorkAssignment_Update, RequireAllPermissions = false)]
        public async Task<WorkAssignmentDto> ChangeStatusAsync(long id, ChangeStatusInput input)
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
        public async Task<WorkAssignmentDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
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
            ("Assigned", "InProgress", "Start", false),
            ("InProgress", "PendingApproval", "Submit", false),
            ("PendingApproval", "Completed", "Approve", false),
            ("PendingApproval", "InProgress", "Revise", false),
            ("*", "Cancelled", "Cancel", false)
        };

        private async Task<WorkAssignmentDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
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
            entity.Status = (WorkAssignmentStatus)Enum.Parse(typeof(WorkAssignmentStatus), transition.To);
            if (input.ActionData != null && input.ActionData.ContainsKey("revisionNote") && !string.IsNullOrWhiteSpace(input.ActionData["revisionNote"]))
                entity.RevisionNote = input.ActionData["revisionNote"];

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Cancel pending ApprovalRecords when the entity is cancelled — otherwise the records
            // sit forever in approvers' inboxes pointing to a cancelled request.
            if (input.Action == "Cancel")
            {
                var pending = _approvalRepo.GetAll()
                    .Where(a => a.EntityType == "WorkAssignment" && a.EntityId == id.ToString() && a.Status == "Pending")
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
                EntityType = "WorkAssignment",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "WorkAssignment", result);

            // Trigger named flow events
            if (input.Action == "Submit")
                await _flowEngine.TriggerAsync("submit-for-approval", "WorkAssignment", result);
            return result;
        }


        [Abp.Authorization.AbpAuthorize(PermissionNames.WorkAssignment_Read)]
        public List<GroupCountDto> GetGroupedCount(WorkAssignmentGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "AssignmentType", "Priority", "Status", "PortfolioManagerId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "AssignmentType":
                    return query
                        .GroupBy(x => x.AssignmentType)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "Priority":
                    return query
                        .GroupBy(x => x.Priority)
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
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.WorkAssignment_Read)]
        public decimal? GetStats(WorkAssignmentStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "DueDate", "CompletedDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "DueDate|CompletedDate":
                    {
                        var pairsDueDateCompletedDate = query
                            .Where(x => x.CompletedDate != null)
                            .Select(x => new { A = x.DueDate, B = x.CompletedDate.Value })
                            .ToList();
                        if (pairsDueDateCompletedDate.Count == 0) return null;
                        return (decimal)pairsDueDateCompletedDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "CompletedDate|DueDate":
                    {
                        var pairsCompletedDateDueDate = query
                            .Where(x => x.CompletedDate != null)
                            .Select(x => new { A = x.CompletedDate.Value, B = x.DueDate })
                            .ToList();
                        if (pairsCompletedDateDueDate.Count == 0) return null;
                        return (decimal)pairsCompletedDateDueDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    default: return null;
                }
            }

            var allowedNumeric = new string[0];
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.WorkAssignment_Read)]
        public async Task<WorkAssignmentReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()

                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new WorkAssignmentReportDto
            {
                Data = ObjectMapper.Map<WorkAssignmentDto>(root),

                // ApprovalRecord alan adlari: EntityType (isim) ve EntityId (STRING).
                // Once EntityName/long varsayilmisti — CS1061 + CS0019 veriyordu.
                ApprovalHistory = ObjectMapper.Map<List<ApprovalRecordDto>>(
                    _approvalRepo.GetAll()
                        .Where(a => a.EntityType == "WorkAssignment" && a.EntityId == id.ToString())
                        .OrderBy(a => a.StepIndex).ToList()),
            };
        }

    }
}
