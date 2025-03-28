﻿using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;
public class RestoreMealCommandHandler(
    IMealRepository mealRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<RestoreMealCommand>
{
    public async Task<Result> Handle(RestoreMealCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new RestoreMealCommandValidator(mealRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        
        await mealRepository.RestoreMeal(Ulid.Parse(request.id));

        #region Decode jwt and system log
        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));

        //Create System Log
        await context.MealLogs.AddAsync(new MealLog
        {
            MealLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} cập nhật thông tin món {request.id}",
            UserId = Ulid.Parse(userId)
        });
        #endregion
        

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}

