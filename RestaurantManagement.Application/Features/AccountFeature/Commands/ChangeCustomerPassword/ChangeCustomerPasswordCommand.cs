using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;

public record ChangeCustomerPasswordCommand(string oldPass, string newPass, string token) : ICommand;

public record ChangeCustomerPasswordRequest(string oldPass, string newPass);
