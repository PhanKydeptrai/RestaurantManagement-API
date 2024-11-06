using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;

public class RestoreEmployeeCommandHandler(
    IUnitOfWork unitOfWork,
    IEmployeeRepository employeeRepository,
    IApplicationDbContext context,
    ISystemLogRepository systemLogRepository) : ICommandHandler<RestoreEmployeeCommand>
{
    public async Task<Result> Handle(RestoreEmployeeCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new RestoreEmployeeCommandValidator(employeeRepository);
        if(!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }
        //Restore employee
        await employeeRepository.RestoreEmployee(request.id);

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"{userId} restore employee {request.id}",
            UserId = Ulid.Parse(userId)
        });
        
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
