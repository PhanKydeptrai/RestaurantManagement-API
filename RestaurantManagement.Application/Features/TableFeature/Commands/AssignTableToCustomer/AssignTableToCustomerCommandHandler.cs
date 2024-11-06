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
        //validate
        var validator = new AssignTableToCustomerCommandValidator(tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await tableRepository.UpdateActiveStatus(request.id, "Occupied");
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
