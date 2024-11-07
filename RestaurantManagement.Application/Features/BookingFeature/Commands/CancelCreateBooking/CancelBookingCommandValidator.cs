using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CancelCreateBooking;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator(IBookingRepository bookingRepository)
    {
        RuleFor(a => a.id)
            
            .Must(a => bookingRepository.IsBookingExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Booking does not exist.")
            .Must(a => bookingRepository.IsBookingCanceled(Ulid.Parse(a)).Result == false)
            .WithMessage("Booking is canceled.")
            .When(a => Ulid.TryParse(a.id, out _))
            
            .NotNull()
            .WithMessage("Id is required.")
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is invalid.");


    }
}
