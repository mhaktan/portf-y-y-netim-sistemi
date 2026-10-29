using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.PortfolioManagers.Dto
{
    [AutoMapTo(typeof(Entities.PortfolioManager))]
    public class CreatePortfolioManagerDto
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeCode { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public long? UserId { get; set; }

        public long BranchId { get; set; }

    }
}