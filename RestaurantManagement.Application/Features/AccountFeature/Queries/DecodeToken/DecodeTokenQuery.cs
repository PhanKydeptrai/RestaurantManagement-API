using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.DecodeToken;

public record DecodeTokenQuery(string token) : IQuery<string>;