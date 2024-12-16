using Microsoft.EntityFrameworkCore;
using Quartz;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Infrastructure.BackgroundJob;

public class UpdateTableStatusForBooking : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTableStatusForBooking(
        IUnitOfWork unitOfWork, 
        IApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        //Kiểm tra toàn bộ booking. 
        Booking[]? bookings = await _context.Bookings.Include(a => a.BookingDetails)
            .Where(a => a.BookingStatus == "Seated" && a.BookingDate == DateOnly.FromDateTime(DateTime.Now))
            .ToArrayAsync();
        
        if(bookings.Length > 0)
        {
            foreach (var booking in bookings)
            {
                foreach (var bookingDetail in booking.BookingDetails)
                {
                    var table = await _context.Tables.FirstOrDefaultAsync(a => a.TableId == bookingDetail.TableId);
                    table.ActiveStatus = "Booked";

                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
        Console.WriteLine("UpdateTableStatusForBooking background job is running");

    }
}
