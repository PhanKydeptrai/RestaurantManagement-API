using Microsoft.EntityFrameworkCore;
using Quartz;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Infrastructure.BackgroundJob;

public class UpdateBookingStatusWhenCustomerNotCome : IJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    public UpdateBookingStatusWhenCustomerNotCome(
        IUnitOfWork unitOfWork, 
        IApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Booking[]? bookings = await _context.Bookings
            .Where(b => b.BookingStatus == "Seated")
            .ToArrayAsync();
        
        foreach(var booking in bookings)
        {
            if(booking.BookingDate == DateOnly.FromDateTime(DateTime.Now) && booking.BookingTime.AddHours(2) < TimeOnly.FromDateTime(DateTime.Now))
            {
                booking.BookingStatus = "Expired";
            }
        }

        await _unitOfWork.SaveChangesAsync();
        Console.WriteLine("UpdateBookingStatusBackgroundJob is running");
    }
}
