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
                config.CreateMap<UserDto, ResoUser>();
                config.CreateMap<ResoUser, UserDto>();
            });
            return mapperConfig;
        }
    }
}
