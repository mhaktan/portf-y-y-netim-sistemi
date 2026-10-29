using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.PortfolioManagers.Dto
{
    public class PagedPortfolioManagerResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? UserId { get; set; }
        public long? BranchId { get; set; }
        public string FullName { get; set; }
        public string EmployeeCode { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
    }
}
