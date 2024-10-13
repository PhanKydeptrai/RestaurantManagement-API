using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ForgotCustomerPassword;

public record ForgotCustomerPasswordCommand(string email) : ICommand;
