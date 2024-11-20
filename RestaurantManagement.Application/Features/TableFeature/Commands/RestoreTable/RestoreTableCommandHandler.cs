using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public class RestoreTableCommandHandler(
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository,
    IApplicationDbContext context) : ICommandHandler<RestoreTableCommand>
{
    public async Task<Result> Handle(RestoreTableCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new RestoreTableCommandValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //restore table
        await tableRepository.RestoreTable(int.Parse(request.id));
        
        #region Decode jwt and system log
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        //Create System Log
        await context.TableLogs.AddAsync(new TableLog
        {
            TableLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"Khôi phục trạng thái bàn {request.id}",
            UserId = Ulid.Parse(userId)
        });
        #endregion
        
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
