using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotPassword;

public record ForgotCustomerPasswordCommand(string email) : ICommand;
