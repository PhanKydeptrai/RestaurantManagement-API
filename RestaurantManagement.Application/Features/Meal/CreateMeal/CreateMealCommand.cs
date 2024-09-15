using MediatR;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.Meal.CreateMeal;

public class CreateMealCommand : IRequest<Result<bool>>
{
    public string MealName { get; set; }
    public decimal Price { get; set; }
    public byte[]? Image { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
}
