using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.BookingFeature.Commands.CustomerCreateBooking;
using RestaurantManagement.Application.Features.BookingFeature.Commands.SubscriberCreateBooking;
using RestaurantManagement.Application.Features.BookingFeature.Commands.TableArrangement;
using RestaurantManagement.Application.Features.BookingFeature.Queries.GetAllBooking;
using RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByBookingId;
using RestaurantManagement.Application.Features.BookingFeature.Queries.GetBookingByUserId;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class BookingController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/booking").WithTags("Booking").DisableAntiforgery();

        endpoints.MapGet("", async (
            [FromQuery] string? filterBookingStatus,    
            [FromQuery] string? filterPaymentStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetAllBookingQuery(
                filterBookingStatus,
                filterPaymentStatus, 
                searchTerm, 
                sortColumn, 
                sortOrder, 
                page, 
                pageSize));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });

        //Lấy thông tin booking theo booking id
        endpoints.MapGet("{id}", async (
            string id,
            ISender sender) =>
        {

            var result = await sender.Send(new GetBookingByBookingIdQuery(Ulid.Parse(id)));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.NoContent();
        });

        //Lấy thông tin booking theo user id
        endpoints.MapGet("user/{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetBookingByUserIdQuery(Ulid.Parse(id)));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });

        //Khách đặt bàn không login
        endpoints.MapPost("", async (
            [FromBody] CustomerCreateBookingCommand command,
            ISender sender) =>
        {

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });

        //Khách đặt bàn đã login
        endpoints.MapPost("subcriber", async (
            [FromBody] SubscriberCreateBookingRequest command,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {

            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new SubscriberCreateBookingCommand(
                command.BookingDate,
                command.BookingTime,
                command.NumberOfCustomers,
                command.Note,
                token
            ));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("customer");

        //Xếp bàn cho khách 
        endpoints.MapPost("table-arrange", async (
            [FromBody] TableArrangementRequest command,
            ISender sender) =>
        {
            var result = await sender.Send(new TableArrangementCommand(Ulid.Parse(command.BookingId), command.TableId));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization();


        
    }
}

