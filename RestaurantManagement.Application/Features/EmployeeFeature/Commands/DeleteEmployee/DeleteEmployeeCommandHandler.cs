using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler : ICommandHandler<DeleteEmployeeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IEmployeeRepository _employeeRepository;
    public DeleteEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        IEmployeeRepository employeeRepository,
        ISystemLogRepository systemLogRepository)
    {
        _unitOfWork = unitOfWork;
        _employeeRepository = employeeRepository;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new DeleteEmployeeCommandValidator(_employeeRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();

            return Result.Failure(errors);
        }

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"{userId} deleted employee {request.id}",
            UserId = Ulid.Parse(userId)
        });

        //Delete employee
        await _employeeRepository.DeleteEmployee(request.id);

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
