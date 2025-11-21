using AutoMapper;
using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Review;

namespace SteamClone.Backend.Profiles;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.GameTitle, opt => opt.MapFrom(src => src.Game.Title));
        CreateMap<ReviewCreateDto, Review>();
    }
}
