﻿using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.MealDto;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetMealById;

public record GetMealByIdQuery(Ulid id) : IQuery<MealResponse>;
