using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToBookedCustomerCommand;

public class UnAssignTableToBookedCustomerCommandHandler : ICommandHandler<UnAssignTableToBookedCustomerCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnAssignTableToBookedCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        ITableRepository tableRepository)
    {
        _unitOfWork = unitOfWork;
        _tableRepository = tableRepository;
    }

    public async Task<Result> Handle(UnAssignTableToBookedCustomerCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UnAssignTableToBookedCustomerCommandValidator(_tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await _tableRepository.UpdateActiveStatus(int.Parse(request.id), "Empty");
        
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
