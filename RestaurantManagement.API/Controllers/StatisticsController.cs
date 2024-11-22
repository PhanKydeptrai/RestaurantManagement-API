using MediatR;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetAllStatisticsInOneYear;
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
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        endpoints.MapGet("statistics/year/{year}", async (
            string year,
            ISender sender) =>
        {
            var result = await sender.Send(new GetAllStatisticsInOneYearQuery(year));
            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();
    }
}
