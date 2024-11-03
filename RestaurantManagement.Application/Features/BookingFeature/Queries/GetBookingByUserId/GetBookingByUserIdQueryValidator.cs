using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;

public class GetBookingByUserIdQueryValidator : AbstractValidator<GetBookingByUserIdQuery>
{
    public GetBookingByUserIdQueryValidator(IBookingRepository bookingRepository, ICustomerRepository customerRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => customerRepository.IsCustomerExist(a).Result == true)
            .WithMessage("Customer does not exist");
    }
}
