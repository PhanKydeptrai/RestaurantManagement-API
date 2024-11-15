using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;

    
public record TableArrangementCommand(
    string BookingId,
    object TableId) : ICommand;

public record TableArrangementRequest(
    object TableId);

#region Stable code
// public record TableArrangementCommand(
//     string BookingId,
//     string TableId) : ICommand;

// public record TableArrangementRequest(
//     string TableId);
#endregion