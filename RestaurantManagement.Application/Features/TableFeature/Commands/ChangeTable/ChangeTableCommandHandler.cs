using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.ChangeTable;

public class ChangeTableCommandHandler : ICommandHandler<ChangeTableCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;

    public ChangeTableCommandHandler(
        ITableRepository tableRepository,
        IUnitOfWork unitOfWork,
        IApplicationDbContext context)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<Result> Handle(ChangeTableCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new ChangeTableCommandValidator(_tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        var transactionCheck = await _context.Tables
            .Include(a => a.BookingDetails.Where(a => a.Booking.BookingStatus == "Occupied"))
            .Include(a => a.Orders)
            .ThenInclude(a => a.OrderTransaction)
            .Where(a => a.TableId == int.Parse(request.oldtableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        //TODO: Đổi lại message và mang vào validator
        if(transactionCheck.OrderTransaction == null)
        {
            return Result.Failure(new[] { new Error("Transaction", "Bàn này đang thanh toán hong có dời được đâu.") });
        }

        //Kiểm tra bàn có booking hay không
        BookingDetail? checkBooking = await _context.Tables 
            .Include(a => a.BookingDetails)
            .ThenInclude(a => a.Booking)
            .Where(a => a.TableId == int.Parse(request.oldtableId))
            .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
            .FirstOrDefaultAsync();

        
        

        if(checkBooking != null)
        {
            checkBooking.TableId = int.Parse(request.newTableId.ToString());
        }
        
        //lấy order của bàn cũ
        var order = await _context.Tables
            .Include(a => a.Orders)
            .Where(a => a.TableId == int.Parse(request.oldtableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        //cập nhật order sang bàn mới
        order.TableId = int.Parse(request.newTableId.ToString());
        //cập nhật trạng thái bàn
        var oldTable = await _context.Tables.FindAsync(int.Parse(request.oldtableId));
        var newTable = await _context.Tables.FindAsync(int.Parse(request.newTableId.ToString()));
        

        oldTable.ActiveStatus = "Empty";
        newTable.ActiveStatus = "Occupied";

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
