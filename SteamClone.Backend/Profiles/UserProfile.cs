using AutoMapper;
using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.User;

namespace SteamClone.Backend.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.Role, opt => opt.Ignore()) // Role is set manually in the controller
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password is hashed separately
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
