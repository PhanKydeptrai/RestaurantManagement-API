using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public record TableArrangementCommand(
    Ulid BookingId,
    int TableId) : ICommand;

public record TableArrangementRequest(
    string BookingId,
    int TableId);
