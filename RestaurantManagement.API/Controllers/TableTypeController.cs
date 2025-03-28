﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Authentication;
using RestaurantManagement.Application.Features.TableTypeFeature.Commands.CreateTableType;
using RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;
using RestaurantManagement.Application.Features.TableTypeFeature.Commands.RestoreTableType;
using RestaurantManagement.Application.Features.TableTypeFeature.Commands.UpdateTableType;
using RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableType;
using RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableTypeInfo;
using RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetTableTypeById;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class TableTypeController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var endpoints = builder.MapGroup("api/tabletype")
            .WithTags("TableType")
            .DisableAntiforgery();

        endpoints.MapGet("",
        async (
            [FromQuery] string? searchTerm,
            [FromQuery] string? filterStatus,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetAllTableTypeQuery(searchTerm, filterStatus, sortColumn, sortOrder, page, pageSize));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result.Errors);
        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Create table type
        endpoints.MapPost("", async (
            [FromForm] string TableTypeName,
            [FromForm] string TableCapacity,
            [FromForm] string TablePrice,
            [FromForm] string? Description,
            [FromForm] IFormFile? Image,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            string token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new CreateTableTypeCommand(TableTypeName, Image, TablePrice, TableCapacity, Description, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamCreateTableTypeCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Update table type
        endpoints.MapPut("{id}", async (
            string id,
            [FromForm] string TableTypeName,
            [FromForm] string TablePrice,
            [FromForm] string? Description,
            [FromForm] string TableCapacity,
            [FromForm] IFormFile? Image,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            string token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new UpdateTableTypeCommand(
                id,
                TableTypeName,
                Image,
                TablePrice,
                TableCapacity,
                Description,
                token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamUpdateTableTypeCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();


        //Get table type by id
        endpoints.MapGet("{id}",
        async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new GetTableTypyByIdQuery(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        }).AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Delete table type by id
        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender,
            HttpContext httpContext,
            IJwtProvider jwtProvider) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new DeleteTableTypeCommand(id, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamDeleteTableTypeCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();

        //Restore table type by id
        endpoints.MapPut("restore/{id}", async (
            string id,
            ISender sender) =>
        {
            var result = await sender.Send(new RestoreTableTypeCommand(id));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);

        })
        .RequireAuthorization("boss")
        .RequireRateLimiting("AntiSpamRestoreTableTypeCommand")
        .AddEndpointFilter<ApiKeyAuthenticationEndpointFilter>();
        
        endpoints.MapGet("tabletype-info", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAllTableInfoQuery());
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result.Errors);
        });
    }
}