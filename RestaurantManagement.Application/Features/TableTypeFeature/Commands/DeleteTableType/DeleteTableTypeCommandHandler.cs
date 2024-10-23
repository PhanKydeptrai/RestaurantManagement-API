using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;

public class DeleteTableTypeCommandHandler : ICommandHandler<DeleteTableTypeCommand>
{
    private readonly ITableTypeRepository _tableTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTableTypeCommandHandler(
        ITableTypeRepository tableTypeRepository, 
        IUnitOfWork unitOfWork)
    {
        _tableTypeRepository = tableTypeRepository;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }
}
