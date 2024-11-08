using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;

public record DeleleMealFromOrderCommand(string id) : ICommand;
