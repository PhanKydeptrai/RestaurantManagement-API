using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.RestoreCustomer;

public record RestoreCustomerCommand(string userId, string token) : ICommand;
