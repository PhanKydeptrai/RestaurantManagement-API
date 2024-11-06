using FluentValidation;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Extentions;

public static class ValidateRequest
{
    public static async Task<bool> RequestValidator<T>(IValidator<T> validator, T request, out Error[] errors)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();
            return false;
        }
        errors = null;
        return true;
    }
}
