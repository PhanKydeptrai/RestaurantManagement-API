using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.RemoveTransaction;

public class RemoveTransactionCommandHandler : ICommandHandler<RemoveTransactionCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly  ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RemoveTransactionCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork, 
        ITableRepository tableRepository)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _tableRepository = tableRepository;
    }

    public async Task<Result> Handle(RemoveTransactionCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new RemoveTransactionCommandValidator(_tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Kiểm tra xem bàn đã có order chưa
        var order = await _context.Tables
            .Include(a => a.BookingDetails.Where(a => a.Booking.BookingStatus == "Occupied"))
            .Include(a => a.Orders)
            .ThenInclude(a => a.OrderTransaction)
            .Where(a => a.TableId == int.Parse(request.tableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        if (order == null)
        {
            var error = new[] { new Error("Order", "Table does not have any order.") };
            return Result.Failure(error);
        }

        if(order.OrderTransaction == null || order.OrderTransaction.Status == "Paid")
        {
            var error = new[] { new Error("Order", "Transaction not found!.") };
            return Result.Failure(error);
        }

        //Remove transaction
        _context.OrderTransactions.Remove(order.OrderTransaction);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
