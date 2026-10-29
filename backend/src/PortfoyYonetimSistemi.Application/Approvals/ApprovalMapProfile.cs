using AutoMapper;
using PortfoyYonetimSistemi.Approvals.Dto;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Approvals
{
    public class ApprovalMapProfile : Profile
    {
        public ApprovalMapProfile()
        {
            CreateMap<ApprovalRecord, ApprovalRecordDto>();
            CreateMap<StatusChangeLog, StatusChangeLogDto>();
        }
    }
}
