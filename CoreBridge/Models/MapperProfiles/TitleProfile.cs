using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity.CoreBridge.Models.Entity;

namespace CoreBridge.Models.MapperProfiles
{
    public class TitleProfile : Profile
    {
        public TitleProfile()
        {
            CreateMap<TitleInfo, TitleInfoDto>().ReverseMap();
        }
    }
}
