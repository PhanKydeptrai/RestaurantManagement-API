using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.RestoreTableType;

public class RestoreTableTypeCommandHandler(
    ITableTypeRepository tableTypeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RestoreTableTypeCommand>
{
    public async Task<Result> Handle(RestoreTableTypeCommand request, CancellationToken cancellationToken)
    {
       
        //Validate request
        var validator = new RestoreTableTypeCommandValidator(tableTypeRepository);  
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        await tableTypeRepository.RestoreTableType(Ulid.Parse(request.id));
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
