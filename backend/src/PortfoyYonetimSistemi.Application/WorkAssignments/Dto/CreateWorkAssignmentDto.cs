using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.WorkAssignments.Dto
{
    [AutoMapTo(typeof(Entities.WorkAssignment))]
    public class CreateWorkAssignmentDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public int AssignmentType { get; set; }

        public int Priority { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public int Status { get; set; }

        [MaxLength(2000)]
        public string CompletionNote { get; set; }

        [MaxLength(1000)]
        public string RevisionNote { get; set; }

        public long PortfolioManagerId { get; set; }

    }
}