using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.TargetPeriods.Dto;

namespace PortfoyYonetimSistemi.TargetPeriods
{
    public class TargetPeriodMapProfile : Profile
    {
        public TargetPeriodMapProfile()
        {
            CreateMap<TargetPeriod, TargetPeriodDto>();
            CreateMap<CreateTargetPeriodDto, TargetPeriod>();
            CreateMap<TargetPeriodDto, TargetPeriod>();
        }
    }
}
