using FluentValidation;
using SteamClone.Backend.DTOs.Coupon;
using SteamClone.Backend.Validators;

public class CreateCouponDtoValidator : AbstractValidator<CreateCouponDto>
{
  public CreateCouponDtoValidator()
  {
    RuleFor(x => x.Code)
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .MaximumLength(50).WithMessage("Coupon code too long.")
        .Matches("^[A-Z0-9_-]+$").WithMessage("Coupon code contains invalid characters.");

    RuleFor(x => x.CouponName)
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .MaximumLength(100).WithMessage("Coupon name too long.");

    RuleFor(x => x.DiscountPercent)
        .InclusiveBetween(1, 100).WithMessage("Discount must be between 1 and 100.");

    RuleFor(x => x.ExpirationDate)
        .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future.");
  }
}
