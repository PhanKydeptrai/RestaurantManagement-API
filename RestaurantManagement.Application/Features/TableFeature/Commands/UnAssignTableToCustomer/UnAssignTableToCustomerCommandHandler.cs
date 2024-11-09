using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToCustomer;

public class UnAssignTableToCustomerCommandHandler : ICommandHandler<UnAssignTableToCustomerCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UnAssignTableToCustomerCommandHandler(
        ITableRepository tableRepository, 
        IUnitOfWork unitOfWork)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnAssignTableToCustomerCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UnAssignTableToCustomerCommandValidator(_tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await _tableRepository.UpdateActiveStatus(int.Parse(request.id), "Empty");
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
