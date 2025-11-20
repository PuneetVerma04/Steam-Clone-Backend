using FluentValidation;
using SteamClone.Backend.DTOs.Cart;

namespace SteamClone.Backend.Validators.Cart;

public class CartRequestDtoValidator : AbstractValidator<CartRequest>
{
  public CartRequestDtoValidator()
  {
    RuleFor(x => x.GameId).ValidId();
    RuleFor(x => x.Quantity).ValidQuantity();
  }
}