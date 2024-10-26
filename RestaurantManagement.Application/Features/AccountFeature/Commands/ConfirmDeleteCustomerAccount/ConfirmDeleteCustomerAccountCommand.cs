using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ConfirmDeleteCustomerAccount;

public record  ConfirmDeleteCustomerAccountCommand(Ulid tokenId) : ICommand;
