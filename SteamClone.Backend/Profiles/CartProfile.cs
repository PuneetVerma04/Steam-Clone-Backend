using AutoMapper;
using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Cart;

namespace SteamClone.Backend.Profiles;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Game.Title))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Game.Price))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Game.ImageUrl));
        CreateMap<CartRequest, CartItem>();
    }
}
