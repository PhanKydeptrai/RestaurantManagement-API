using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;

public class DeleleMealFromOrderCommandHandler : ICommandHandler<DeleleMealFromOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderDetailRepository _orderDetailRepository;

    public DeleleMealFromOrderCommandHandler(
        IApplicationDbContext context,
        IOrderDetailRepository orderDetailRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _orderDetailRepository = orderDetailRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleleMealFromOrderCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new DeleleMealFromOrderCommandValidator(_orderDetailRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //delete
        var orderDetail = await _context.OrderDetails.Include(a => a.Order).Where(a => a.OrderDetailId == request.id).FirstOrDefaultAsync();

        orderDetail.Order.Total = orderDetail.Order.Total - orderDetail.UnitPrice;

        _context.OrderDetails.Remove(orderDetail);
        
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
