using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace PortfoyYonetimSistemi.Branchs.Dto
{
    [AutoMapFrom(typeof(Entities.Branch))]
    public class BranchDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Region { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }

    }
}