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

        RuleFor(a => a.NumberOfCustomers)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .Must(a => !string.IsNullOrEmpty(a.ToString()) && int.TryParse(a.ToString(), out _))
            .WithMessage("{PropertyName} must be a number")
            .Must(a => !string.IsNullOrEmpty(a.ToString()) && bookingRepository.IsCapacityAvailable(int.Parse(a.ToString())).Result == true)
            .WithMessage("Seat is not enough for {PropertyName} customers");
    }


    #region Stable code
    // public SubscriberCreateBookingCommandValidator(IBookingRepository bookingRepository)
    // {
    //     RuleFor(a => a.BookingDate)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .Must(a => bookingRepository.IsBookingDateValid(a).Result == true)
    //         .WithMessage("{PropertyName} is invalid");

    //     RuleFor(a => a.BookingTime)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .Must(a => bookingRepository.IsBookingTimeValid(a).Result == true)
    //         .WithMessage("{PropertyName} is outside of the working hours");

    //     RuleFor(a => a.Note)
    //         .MaximumLength(250)
    //         .WithMessage("{PropertyName} must not exceed 250 characters");

    //     RuleFor(a => a.NumberOfCustomers)
    //         .Must(a => bookingRepository.IsCapacityAvailable((int)a).Result == true)
    //         .WithMessage("Seat is not enough for {PropertyName} customers")
    //         .When(a => a != null && int.TryParse(a.NumberOfCustomers.ToString(), out _))

    //         .NotNull()
    //         .WithMessage("{PropertyName} is required")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .Must(a => a != null && int.TryParse(a.ToString(), out _))
    //         .WithMessage("{PropertyName} must be a number");
    // }
    #endregion
}
