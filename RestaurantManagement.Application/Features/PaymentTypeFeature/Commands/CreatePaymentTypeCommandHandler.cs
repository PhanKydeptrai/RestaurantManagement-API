using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;


namespace RestaurantManagement.Application.Features.PaymentTypeFeature.Commands;

public class CreatePaymentTypeCommandHandler(
    IApplicationDbContext context,
    IPaymentTypeRepository paymentTypeRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePaymentTypeCommand>
{
    public async Task<Result> Handle(CreatePaymentTypeCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreatePaymentTypeCommandValidator(paymentTypeRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }
        var paymentType = new PaymentType
        {
            PaymentTypeId = Ulid.NewUlid(),
            Name = request.PaymentTypeName
        };
        await context.PaymentTypes.AddAsync(paymentType);
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
