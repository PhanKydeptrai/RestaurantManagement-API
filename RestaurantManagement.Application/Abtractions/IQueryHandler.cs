using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Abtractions;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
