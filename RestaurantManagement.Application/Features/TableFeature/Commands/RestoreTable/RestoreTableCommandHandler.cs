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
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //restore table
        await _tableRepository.RestoreTable(request.id);

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Khôi phục trạng thái bàn {request.id} ",
            UserId = Ulid.Parse(userId)
        });

        return Result.Success();
    }
}
