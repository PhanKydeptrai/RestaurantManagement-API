using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.RestoreTableType;

public class RestoreTableTyeCommandHandler : ICommandHandler<RestoreTableTyeCommand>
{
    private readonly ITableTypeRepository _tableTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RestoreTableTyeCommandHandler(
        ITableTypeRepository tableTypeRepository, 
        IUnitOfWork unitOfWork)
    {
        _tableTypeRepository = tableTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RestoreTableTyeCommand request, CancellationToken cancellationToken)
    {
        //Validator
        var validator = new RestoreTableTyeCommandValidator(_tableTypeRepository);  
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        await _tableTypeRepository.RestoreTableType(request.id);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
