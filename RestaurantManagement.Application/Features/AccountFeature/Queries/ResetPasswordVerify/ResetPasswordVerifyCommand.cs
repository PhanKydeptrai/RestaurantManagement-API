using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.ResetPasswordVerify;

public record ResetPasswordVerifyCommand(Ulid tokenId) : ICommand;

