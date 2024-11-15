using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToBookedCustomerCommand;

public class UnAssignTableToBookedCustomerCommandHandler(
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository) : ICommandHandler<UnAssignTableToBookedCustomerCommand>
{
    public async Task<Result> Handle(UnAssignTableToBookedCustomerCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new UnAssignTableToBookedCustomerCommandValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        await tableRepository.UpdateActiveStatus(int.Parse(request.id), "Empty");
        
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
