using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.RestoreTableType;

public class RestoreTableTypeCommandHandler(
    ITableTypeRepository tableTypeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RestoreTableTypeCommand>
{
    public async Task<Result> Handle(RestoreTableTypeCommand request, CancellationToken cancellationToken)
    {
        //Validator
        var validator = new RestoreTableTypeCommandValidator(tableTypeRepository);  
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        await tableTypeRepository.RestoreTableType(Ulid.Parse(request.id));
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
