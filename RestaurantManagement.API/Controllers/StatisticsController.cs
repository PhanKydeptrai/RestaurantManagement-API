using MediatR;
using Microsoft.VisualBasic;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.StatisticsFeature.Queries.GetStatisticsByDay;

namespace RestaurantManagement.API.Controllers;

public class StatisticsController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/orders").WithTags("Statistic").DisableAntiforgery();

        // lấy theo ngày hiện tại
        endpoints.MapGet("statistics/day", async (ISender sender) =>
        {
            var result = await sender.Send(new GetStatisticsByDayQuery());
            return Results.Ok(result);
        });

        // lấy theo tháng hiện tại
        endpoints.MapGet("statistics/month", async (ISender sender) =>
        {
            var result = await sender.Send(new GetStatisticsByDayQuery());
            return Results.Ok(result);
        });

        // lấy theo năm hiện tại
        endpoints.MapGet("statistics/year", async (ISender sender) =>
        {
            var result = await sender.Send(new ());
            return Results.Ok(result);
        });


        // // lấy theo ngày hiện tại
        // endpoints.MapGet("statistics/date/{date}", async (ISender sender) =>
        // {
        //     var result = await sender.Send(new GetStatisticsByDayQuery());
        //     return Results.Ok(result);
        // });

        // // lấy theo tháng hiện tại
        // endpoints.MapGet("statistics/month/{month}", async (ISender sender) =>
        // {
        //     var result = await sender.Send(new ());
        //     return Results.Ok(result);
        // });

        // // lấy theo năm hiện tại
        // endpoints.MapGet("statistics/year/{year}", async (ISender sender) =>
        // {
        //     var result = await sender.Send(new ());
        //     return Results.Ok(result);
        // });


        
    }
}
