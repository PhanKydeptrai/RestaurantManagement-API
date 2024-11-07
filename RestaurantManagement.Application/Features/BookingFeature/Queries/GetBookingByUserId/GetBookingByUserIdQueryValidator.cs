using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;

public class GetBookingByUserIdQueryValidator : AbstractValidator<GetBookingByUserIdQuery>
{
    public GetBookingByUserIdQueryValidator(ICustomerRepository customerRepository)
    {   
        RuleFor(a => a.id) 
            .Must(a => customerRepository.IsCustomerExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Customer does not exist")
            .When(a => Ulid.TryParse(a.id, out _) == true)
            
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => Ulid.TryParse(a, out _) == true)
            .WithMessage("Id is not valid");
            
            
    }
}
