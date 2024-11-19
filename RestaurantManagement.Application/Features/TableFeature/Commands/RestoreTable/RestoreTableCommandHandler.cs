using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public class RestoreTableCommandHandler(
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository) : ICommandHandler<RestoreTableCommand>
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
        //TODO: Cập nhật system log
        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);
        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Khôi phục trạng thái bàn {request.id} ",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        return Result.Success();
    }
}
