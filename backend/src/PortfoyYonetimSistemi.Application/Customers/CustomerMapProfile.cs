using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.Customers.Dto;

namespace PortfoyYonetimSistemi.Customers
{
    public class CustomerMapProfile : Profile
    {
        public CustomerMapProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.PortfolioManagerDisplay, o => o.MapFrom(s => s.PortfolioManager != null ? s.PortfolioManager.FullName.ToString() : null));
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<CustomerDto, Customer>();
        }
    }
}
