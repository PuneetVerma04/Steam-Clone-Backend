using FluentValidation;
using SteamClone.Backend.DTOs.Order;

namespace SteamClone.Backend.Validators.Order;

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
  public OrderItemDtoValidator()
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