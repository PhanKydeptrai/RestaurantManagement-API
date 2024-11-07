using FluentValidation;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByBookingId;

public class GetBookingByBookingIdQueryValidator : AbstractValidator<GetBookingByBookingIdQuery>
{
    public GetBookingByBookingIdQueryValidator()
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not valid.");
    }
}
