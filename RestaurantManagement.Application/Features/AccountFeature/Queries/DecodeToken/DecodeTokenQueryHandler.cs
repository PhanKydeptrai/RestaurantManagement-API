using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.DecodeToken;

public class DecodeTokenQueryHandler : IQueryHandler<DecodeTokenQuery, string>
{
    public async Task<Result<string>> Handle(DecodeTokenQuery request, CancellationToken cancellationToken)
    {
        //Decode jwt
        try
        {
            var claims = JwtHelper.DecodeJwt(request.token);
            claims.TryGetValue("sub", out var userId);
            return Result<string>.Success(userId);
        }
        catch
        {
            return Result<string>.Failure(new[] { new Error("token", "Invalid token") });
        }
    }
}
