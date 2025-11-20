using FluentValidation;
using Microsoft.AspNetCore.Rewrite;

namespace SteamClone.Backend.Validators;

public static class CentralValidator
{
  public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .EmailAddress().WithMessage(ValidationMessages.InvalidEmail);
  }

  public static IRuleBuilderOptions<T, string> ValidUsername<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .Matches(CommonRegEx.UsernamePattern).WithMessage(ValidationMessages.InvalidUsername);
  }

  public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .MinimumLength(8)
        .Matches("[A-Z]").WithMessage(ValidationMessages.PasswordTooWeak)
        .Matches("[a-z]").WithMessage(ValidationMessages.PasswordTooWeak)
        .Matches("[0-9]").WithMessage(ValidationMessages.PasswordTooWeak);
  }

  public static IRuleBuilderOptions<T, int> ValidQuantity<T>(this IRuleBuilder<T, int> rule)
  {
    return rule
        .GreaterThan(0).WithMessage(ValidationMessages.InvalidQuantity);
  }

  public static IRuleBuilderOptions<T, decimal> ValidPrice<T>(this IRuleBuilder<T, decimal> rule)
  {
    return rule
        .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.InvalidPrice);
  }

  public static IRuleBuilderOptions<T, int> ValidRating<T>(this IRuleBuilder<T, int> rule)
  {
    return rule
        .InclusiveBetween(1, 5).WithMessage(ValidationMessages.InvalidRating);
  }
}