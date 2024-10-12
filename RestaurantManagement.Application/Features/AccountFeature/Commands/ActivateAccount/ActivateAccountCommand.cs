using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ActivateAccount;

public record ActivateAccountCommand(Ulid tokenId) : ICommand;
