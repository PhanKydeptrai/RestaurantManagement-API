using FluentValidation;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(p => p.id)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not a valid Ulid.");
    }
}
