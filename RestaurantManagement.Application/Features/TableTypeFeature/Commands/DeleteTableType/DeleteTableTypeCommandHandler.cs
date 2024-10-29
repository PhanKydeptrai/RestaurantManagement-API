using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;

public class DeleteTableTypeCommandHandler : ICommandHandler<DeleteTableTypeCommand>
{
    private readonly ITableTypeRepository _tableTypeRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTableTypeCommandHandler(
        ITableTypeRepository tableTypeRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _tableTypeRepository = tableTypeRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(DeleteTableTypeCommand request, CancellationToken cancellationToken)
    {
        //Validator
        var validator = new DeleteTableTypeCommandValidator(_tableTypeRepository);  
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        await _tableTypeRepository.DeleteTableType(request.id);

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Xoá loại bàn {request.id} ",
            UserId = Ulid.Parse(userId)
        });
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }
}
