using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.SubscriberCreateBooking;


//TODO: Untrusted data
public record SubscriberCreateBookingCommand(
    DateOnly BookingDate,
    TimeOnly BookingTime,
    int NumberOfCustomers,
    string? Note,
    string token
) : ICommand;

public record SubscriberCreateBookingRequest(
    DateOnly BookingDate,
    TimeOnly BookingTime,
    int NumberOfCustomers,
    string? Note
);

