using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.PortfolioManagers.Dto;

namespace PortfoyYonetimSistemi.PortfolioManagers
{
    public class PortfolioManagerMapProfile : Profile
    {
        public PortfolioManagerMapProfile()
        {
            CreateMap<PortfolioManager, PortfolioManagerDto>()
                .ForMember(d => d.UserDisplay, o => o.MapFrom(s => s.User != null ? s.User.Name.ToString() : null))
                .ForMember(d => d.BranchDisplay, o => o.MapFrom(s => s.Branch != null ? s.Branch.Name.ToString() : null));
            CreateMap<CreatePortfolioManagerDto, PortfolioManager>();
            CreateMap<PortfolioManagerDto, PortfolioManager>();
        }
    }
}
