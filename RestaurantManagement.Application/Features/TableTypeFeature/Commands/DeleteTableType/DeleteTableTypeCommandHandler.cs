using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;

public class DeleteTableTypeCommandHandler(
    ITableTypeRepository tableTypeRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<DeleteTableTypeCommand>
{
    public async Task<Result> Handle(DeleteTableTypeCommand request, CancellationToken cancellationToken)
    {
        //Validator
        //TODO: validate
        var validator = new DeleteTableTypeCommandValidator(tableTypeRepository);

        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await tableTypeRepository.DeleteTableType(Ulid.Parse(request.id));

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
        //     LogDetail = $"Xoá loại bàn {request.id} ",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
