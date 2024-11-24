using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.CreateVoucher;

public class CreateVoucherCommandValidator : AbstractValidator<CreateVoucherCommand>
{
    // public Ulid VoucherId { get; set; }
    // public string VoucherName { get; set; } //Tên voucher, Sử dụng để áp dụng
    // public string VoucherCode { get; set; } //Sử dụng để áp dụng
    // public string VoucherType { get; set; } //Loại voucher 
    // public int? PercentageDiscount { get; set; } //Phần trăm giảm giá
    // public string MaximumDiscountAmount { get; set; } //Số tiền giảm tối đa
    // public string MinimumOrderAmount { get; set; } //Khoản tiền tối thiểu để sử dụng voucher
    // public string VoucherConditions { get; set; } //Điều kiện được nhận voucher dựa theo chi tiêu của khách hàng.
    // public DateTime StartDate { get; set; } //Ngày bắt đầu áp dụng voucher
    // public DateTime ExpiredDate { get; set; } //Ngày hết hạn của voucher
    // public string? Description { get; set; } //Mô tả về voucher
    // public string Status { get; set; } //Trạng thái của voucher
    public CreateVoucherCommandValidator(IVoucherRepository voucherRepository)
    {
        RuleFor(a => a.VoucherName)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters")
            .Must(a => voucherRepository.IsVoucherNameExists(a).Result == false)
            .WithMessage("{PropertyName} already exists");

        RuleFor(a => a.VoucherCode)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} is required")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters")
            .Must(a => !voucherRepository.IsVoucherCodeExists(a).Result)
            .WithMessage("{PropertyName} already exists");

        RuleFor(a => a.PercentageDiscount)
            .Must(p => p == null || int.TryParse(p, out _))
            .WithMessage("{PropertyName} must be an integer.")
            .Must(p => p != null && int.Parse(p) >= 0 && int.Parse(p) <= 100)
            .WithMessage("{PropertyName} must be between 0 and 100.")
            .When(p => !string.IsNullOrEmpty(p.PercentageDiscount));

        

        RuleFor(a => a.MaximumDiscountAmount)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p, out _))
            .WithMessage("{PropertyName} must be a decimal.");
        
        RuleFor(a => a.MinimumOrderAmount)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p, out _))
            .WithMessage("{PropertyName} must be a decimal.");

        RuleFor(a => a.VoucherConditions)
            .Must(p => decimal.TryParse(p, out _))
            .WithMessage("{PropertyName} must be a decimal.")
            .Must(a => a != null && decimal.Parse(a) >= 0)
            .WithMessage("{PropertyName} must be greater than or equal to 0.")
            .When(p => !string.IsNullOrEmpty(p.VoucherConditions)); 
            
        RuleFor(a => a.StartDate)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(p => p > DateTime.UtcNow)
            .WithMessage("{PropertyName} must be greater than current date.");

        RuleFor(a => a.ExpiredDate)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must((a, b) => b > a.StartDate)
            .WithMessage("{PropertyName} must be greater than StartDate.");

        RuleFor(a => a.Description)
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.")
            .When(p => !string.IsNullOrEmpty(p.Description));

    }

    #region Old CreateVoucherCommandValidator
    // public CreateVoucherCommandValidator(IVoucherRepository voucherRepository)
    // {
    //     RuleFor(a => a.VoucherName)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required")
    //         .MaximumLength(50)
    //         .WithMessage("{PropertyName} must not exceed 50 characters")
    //         .Must(a => voucherRepository.IsVoucherNameExists(a).Result == false)
    //         .WithMessage("{PropertyName} already exists");

    //     RuleFor(a => a.MaxDiscount)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required.")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required.")
    //         .Must(p => decimal.TryParse(p.ToString(), out _))
    //         .WithMessage("{PropertyName} must be a decimal.");

    //     RuleFor(a => a.VoucherCondition)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required.")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required.")
    //         .Must(p => decimal.TryParse(p.ToString(), out _))
    //         .WithMessage("{PropertyName} must be a decimal.");

    //     RuleFor(a => a.StartDate)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required.")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required.")
    //         .Must(p => p > DateTime.UtcNow)
    //         .WithMessage("{PropertyName} must be greater than current date.");

    //     RuleFor(a => a.ExpiredDate)
    //         .NotNull()
    //         .WithMessage("{PropertyName} is required.")
    //         .NotEmpty()
    //         .WithMessage("{PropertyName} is required.")
    //         .Must((a, b) => b > a.StartDate)
    //         .WithMessage("{PropertyName} must be greater than StartDate.");
    // }
    #endregion
}
