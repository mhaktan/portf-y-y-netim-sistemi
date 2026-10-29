using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.PortfolioManagers.Dto
{
    [AutoMapFrom(typeof(Entities.PortfolioManager))]
    public class PortfolioManagerDto : EntityDto<long>
    {
        public string FullName { get; set; }

        public string EmployeeCode { get; set; }

        public string Title { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public long? UserId { get; set; }

        public string UserDisplay { get; set; }

        public long BranchId { get; set; }

        public string BranchDisplay { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}