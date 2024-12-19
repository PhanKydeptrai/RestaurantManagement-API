using FluentValidation;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Extentions;

public static class ValidateRequest
{
    public static bool RequestValidator<T>(IValidator<T> validator, T request, out Error[] errors)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();
            return false;
        }
        errors = null!;
        return true;
    }
}
