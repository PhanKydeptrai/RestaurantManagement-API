using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;

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
        if (!await ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await _tableRepository.UpdateActiveStatus(request.id, "Occupied");
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
