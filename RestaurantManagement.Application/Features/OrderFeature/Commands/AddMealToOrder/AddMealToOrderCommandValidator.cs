using System.Data;
using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.AddMealToOrder;

public class AddMealToOrderCommandValidator : AbstractValidator<AddMealToOrderCommand>
{
    public AddMealToOrderCommandValidator(
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
            .Must(a => tableRepository.IsTableExistAndActive(int.Parse(a)).Result == true)
            .WithMessage("Table is not exist")
            .Must(a => tableRepository.GetActiveStatus(int.Parse(a)).Result == "Occupied")
            .WithMessage("Table is not occupied")
            .When(a => int.TryParse(a.TableId, out _))
            
            .NotNull()
            .WithMessage("TableId is required")
            .NotEmpty()
            .WithMessage("TableId is required")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("TableId must be a number");

        RuleFor(a => a.MealId)
            
            .Must(a => mealRepository.IsMealExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Meal is not exist")
            .Must(a => mealRepository.GetSellStatus(Ulid.Parse(a)).Result == "Active")
            .WithMessage("SellStatus is not active")
            .Must(a => mealRepository.GetMealStatus(Ulid.Parse(a)).Result == "Active")
            .WithMessage("MealStatus is not active")
            .When(a => Ulid.TryParse(a.MealId, out _))
            .NotNull()
            .WithMessage("MealId is required")
            .NotEmpty()
            .WithMessage("MealId is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("MealId must be a valid Ulid");

            
    }
}
