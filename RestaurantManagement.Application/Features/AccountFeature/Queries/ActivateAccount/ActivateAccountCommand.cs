using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.ActivateAccount;

public record ActivateAccountCommand(Ulid tokenId) : ICommand;
