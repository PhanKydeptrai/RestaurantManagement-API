using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

public record LoginQuery(string loginString, string passWord) : IRequest<Result<string>>;
