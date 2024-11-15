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
        //TODO: validate
        //validate
        var validator = new UnAssignTableToBookedCustomerCommandValidator(tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await tableRepository.UpdateActiveStatus(int.Parse(request.id), "Empty");
        
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
