using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;

public class DeleteTableCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<DeleteTableCommand>
{
    public async Task<Result> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new DeleteTableCommandValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Delete table
        await tableRepository.DeleteTable(int.Parse(request.id));

        #region Decode jwt and system log
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await context.TableLogs.AddAsync(new TableLog
        {
            TableLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"Đã xoá bàn {request.id}",
            UserId = Ulid.Parse(userId)
        });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
