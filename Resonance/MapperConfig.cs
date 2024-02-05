using AutoMapper;

namespace Resonance
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<CallVolumeDto, CallVolume>();
                //config.CreateMap<CallVolume, CallVolumeDto>();

                //config.CreateMap<UserDto, User>();
                //config.CreateMap<User, UserDto>();
            });
            return mapperConfig;
        }
    }
}
