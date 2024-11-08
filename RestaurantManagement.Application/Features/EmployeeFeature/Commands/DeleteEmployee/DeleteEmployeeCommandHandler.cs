using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(
    IUnitOfWork unitOfWork,
    IEmployeeRepository employeeRepository,
    ISystemLogRepository systemLogRepository) : ICommandHandler<DeleteEmployeeCommand>
{
    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new DeleteEmployeeCommandValidator(employeeRepository);
        if(!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"{userId} deleted employee {request.id}",
            UserId = Ulid.Parse(userId)
        });

        //Delete employee
        await employeeRepository.DeleteEmployee(Ulid.Parse(request.id));

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
