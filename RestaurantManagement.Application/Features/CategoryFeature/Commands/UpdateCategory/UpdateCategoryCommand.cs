using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public record UpdateCategoryCommand(
   string CategoryId,
   string CategoryName,
   IFormFile? Image,
   string Token) : ICommand;
