using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ChangeCustomerPassword;

public record ChangePasswordCommand(string oldPass, string newPass, string token) : ICommand;

public record ChangePasswordRequest(string oldPass, string newPass);
