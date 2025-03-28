using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CustomerCreateBooking;

//NOTE Xử lý untrusted data
public class CustomerCreateBookingCommand : ICommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly BookingTime { get; set; }
    public object NumberOfCustomers { get; set; }
    public string? Note { get; set; }
}

