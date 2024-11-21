using MediatR;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetStatisticsByDay;

namespace RestaurantManagement.API.Controllers;

public class StatisticsController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/orders").WithTags("Statistic").DisableAntiforgery();

        // lấy theo ngày hiện tại
        endpoints.MapGet("statistics/{datetime}", async (
            string datetime,
            ISender sender) =>
        {
            var result = await sender.Send(new GetStatisticsByDayQuery(datetime));
            return Results.Ok(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();
    }
}
