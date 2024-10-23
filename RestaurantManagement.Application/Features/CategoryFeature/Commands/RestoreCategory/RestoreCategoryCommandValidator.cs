using System;
using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreCategory;

public class RestoreCategoryCommandValidator : AbstractValidator<RestoreCategoryCommand>
{
    public RestoreCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.id)
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => categoryRepository.IsCategoryExist(a).Result == true)
            .WithMessage("Category not found")
            .Must(a => categoryRepository.CheckStatusOfCategory(a).Result == false)
            .WithMessage("Category status is kd");
            
    }
}
