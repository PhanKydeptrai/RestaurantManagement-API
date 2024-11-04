using System.Data;
using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(
        ITableRepository tableRepository,
        IMealRepository mealRepository)
    {
        RuleFor(a => a.Quantity)
            .NotNull()
            .WithMessage("Quantity is required")
            .NotEmpty()
            .WithMessage("Quantity is required")
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThan(100)
            .WithMessage("Quantity must be less than 100");

        RuleFor(a => a.TableId)
            .NotNull()
            .WithMessage("TableId is required")
            .NotEmpty()
            .WithMessage("TableId is required")
            .Must(a => tableRepository.IsTableExist(a).Result == true)
            .WithMessage("Table is not exist")
            .Must(a => tableRepository.GetActiveStatus(a).Result == "Occupied")
            .WithMessage("Table is not occupied");

        RuleFor(a => a.MealId)
            .NotNull()
            .WithMessage("MealId is required")
            .NotEmpty()
            .WithMessage("MealId is required")
            .Must(a => mealRepository.IsMealExist(a).Result == true)
            .WithMessage("Meal is not exist")
            .Must(a => mealRepository.GetSellStatus(a).Result == "Active")
            .WithMessage("SellStatus is not active")
            .Must(a => mealRepository.GetMealStatus(a).Result == "Active")
            .WithMessage("MealStatus is not active");

            
    }
}
