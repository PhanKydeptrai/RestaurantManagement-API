namespace RestaurantManagement.API.Abstractions;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}
