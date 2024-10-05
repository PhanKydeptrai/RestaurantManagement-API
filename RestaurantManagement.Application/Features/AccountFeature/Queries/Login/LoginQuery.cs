using MediatR;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.Login;

public record LoginQuery(string loginString, string passWord) : IRequest<Result<string>>;
