using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeRole;

public class UpdateEmployeeRoleCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<UpdateEmployeeRoleCommand>
{
    public async Task<Result> Handle(UpdateEmployeeRoleCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UpdateEmployeeRoleCommandValidator(employeeRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //Update Employee Role
        await employeeRepository.UpdateEmployeeRole(Ulid.Parse(request.id), request.role);


        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Cập nhật thông tin trạng thái bán của {request.id} thành bán",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
