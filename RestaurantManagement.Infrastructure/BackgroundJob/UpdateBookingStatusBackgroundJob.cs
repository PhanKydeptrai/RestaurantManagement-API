using Microsoft.EntityFrameworkCore;
using Quartz;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Infrastructure.BackgroundJob;

public class UpdateBookingStatusBackgroundJob : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookingStatusBackgroundJob(
        IApplicationDbContext context, 
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Booking[]? bookings = await _context.Bookings
            .Where(b => b.BookingStatus == "Waiting")
            .ToArrayAsync();
        
        foreach(var booking in bookings)
        {
            if(booking.CreatedDate.AddMinutes(15) < DateTime.Now)
            {
                booking.BookingStatus = "Expired";
            }
        }

        await _unitOfWork.SaveChangesAsync();
        Console.WriteLine("UpdateBookingStatusBackgroundJob is running");
    }
}
