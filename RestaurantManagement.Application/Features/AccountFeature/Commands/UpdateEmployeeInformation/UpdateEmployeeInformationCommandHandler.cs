using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;

public class UpdateEmployeeInformationCommandHandler : ICommandHandler<UpdateEmployeeInformationCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ISystemLogRepository _systemLogRepository;
    public UpdateEmployeeInformationCommandHandler(
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

    public async Task<Result> Handle(UpdateEmployeeInformationCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateEmployeeInformationCommandValidator(_employeeRepository);
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);

        }

        var user = await _context.Employees
            .Where(a => a.EmployeeId == request.EmployeeId)
            .Select(a => a.User)
            .FirstAsync();

        if (user == null)
        {
            Error[] error = { new Error("Employee", "Employee not found") };
            return Result.Failure(error);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.PhoneNumber;
        user.ImageUrl = request.ImageUrl ?? user.ImageUrl;
        //Decode token to get userId
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"{userId} cập nhật thông tin tài khoản",
            UserId = Ulid.Parse(userId)
        });


        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
