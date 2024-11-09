using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.BillFeature.Queries.GetAllBill;
using RestaurantManagement.Application.Features.BillFeature.Queries.GetBillById;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.API.Controllers
{
    public class BillController : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("api/bill").WithTags("Bill").DisableAntiforgery();

            endpoints.MapGet("", async (
                [FromQuery] string? filterUserId,
                [FromQuery] string? searchTerm,
                [FromQuery] string? sortColumn,
                [FromQuery] string? sortOrder,
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                ISender sender) =>
            {
                var query = new GetAllBillQuery(filterUserId, searchTerm, sortColumn, sortOrder, page, pageSize);
                var result = await sender.Send(query);
                if(result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            });

            endpoints.MapGet("{id}", async (
                string id,
                ISender sender) =>
            {
                var query = new GetBillByIdQuery(id);
                var result = await sender.Send(query);
                if(result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            });

        }
    }
}
