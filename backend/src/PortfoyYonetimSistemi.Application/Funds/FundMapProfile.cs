using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.Funds.Dto;

namespace PortfoyYonetimSistemi.Funds
{
    public class FundMapProfile : Profile
    {
        public FundMapProfile()
        {
            CreateMap<Fund, FundDto>();
            CreateMap<CreateFundDto, Fund>();
            CreateMap<FundDto, Fund>();
        }
    }
}
