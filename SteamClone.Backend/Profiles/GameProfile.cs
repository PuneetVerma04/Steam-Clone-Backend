using AutoMapper;
using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Game;

namespace SteamClone.Backend.Profiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<Game, GameResponseDTO>()
            .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Username : null));
        CreateMap<CreateGameRequestDTO, Game>()
            .ForMember(dest => dest.Publisher, opt => opt.Ignore());
        CreateMap<UpdateGameRequestDTO, Game>()
            .ForMember(dest => dest.PublisherId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
