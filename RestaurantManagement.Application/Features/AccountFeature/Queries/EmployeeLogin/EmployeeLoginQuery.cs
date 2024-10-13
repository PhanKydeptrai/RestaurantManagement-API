using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.EmployeeLogin;

public record EmployeeLoginQuery(string loginString, string passWord) : IQuery<string>;

