using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;

public record DeleleMealFromOrderCommand(Ulid id) : ICommand;
