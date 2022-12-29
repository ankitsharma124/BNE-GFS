using AutoMapper;
using CoreBridge.Models.DTO;
using CoreBridge.Models.Entity;

namespace CoreBridge.Models.MapperProfiles
{
    public class GeneralPrrofile : Profile
    {
        public GeneralPrrofile()
        {
            CreateMap<TitleInfo, TitleInfoDto>().ReverseMap();
            CreateMap<AppUser, AppUserDto>().ReverseMap();
            CreateMap<GFSUser, GFSUserDto>().ReverseMap();
            CreateMap<UserPlatform, UserPlatformDto>().ReverseMap();
        }
    }
}
