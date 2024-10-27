using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;
using RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeRole;
using RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetAllEmployee;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.API.Controllers;

public class EmployeeController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/employee").WithTags("Employee").DisableAntiforgery();

        //Update information for employee
        endpoints.MapPut("{id}",
        async (
            Ulid id,
            [FromForm] string FirstName,
            [FromForm] string LastName,
            [FromForm] string PhoneNumber,
            [FromForm] IFormFile? UserImage,
            HttpContext httpContext,
            ISender sender,
            IJwtProvider jwtProvider) =>
        {
            //Xử lý ảnh
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            var memoryStream = new MemoryStream();
            await UserImage.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(UserImage.FileName, memoryStream),
                UploadPreset = "iiwd8tcu"
            };

            var resultUpload = await cloudinary.UploadAsync(uploadParams);
            Console.WriteLine(resultUpload.JsonObj);
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);


            var result = await sender.Send(
                new UpdateEmployeeInformationCommand(
                    id,
                    FirstName,
                    LastName,
                    PhoneNumber,
                    resultUpload.SecureUrl.ToString(),
                    token));

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization();

        //Create employee
        endpoints.MapPost("",
        async (
            [FromForm] string FirstName,
            [FromForm] string LastName,
            [FromForm] string PhoneNumber,
            [FromForm] string Email,
            [FromForm] string Role,
            [FromForm] string Gender,
            [FromForm] IFormFile? UserImage,
            ISender sender) =>
        {
            //Xử lý ảnh
            string imageUrl = string.Empty;
            if (UserImage != null)
            {
                DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
                Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
                cloudinary.Api.Secure = true;

                var memoryStream = new MemoryStream();
                await UserImage.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(UserImage.FileName, memoryStream),
                    UploadPreset = "iiwd8tcu"
                };

                var resultUpload = await cloudinary.UploadAsync(uploadParams);
                imageUrl = resultUpload.SecureUrl.ToString();
                Console.WriteLine(resultUpload.JsonObj);
            }

            var result = await sender.Send(
                new CreateEmployeeCommand(
                    FirstName,
                    LastName,
                    PhoneNumber,
                    Email,
                    imageUrl,
                    Role,
                    Gender));

            if (!result.IsSuccess)
            {
                return Results.BadRequest(result);
            }
            return Results.Ok(result);

        });



        //Get all
        endpoints.MapGet("",
        async (
            [FromQuery] string? filterGender,
            [FromQuery] string? filterRole,
            [FromQuery] string? filterStatus,
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortColumn,
            [FromQuery] string? sortOrder,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetAllEmployeeQuery(filterGender, filterRole, filterStatus, searchTerm, sortColumn, sortOrder, page, pageSize));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        });

        //Delete employee
        endpoints.MapDelete("{id}", async (
            string id,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new DeleteEmployeeCommand(Ulid.Parse(id), token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }).RequireAuthorization("management");

        //restore employee
        endpoints.MapPut("restore-employee/{id}", async (
            string id,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);
            var result = await sender.Send(new RestoreEmployeeCommand(Ulid.Parse(id), token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
            
        }).RequireAuthorization("management");

        //restore employee
        endpoints.MapPut("employee-role/{id}", async (
            string id,
            [FromForm] string role,
            ISender sender,
            IJwtProvider jwtProvider,
            HttpContext httpContext) =>
        {
            //lấy token
            var token = jwtProvider.GetTokenFromHeader(httpContext);

            var result = await sender.Send(new UpdateEmployeeRoleCommand(Ulid.Parse(id), role, token));
            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
            
        }).RequireAuthorization("management");
    }
}
