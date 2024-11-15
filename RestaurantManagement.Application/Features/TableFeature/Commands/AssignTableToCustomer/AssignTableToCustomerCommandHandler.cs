using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;

public class GetTableForCustomerCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AssignTableToCustomerCommand>
{
    public async Task<Result> Handle(AssignTableToCustomerCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new AssignTableToCustomerCommandValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        await tableRepository.UpdateActiveStatus(int.Parse(request.id), "Occupied");
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
