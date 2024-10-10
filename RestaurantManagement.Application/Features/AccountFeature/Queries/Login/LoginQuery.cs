using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

public record LoginQuery(string loginString, string passWord) : ICommand<string>;
