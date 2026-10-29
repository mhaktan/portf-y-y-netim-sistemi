using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.ManagerTargets.Dto;

namespace PortfoyYonetimSistemi.ManagerTargets
{
    public class ManagerTargetMapProfile : Profile
    {
        public ManagerTargetMapProfile()
        {
            CreateMap<ManagerTarget, ManagerTargetDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.TargetPeriodDisplay, o => o.MapFrom(s => s.TargetPeriod != null ? s.TargetPeriod.PeriodName.ToString() : null))
                .ForMember(d => d.PortfolioManagerDisplay, o => o.MapFrom(s => s.PortfolioManager != null ? s.PortfolioManager.FullName.ToString() : null));
            CreateMap<CreateManagerTargetDto, ManagerTarget>();
            CreateMap<ManagerTargetDto, ManagerTarget>();
        }
    }
}
