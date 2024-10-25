using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public class RestoreTableCommandHandler : ICommandHandler<RestoreTableCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly ITableRepository _tableRepository;

    public RestoreTableCommandHandler(
        IUnitOfWork unitOfWork,
        ITableRepository tableRepository,
        ISystemLogRepository systemLogRepository)
    {
        _unitOfWork = unitOfWork;
        _tableRepository = tableRepository;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(RestoreTableCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new RestoreTableCommandValidator(_tableRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //restore table


        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật thông tin trạng thái bán của {request.id} thành bán",
            UserId = Ulid.Parse(userId)
        });

        return Result.Success();
    }
}
