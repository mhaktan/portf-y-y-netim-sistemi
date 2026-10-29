using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.Branchs.Dto;

namespace PortfoyYonetimSistemi.Branchs
{
    public class BranchMapProfile : Profile
    {
        public BranchMapProfile()
        {
            CreateMap<Branch, BranchDto>();
            CreateMap<CreateBranchDto, Branch>();
            CreateMap<BranchDto, Branch>();
        }
    }
}
