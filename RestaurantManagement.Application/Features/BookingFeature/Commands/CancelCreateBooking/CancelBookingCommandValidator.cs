using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CancelCreateBooking;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator(IBookingRepository bookingRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("Id is required.")
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(a => bookingRepository.IsBookingExist(a).Result == true)
            .WithMessage("Booking does not exist.")
            .Must(a => bookingRepository.IsBookingCanceled(a).Result == false)
            .WithMessage("Booking is canceled.");


    }
}
