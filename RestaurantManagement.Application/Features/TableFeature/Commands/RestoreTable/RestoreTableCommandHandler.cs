using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public class RestoreTableCommandHandler(
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository,
    ISystemLogRepository systemLogRepository) : ICommandHandler<RestoreTableCommand>
{
    public async Task<Result> Handle(RestoreTableCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new RestoreTableCommandValidator(tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
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
