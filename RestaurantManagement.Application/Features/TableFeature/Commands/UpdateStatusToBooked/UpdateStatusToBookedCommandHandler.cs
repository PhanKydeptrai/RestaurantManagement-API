using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UpdateStatusToBooked;

public class UpdateStatusToBookedCommandHandler : ICommandHandler<UpdateStatusToBookedCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;
    public UpdateStatusToBookedCommandHandler(
        ITableRepository tableRepository,
        ISystemLogRepository systemLogRepository,
        IUnitOfWork unitOfWork)
    {
        _tableRepository = tableRepository;
        _systemLogRepository = systemLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateStatusToBookedCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UpdateStatusToBookedCommandValidator(_tableRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //Update table status
        await _tableRepository.UpdateActiveStatus(request.id, "booked");

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật thông tin trạng thái bàn {request.id} thành booked",
            UserId = Ulid.Parse(userId)
        });

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
