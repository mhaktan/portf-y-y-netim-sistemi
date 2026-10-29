using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.CustomerManagerTransfers.Dto;

namespace PortfoyYonetimSistemi.CustomerManagerTransfers
{
    public class CustomerManagerTransferMapProfile : Profile
    {
        public CustomerManagerTransferMapProfile()
        {
            CreateMap<CustomerManagerTransfer, CustomerManagerTransferDto>()
                .ForMember(d => d.CustomerDisplay, o => o.MapFrom(s => s.Customer != null ? s.Customer.FullName.ToString() : null))
                .ForMember(d => d.PortfolioManagerDisplay, o => o.MapFrom(s => s.PortfolioManager != null ? s.PortfolioManager.FullName.ToString() : null));
            CreateMap<CreateCustomerManagerTransferDto, CustomerManagerTransfer>();
            CreateMap<CustomerManagerTransferDto, CustomerManagerTransfer>();
        }
    }
}
