using AutoMapper;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.MarketDatas.Dto;

namespace PortfoyYonetimSistemi.MarketDatas
{
    public class MarketDataMapProfile : Profile
    {
        public MarketDataMapProfile()
        {
            CreateMap<MarketData, MarketDataDto>()
                .ForMember(d => d.FundDisplay, o => o.MapFrom(s => s.Fund != null ? s.Fund.FundName.ToString() : null));
            CreateMap<CreateMarketDataDto, MarketData>();
            CreateMap<MarketDataDto, MarketData>();
        }
    }
}
