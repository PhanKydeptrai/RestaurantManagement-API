using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.DeleteCustomerAccount;

public record DeleteCustomerAccountCommand(string token) : ICommand;