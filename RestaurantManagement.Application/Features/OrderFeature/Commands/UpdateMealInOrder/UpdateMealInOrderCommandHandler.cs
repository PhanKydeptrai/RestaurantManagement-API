using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;

public class UpdateMealInOrderCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    IOrderDetailRepository orderDetailRepository) : ICommandHandler<UpdateMealInOrderCommand>
{
    public async Task<Result> Handle(UpdateMealInOrderCommand request, CancellationToken cancellationToken)
    {
    
        //Validate request
        var validator = new UpdateMealInOrderCommandValidator(orderDetailRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Update order detail
        var orderDetail = await context.OrderDetails
            .Include(a => a.Order)
            .Include(a => a.Meal)
            .FirstOrDefaultAsync(a => a.OrderDetailId == Ulid.Parse(request.OrderDetailId));

        orderDetail.Quantity = int.Parse(request.Quantity.ToString());

        orderDetail.Order.Total = orderDetail.Order.Total - orderDetail.UnitPrice;  
    
        orderDetail.UnitPrice = orderDetail.Meal.Price * int.Parse(request.Quantity.ToString());

        orderDetail.Order.Total += orderDetail.UnitPrice;
        
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
