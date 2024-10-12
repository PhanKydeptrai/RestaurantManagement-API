using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.ResetPasswordVerify;

public record ResetPasswordVerifyCommand(Ulid tokenId) : ICommand;

