using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToCustomer;

public class UnAssignTableToCustomerCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UnAssignTableToCustomerCommand>
{
    public async Task<Result> Handle(UnAssignTableToCustomerCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UnAssignTableToCustomerCommandValidator(tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await tableRepository.UpdateActiveStatus(int.Parse(request.id), "Empty");
        
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
