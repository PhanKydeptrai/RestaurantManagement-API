using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.VerifyChangeCustomerPassword;

public record VerifyChangeCustomerPasswordCommand(Ulid tokenId) : ICommand;
