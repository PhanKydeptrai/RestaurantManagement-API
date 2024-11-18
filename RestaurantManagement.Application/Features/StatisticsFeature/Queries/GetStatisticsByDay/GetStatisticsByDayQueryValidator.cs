using FluentValidation;

namespace RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetStatisticsByDay;

public class GetStatisticsByDayQueryValidator : AbstractValidator<GetStatisticsByDayQuery>
{
    public GetStatisticsByDayQueryValidator()
    {
        RuleFor(p => p.datetime)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .NotNull()
            .Must(a => DateOnly.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a valid date."); 
    }
}
