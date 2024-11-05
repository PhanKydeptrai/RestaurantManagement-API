using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.MealDto;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetMealInfo;

public record GetMealInfoQuery() : IQuery<List<MealInfo>>;
