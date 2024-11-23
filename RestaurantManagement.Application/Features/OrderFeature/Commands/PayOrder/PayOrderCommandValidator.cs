using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;

#region New PayOrderCommandValidator
// public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
// {
//     public PayOrderCommandValidator(
//         ITableRepository tableRepository)
//     {
//         RuleFor(a => a.tableId)
//             .NotNull()
//             .WithMessage("{PropertyName} is required.")
//             .NotEmpty()
//             .WithMessage("{PropertyName} is required.")
//             .Must(a => int.TryParse(a, out _))
//             .WithMessage("{PropertyName} must be a number.")
//             .Must(a => tableRepository.IsTableExistAndActive(int.Parse(a)).Result == true)
//             .WithMessage("Table does not exist.");

//     }
// }
#endregion

#region New PayOrderCommandValidator
public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
{
    public PayOrderCommandValidator(
        ITableRepository tableRepository, 
        IVoucherRepository voucherRepository,
        ICustomerRepository customerRepository)
    {
        RuleFor(a => a.tableId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.")
            .Must(a => tableRepository.IsTableExistAndActive(int.Parse(a)).Result == true)
            .WithMessage("Table does not exist.");

        RuleFor(a => a.voucherName)
            .Must(a => voucherRepository.IsVoucherValid(a).Result == true)
            .WithMessage("{PropertyName} is not valid.")
            .When(a => !string.IsNullOrEmpty(a.voucherName) && !string.IsNullOrEmpty(a.phoneNumber));

        RuleFor(a => a.phoneNumber) //Kiểm tra số điện thoại khách hàng
            .Matches(@"^0\d{9}$")
            .WithMessage("PhoneNumber must start with 0 and be 10 digits long.")
            .Must(a => customerRepository.IsCustomerHasThisPhoneNumberActive(a).Result)
            .WithMessage("Phonenumber of customer is not exist")
            .When(a => !string.IsNullOrEmpty(a.voucherName) && !string.IsNullOrEmpty(a.phoneNumber));
    }
}
#endregion
