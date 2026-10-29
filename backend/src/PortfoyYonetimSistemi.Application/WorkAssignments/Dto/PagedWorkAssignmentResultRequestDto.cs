using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.WorkAssignments.Dto
{
    public class PagedWorkAssignmentResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? PortfolioManagerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignmentType { get; set; }
        public int? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? Status { get; set; }
        public string CompletionNote { get; set; }
        public string RevisionNote { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public DateTime? CompletedDateFrom { get; set; }
        public DateTime? CompletedDateTo { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string AssignmentTypeIn { get; set; }
        public int? AssignmentTypeNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string PriorityIn { get; set; }
        public int? PriorityNot { get; set; }
        /// <summary>Virgülle ayrılmış enum indeksleri — ör. "0,2"</summary>
        public string StatusIn { get; set; }
        public int? StatusNot { get; set; }
    }
}
