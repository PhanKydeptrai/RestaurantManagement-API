using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;

public class RestoreEmployeeCommandHandler : ICommandHandler<RestoreEmployeeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IApplicationDbContext _context;

    public RestoreEmployeeCommandHandler(
        IUnitOfWork unitOfWork,
        IEmployeeRepository employeeRepository,
        IApplicationDbContext context,
        ISystemLogRepository systemLogRepository)
    {
        _unitOfWork = unitOfWork;
        _employeeRepository = employeeRepository;
        _context = context;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(RestoreEmployeeCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new RestoreEmployeeCommandValidator(_employeeRepository);
        if(!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }
        //Restore employee
        await _employeeRepository.RestoreEmployee(request.id);

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"{userId} restore employee {request.id}",
            UserId = Ulid.Parse(userId)
        });
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
