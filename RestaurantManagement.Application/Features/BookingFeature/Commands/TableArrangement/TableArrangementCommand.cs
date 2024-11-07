using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

public record TableArrangementCommand(
    string BookingId,
    string TableId) : ICommand;

public record TableArrangementRequest(
    string TableId);
