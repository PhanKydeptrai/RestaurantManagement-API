using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.LoginDto;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

public record LoginQuery(string loginString, string passWord) : ICommand<LoginResponse>;
