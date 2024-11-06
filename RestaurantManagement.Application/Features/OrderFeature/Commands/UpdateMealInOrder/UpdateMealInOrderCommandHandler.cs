using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;

public class UpdateMealInOrderCommandHandler : ICommandHandler<UpdateMealInOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMealInOrderCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IOrderDetailRepository orderDetailRepository)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _orderDetailRepository = orderDetailRepository;
    }

    public async Task<Result> Handle(UpdateMealInOrderCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UpdateMealInOrderCommandValidator(_orderDetailRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //Update order detail
        var orderDetail = await _context.OrderDetails
            .Include(a => a.Order)
            .Include(a => a.Meal)
            .FirstOrDefaultAsync(a => a.OrderDetailId == request.OrderDetailId);

        orderDetail.Quantity = request.Quantity;

        orderDetail.Order.Total = orderDetail.Order.Total - orderDetail.UnitPrice;  
    
        orderDetail.UnitPrice = orderDetail.Meal.Price * request.Quantity;

        orderDetail.Order.Total += orderDetail.UnitPrice;
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
