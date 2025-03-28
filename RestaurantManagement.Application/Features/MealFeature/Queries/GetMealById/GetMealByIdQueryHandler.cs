﻿using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.MealDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetMealById;

public class GetMealByIdQueryHandler(
    IMealRepository mealRepository,
    IApplicationDbContext context) : IQueryHandler<GetMealByIdQuery, MealResponse>
{
    public async Task<Result<MealResponse>> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
    {

        
        //Validate request
        var validator = new GetMealByIdQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<MealResponse>.Failure(errors!);
        }

        var meal = await context.Meals
            .Where(a => a.MealId == Ulid.Parse(request.id))
            .Select(a => new MealResponse(
                a.MealId,
                a.MealName,
                a.Price,
                a.ImageUrl,
                a.Description,
                a.SellStatus,
                a.MealStatus,
                a.Category.CategoryName,
                a.Category.CategoryId.ToString()))
            .FirstOrDefaultAsync();


        if (meal != null)
        {
            return Result<MealResponse>.Success(meal);
        }

        return Result<MealResponse>.Failure(new[] { new Error("Meal", "Meal not found") });

    }
}
