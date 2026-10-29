using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;

namespace PortfoyYonetimSistemi.CustomerFundHoldings
{
    public class CustomerFundHoldingMapProfile : Profile
    {
        public CustomerFundHoldingMapProfile()
        {
            CreateMap<CustomerFundHolding, CustomerFundHoldingDto>()
                .ForMember(d => d.CustomerDisplay, o => o.MapFrom(s => s.Customer != null ? s.Customer.FullName.ToString() : null))
                .ForMember(d => d.FundDisplay, o => o.MapFrom(s => s.Fund != null ? s.Fund.FundName.ToString() : null));
            CreateMap<CreateCustomerFundHoldingDto, CustomerFundHolding>();
            CreateMap<CustomerFundHoldingDto, CustomerFundHolding>();
        }
    }
}
