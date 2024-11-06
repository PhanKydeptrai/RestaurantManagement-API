using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;

public class AssignTableToBookedCustomerCommandHandler : ICommandHandler<AssignTableToBookedCustomerCommand>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;

    public AssignTableToBookedCustomerCommandHandler(
        ITableRepository tableRepository,
        IUnitOfWork unitOfWork,
        IApplicationDbContext context)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<Result> Handle(AssignTableToBookedCustomerCommand request, CancellationToken cancellationToken)
    {
        
         //validate
        var validator = new AssignTableToBookedCustomerCommandValidator(_tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //lấy thông tin booking

        var booking = await _context.Bookings.Include(a => a.BookingDetails)
            .Where(a =>  a.BookingDetails.Any(b => b.TableId == request.tableId && a.BookingStatus == "Seated"))
            .FirstOrDefaultAsync();
            
            booking.BookingStatus = "Occupied";

        await _tableRepository.UpdateActiveStatus(request.tableId, "Occupied");
        await _unitOfWork.SaveChangesAsync  ();
        return Result.Success();
    }
}
