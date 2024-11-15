using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;

public class DeleleMealFromOrderCommandHandler(
    IApplicationDbContext context,
    IOrderDetailRepository orderDetailRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleleMealFromOrderCommand>
{
    public async Task<Result> Handle(DeleleMealFromOrderCommand request, CancellationToken cancellationToken)
    {
        //validate
        //TODO: validate
        var validator = new DeleleMealFromOrderCommandValidator(orderDetailRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //delete
        var orderDetail = await context.OrderDetails.Include(a => a.Order).Where(a => a.OrderDetailId == Ulid.Parse(request.id)).FirstOrDefaultAsync();

        orderDetail.Order.Total = orderDetail.Order.Total - orderDetail.UnitPrice;

        context.OrderDetails.Remove(orderDetail);
        
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
