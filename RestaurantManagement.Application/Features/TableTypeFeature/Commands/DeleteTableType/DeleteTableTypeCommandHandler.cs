using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;

public class DeleteTableTypeCommandHandler(
    ITableTypeRepository tableTypeRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<DeleteTableTypeCommand>
{
    public async Task<Result> Handle(DeleteTableTypeCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new DeleteTableTypeCommandValidator(tableTypeRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        await tableTypeRepository.DeleteTableType(Ulid.Parse(request.id));

        #region Decode jwt and system log
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        //Create System Log
        await context.TableTypeLogs.AddAsync(new TableTypeLog
        {
            TableTypeLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} xoá loại bàn {request.id} ",
            UserId = Ulid.Parse(userId)
        });
        #endregion

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
