using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.PerformanceReports.Dto;

namespace PortfoyYonetimSistemi.PerformanceReports
{
    public class PerformanceReportMapProfile : Profile
    {
        public PerformanceReportMapProfile()
        {
            CreateMap<PerformanceReport, PerformanceReportDto>()
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.PortfolioManagerDisplay, o => o.MapFrom(s => s.PortfolioManager != null ? s.PortfolioManager.FullName.ToString() : null))
                .ForMember(d => d.TargetPeriodDisplay, o => o.MapFrom(s => s.TargetPeriod != null ? s.TargetPeriod.PeriodName.ToString() : null));
            CreateMap<CreatePerformanceReportDto, PerformanceReport>();
            CreateMap<PerformanceReportDto, PerformanceReport>();
        }
    }
}
