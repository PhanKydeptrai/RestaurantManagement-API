using FluentValidation;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(a => a.loginString)
            .NotEmpty().WithMessage("Phone number or Email is required.")
            .NotNull().WithMessage("Phone number or Email is required.")
            .Custom((loginString, context) =>
            {
                if (loginString.Any(char.IsLetter))
                {
                    var emailValidator = new InlineValidator<string>();
                    emailValidator.RuleFor(email => email).EmailAddress().WithMessage("Email is invalid.");

                    var validationResult = emailValidator.Validate(loginString);
                    if (!validationResult.IsValid)
                    {
                        context.AddFailure("Email is invalid.");
                    }
                }
                else
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(loginString, @"^0\d{9}$"))
                    {
                        context.AddFailure("PhoneNumber must start with 0 and be 10 digits long.");
                    }
                }
            }).When(a => !string.IsNullOrEmpty(a.loginString));

        RuleFor(a => a.passWord)
            .NotEmpty().NotNull()
            .WithMessage("Password is required.");
    }
}