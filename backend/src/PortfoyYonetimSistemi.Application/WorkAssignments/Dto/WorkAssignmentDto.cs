using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.WorkAssignments.Dto
{
    [AutoMapFrom(typeof(Entities.WorkAssignment))]
    public class WorkAssignmentDto : EntityDto<long>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int AssignmentType { get; set; }

        public int Priority { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public int Status { get; set; }

        public string CompletionNote { get; set; }

        public string RevisionNote { get; set; }

        /// <summary>
        /// String form of the status — used by flow conditions (triggerData.statusName equals "PendingX").
        /// </summary>
        public string StatusName { get; set; }

        public long PortfolioManagerId { get; set; }

        public string PortfolioManagerDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}