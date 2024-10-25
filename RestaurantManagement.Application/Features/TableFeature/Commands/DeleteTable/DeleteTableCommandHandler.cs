using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;

public class DeleteTableCommandHandler : ICommandHandler<DeleteTableCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;

    public DeleteTableCommandHandler(
        ITableRepository tableRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new DeleteTableCommandValidator(_tableRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //Delete table
        await _tableRepository.UpdateActiveStatus(request.id, "inactive");

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
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
