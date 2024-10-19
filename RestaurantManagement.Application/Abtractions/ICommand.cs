using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Abtractions;

//Trừu tượng hóa IRequest để sử dụng Result model (Không có tham số trả về)
public interface ICommand : IRequest<Result>
{

}

//Trừu tượng hóa IRequest để sử dụng Result model (Có tham số trả về)
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{

}