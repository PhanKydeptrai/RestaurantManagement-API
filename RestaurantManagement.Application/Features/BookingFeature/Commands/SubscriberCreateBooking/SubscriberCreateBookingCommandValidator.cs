using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.SubscriberCreateBooking;

public class SubscriberCreateBookingCommandValidator : AbstractValidator<SubscriberCreateBookingCommand>
{
    public SubscriberCreateBookingCommandValidator(IBookingRepository bookingRepository)
    {
        RuleFor(a => a.BookingDate)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .Must(a => bookingRepository.IsBookingDateValid(a).Result == true)
            .WithMessage("{PropertyName} is invalid");

        RuleFor(a => a.BookingTime)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .Must(a => bookingRepository.IsBookingTimeValid(a).Result == true)
            .WithMessage("{PropertyName} is outside of the working hours");

        RuleFor(a => a.Note)
            .MaximumLength(250)
            .WithMessage("{PropertyName} must not exceed 250 characters");

        // RuleFor(a => a.NumberOfCustomers)
        //     .NotNull()
        //     .WithMessage("{PropertyName} is required")
        //     .NotEmpty()
        //     .WithMessage("{PropertyName} is required")
        //     .GreaterThan(0)
        //     .WithMessage("{PropertyName} must be greater than 0")
        //     .Must(a => bookingRepository.IsCapacityAvailable(a).Result == true)
        //     .WithMessage("Seat is not enough for {PropertyName} customers");
    }
}
