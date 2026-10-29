using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.WorkAssignments.Dto;

namespace PortfoyYonetimSistemi.WorkAssignments
{
    public class WorkAssignmentMapProfile : Profile
    {
        public WorkAssignmentMapProfile()
        {
            CreateMap<WorkAssignment, WorkAssignmentDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.PortfolioManagerDisplay, o => o.MapFrom(s => s.PortfolioManager != null ? s.PortfolioManager.FullName.ToString() : null));
            CreateMap<CreateWorkAssignmentDto, WorkAssignment>();
            CreateMap<WorkAssignmentDto, WorkAssignment>();
        }
    }
}
