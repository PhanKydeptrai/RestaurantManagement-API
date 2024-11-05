using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;

public class PayOrderCommandHandler : ICommandHandler<PayOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayOrderCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ITableRepository tableRepository)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _tableRepository = tableRepository;
    }

    public async Task<Result> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new PayOrderCommandValidator(_tableRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }

        //Kiểm tra xem bàn đã có order chưa
       
        var order = await _context.Tables
            .Include(a => a.Orders)
            .Where(a => a.TableId == request.tableId)
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();
        
        if(order == null)
        {
            var errors = new[] { new Error("Order", "Table does not have any order.") };
            return Result.Failure(errors);
        }

        //Thanh toán order
        order.PaymentStatus = "Paid";
        await _tableRepository.UpdateActiveStatus(request.tableId, "Empty");

        //Kiểm tra bàn có booking hay không
        var checkBooking = await _context.Tables.Include(a => a.BookingDetails)
            .Where(a => a.TableId == request.tableId)
            .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
            .FirstOrDefaultAsync();

        checkBooking.Booking.BookingStatus = "Completed";
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
