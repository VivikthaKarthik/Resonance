using AutoMapper;
using ResoClassAPI.DTOs;
using ResoClassAPI.Models.Domain;

namespace Resonance
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UserDto, User>().ForMember(x => x.RoleId, opt => opt.Ignore()).ForMember(x => x.Role, opt => opt.Ignore());
                config.CreateMap<User, UserDto>().ForMember(x => x.Role, opt => opt.Ignore());
            });
            return mapperConfig;
        }
    }
}
