using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.LoginDto;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.LoginWithFacebook;

public record LoginWithFacebookQuery(
    string email,
    string imageUrl,
    string userName
) : IQuery<LoginResponse>;


