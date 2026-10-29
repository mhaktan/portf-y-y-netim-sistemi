using System;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.Branchs.Dto
{
    [AutoMapTo(typeof(Entities.Branch))]
    public class CreateBranchDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Code { get; set; }

        [MaxLength(100)]
        public string Region { get; set; }

    }
}