using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.LoginDto;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.LoginWithGoogle;

public record LoginWithGoogleQuery(string token) : IQuery<LoginResponse>;