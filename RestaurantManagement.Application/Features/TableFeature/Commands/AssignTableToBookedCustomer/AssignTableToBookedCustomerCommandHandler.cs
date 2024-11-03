using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;

public class AssignTableToBookedCustomerCommandHandler : ICommandHandler<AssignTableToBookedCustomerCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignTableToBookedCustomerCommandHandler(
        ITableRepository tableRepository, 
        IUnitOfWork unitOfWork)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignTableToBookedCustomerCommand request, CancellationToken cancellationToken)
    {
        
         //validate
        var validator = new AssignTableToBookedCustomerCommandValidator(_tableRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        await _tableRepository.UpdateActiveStatus(request.tableId, "Occupied");
        await _unitOfWork.SaveChangesAsync  ();
        return Result.Success();
    }
}
