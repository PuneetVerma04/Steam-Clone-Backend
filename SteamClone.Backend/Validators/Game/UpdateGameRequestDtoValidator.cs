using FluentValidation;
using SteamClone.Backend.DTOs.Game;
using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Validators.Game;

public class UpdateGameRequestDTOValidator : AbstractValidator<UpdateGameRequestDTO>
{
    private readonly BackendDbContext _dbContext;

    public UpdateGameRequestDTOValidator(BackendDbContext dbContext)
    {
        _dbContext = dbContext;

        // Id field removed from DTO - it comes from route parameter

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title!).ValidTitle();
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description!).ValidDescription();
        });

        When(x => x.Price != null, () =>
        {
            RuleFor(x => x.Price!.Value).ValidPrice();
        });

        When(x => x.Genre != null, () =>
        {
            RuleFor(x => x.Genre!)
                .NotEmpty().WithMessage("Genre cannot be empty.");
        });

        RuleFor(x => x.PublisherId)
            .Null()
            .WithMessage("PublisherId cannot be changed.");

        When(x => x.ReleaseDate != null, () =>
        {
            RuleFor(x => x.ReleaseDate!.Value).ValidReleaseDate();
        });

        When(x => x.ImageUrl != null, () =>
        {
            RuleFor(x => x.ImageUrl!).ValidUrl();
        });
    }

    private bool BeAValidPublisher(int publisherId)
    {
        return _dbContext.Users.Any(u => u.Id == publisherId && u.Role == UserRole.Publisher);
    }
}
