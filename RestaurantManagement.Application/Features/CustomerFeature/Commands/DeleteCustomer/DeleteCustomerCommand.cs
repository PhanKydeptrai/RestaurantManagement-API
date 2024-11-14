using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.DeleteCustomer;

public record DeleteCustomerCommand(string userId) : ICommand;
