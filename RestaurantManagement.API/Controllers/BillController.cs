using MediatR;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.BillFeature.Queries.GetAllBill;
using RestaurantManagement.Application.Features.BillFeature.Queries.GetAllBillByUserId;
using RestaurantManagement.Application.Features.BillFeature.Queries.GetBillById;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Authentication;


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
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            });

            //lấy bill theo bill id
            endpoints.MapGet("{id}", async (
                string id,
                ISender sender) =>
            {
                var query = new GetBillByIdQuery(id);
                var result = await sender.Send(query);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);
            });

            endpoints.MapGet("bill-user", async (
                // [FromQuery] string? searchTerm,
                [FromQuery] string? sortColumn,
                [FromQuery] string? sortOrder,
                [FromQuery] int? page,
                [FromQuery] int? pageSize,
                ISender sender,
                HttpContext httpContext,
                IJwtProvider jwtProvider) =>
            {
                //lấy token
                var token = jwtProvider.GetTokenFromHeader(httpContext);
                var query = new GetAllBillByUserIdQuery(
                    token, 
                    // searchTerm, 
                    sortColumn, 
                    sortOrder, 
                    page, 
                    pageSize);
                var result = await sender.Send(query);
                if (result.IsSuccess)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result);

            }).RequireAuthorization();

            //xuất bill pdf
            endpoints.MapGet("bill-export/{id}", async (
                string id,
                ISender sender) =>
            {
                var result = await sender.Send(new GetBillByIdQuery(id));
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }
                var html = await RazorTemplateEngine.RenderAsync("Views/BillReport.cshtml", result.Value);
                var renderer = new ChromePdfRenderer();

                using var pdfDocument = renderer.RenderHtmlAsPdf(html);

                return Results.File(pdfDocument.BinaryData, "application/pdf", "bill.pdf");
            }).RequireAuthorization();

        }
    }
}
