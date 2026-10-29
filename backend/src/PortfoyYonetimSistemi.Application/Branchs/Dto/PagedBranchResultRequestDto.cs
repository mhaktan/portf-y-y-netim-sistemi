using System;
using Abp.Application.Services.Dto;

namespace PortfoyYonetimSistemi.Branchs.Dto
{
    public class PagedBranchResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Region { get; set; }
    }
}
