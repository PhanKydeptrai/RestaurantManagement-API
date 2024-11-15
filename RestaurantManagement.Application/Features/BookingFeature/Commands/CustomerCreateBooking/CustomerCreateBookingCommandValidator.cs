using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CustomerCreateBooking;

public class CustomerCreateBookingCommandValidator : AbstractValidator<CustomerCreateBookingCommand>
{
    public CustomerCreateBookingCommandValidator(IBookingRepository bookingRepository)
    {
        RuleFor(a => a.FirstName)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters");

        RuleFor(a => a.LastName)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters");

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

        RuleFor(a => a.NumberOfCustomers)
            .Must(a => int.Parse(a.ToString()) > 0)
            .WithMessage("{PropertyName} must be greater than 0")
            .Must(a => bookingRepository.IsCapacityAvailable(int.Parse(a.ToString())).Result == true)
            .WithMessage("Seat is not enough for {PropertyName} customers")
            .When(a => a != null && int.TryParse(a.NumberOfCustomers.ToString(), out _))

            .NotNull()
            .WithMessage("{PropertyName} is null")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .Must(a => a != null && int.TryParse(a.ToString(), out _))
            .WithMessage("{PropertyName} must be a number");
            

        RuleFor(a => a.Note)
            .MaximumLength(250)
            .WithMessage("{PropertyName} must not exceed 250 characters");

        RuleFor(a => a.Email)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .EmailAddress()
            .WithMessage("{PropertyName} is invalid");

        RuleFor(a => a.PhoneNumber)
            .NotNull()
            .WithMessage("Phonenumber is required.")
            .NotEmpty()
            .WithMessage("Phonenumber is required.")
            .Matches(@"^0\d{9}$")
            .WithMessage("PhoneNumber must start with 0 and be 10 digits long.");


    }
    #region Stable Code
    // public CustomerCreateBookingCommandValidator(IBookingRepository bookingRepository)
    // {
    //     RuleFor(a => a.FirstName)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .MaximumLength(50)
    //         .WithMessage("{PropertyName} must not exceed 50 characters");

    //     RuleFor(a => a.LastName)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .MaximumLength(50)
    //         .WithMessage("{PropertyName} must not exceed 50 characters");

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

    //     RuleFor(a => a.NumberOfCustomers)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is null")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .GreaterThan(0)
    //         .WithMessage("{PropertyName} must be greater than 0")
    //         .Must(a => bookingRepository.IsCapacityAvailable(a).Result == true)
    //         .WithMessage("Seat is not enough for {PropertyName} customers");

    //     RuleFor(a => a.Note)
    //         .MaximumLength(250)
    //         .WithMessage("{PropertyName} must not exceed 250 characters");

    //     RuleFor(a => a.Email)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .EmailAddress()
    //         .WithMessage("{PropertyName} is invalid");

    //     RuleFor(a => a.PhoneNumber)
    //         .NotNull()
    //         .WithMessage("Phonenumber is required.")
    //         .NotEmpty()
    //         .WithMessage("Phonenumber is required.")
    //         .Matches(@"^0\d{9}$")
    //         .WithMessage("PhoneNumber must start with 0 and be 10 digits long.");


    // }
    #endregion
}

