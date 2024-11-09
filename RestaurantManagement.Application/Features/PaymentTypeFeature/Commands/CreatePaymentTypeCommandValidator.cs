using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.PaymentTypeFeature.Commands;

public class CreatePaymentTypeCommandValidator : AbstractValidator<CreatePaymentTypeCommand>
{
    public CreatePaymentTypeCommandValidator(IPaymentTypeRepository paymentTypeRepository)
    {
        RuleFor(p => p.PaymentTypeName)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.")
            .Must(a => paymentTypeRepository.IsPaymentTypeExist(a).Result)
            .WithMessage("{PropertyName} already exists.");
    }
}
