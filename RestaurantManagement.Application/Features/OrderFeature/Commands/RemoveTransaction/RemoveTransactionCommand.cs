using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.RemoveTransaction;

public record RemoveTransactionCommand(string tableId) : ICommand;
