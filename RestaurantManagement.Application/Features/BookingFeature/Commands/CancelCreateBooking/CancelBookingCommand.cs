using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.CancelCreateBooking;

public record CancelBookingCommand(Ulid id) : ICommand;
