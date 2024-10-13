using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotEmployeePassword;

public record ForgotEmployeePasswordCommand(string email) : ICommand;
