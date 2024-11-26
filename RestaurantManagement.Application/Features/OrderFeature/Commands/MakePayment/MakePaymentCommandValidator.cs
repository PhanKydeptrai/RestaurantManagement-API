using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.MakePayment;

public class MakePaymentCommandValidator : AbstractValidator<MakePaymentCommand>
{
    public MakePaymentCommandValidator(
        ITableRepository tableRepository, 
        IVoucherRepository voucherRepository, 
        ICustomerRepository customerRepository)
    {
        RuleFor(a => a.tableId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => a != null && int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.")
            .Must(a => a != null && tableRepository.IsTableExistAndActive(int.Parse(a)).Result == true)
            .WithMessage("Table does not exist.");

        RuleFor(a => a.voucherCode)
            .Must(a => voucherRepository.IsVoucherCodeUseable(a).Result == true)
            .WithMessage("{PropertyName} is not useable.")
            .When(a => !string.IsNullOrEmpty(a.voucherCode));

        RuleFor(a => a.phoneNumber) //Kiểm tra số điện thoại khách hàng
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches(@"^0\d{9}$")
            .WithMessage("PhoneNumber must start with 0 and be 10 digits long.")
            .Must(a => customerRepository.IsCustomerHasThisPhoneNumberActive(a).Result)
            .WithMessage("Phonenumber of customer is not exist")
            .When(a => !string.IsNullOrEmpty(a.voucherCode) || !string.IsNullOrEmpty(a.phoneNumber));
    }
}
