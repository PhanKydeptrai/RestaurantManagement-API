using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.LoginDto;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.EmployeeLogin;

public record EmployeeLoginQuery(string loginString, string passWord) : IQuery<LoginResponse>;

