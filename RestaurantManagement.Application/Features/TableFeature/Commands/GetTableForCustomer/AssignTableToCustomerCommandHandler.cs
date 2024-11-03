using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.GetTableForCustomer;

public class GetTableForCustomerCommandHandler : ICommandHandler<AssignTableToCustomerCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITableRepository _tableRepository;

    public GetTableForCustomerCommandHandler(
        ITableRepository tableRepository,
        IUnitOfWork unitOfWork)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignTableToCustomerCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new AssignTableToCustomerCommandValidator(_tableRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        await _tableRepository.UpdateActiveStatus(request.id, "Occupied");
        await _unitOfWork.SaveChangesAsync  ();
        return Result.Success();
    }
}
