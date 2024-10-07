using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Abtractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
    
}