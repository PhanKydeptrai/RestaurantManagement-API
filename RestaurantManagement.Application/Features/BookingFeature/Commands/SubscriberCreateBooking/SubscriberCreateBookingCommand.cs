using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.SubscriberCreateBooking;

//NOTE: Đã lý Unstrusted data
public record SubscriberCreateBookingCommand(
    DateOnly BookingDate, //TODO: Xử lý unstrusted date
    TimeOnly BookingTime,
    object NumberOfCustomers,
    string? Note,
    string token
) : ICommand;

public record SubscriberCreateBookingRequest(
    DateOnly BookingDate,
    TimeOnly BookingTime,
    object NumberOfCustomers,
    string? Note
);


#region Stable code
// public record SubscriberCreateBookingCommand(
//     DateOnly BookingDate,
//     TimeOnly BookingTime,
//     object NumberOfCustomers,
//     string? Note,
//     string token
// ) : ICommand;

// public record SubscriberCreateBookingRequest(
//     DateOnly BookingDate,
//     TimeOnly BookingTime,
//     object NumberOfCustomers,
//     string? Note
// );
#endregion

