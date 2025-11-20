using FluentValidation;
using SteamClone.Backend.DTOs.Cart;

namespace SteamClone.Backend.Validators.Cart;

public class CartItemDtoValidator : AbstractValidator<CartItemDto>
{
    public CartItemDtoValidator()
    {
        RuleFor(x => x.GameId).ValidId();

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title!).ValidTitle();
        });

        RuleFor(x => x.Quantity).ValidQuantity();
        RuleFor(x => x.Price).ValidPrice();

        When(x => x.ImageUrl != null, () =>
        {
            RuleFor(x => x.ImageUrl!).ValidUrl();
        });
    }
}