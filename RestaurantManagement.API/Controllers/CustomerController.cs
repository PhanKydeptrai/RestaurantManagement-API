using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.API.Extentions;
using RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;
using RestaurantManagement.Domain.IRepos;


namespace RestaurantManagement.API.Controllers
{
    public class CustomerController : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var endpoints = app.MapGroup("api/customer").WithTags("Customer").DisableAntiforgery();

            // Get all with pagination
            endpoints.MapGet("",
            async (
                ISender sender,
                [FromQuery] string? seachTerm,
                [FromQuery] int page,
                [FromQuery] int pageSize,
                [FromQuery] string? sortColumn,
                [FromQuery] string? sortOrder) =>
            {
                CustomerFilterQuery request = new CustomerFilterQuery(seachTerm, sortColumn, sortOrder, page, pageSize);
                var result = await sender.Send(request);
                if (result != null)
                {
                    return Results.Ok(result);
                }
                return Results.BadRequest(result.ToProblemDetails());
            });

            //Get by id
            endpoints.MapGet("{id}", async (Ulid id, ISender sender) =>
            {
                GetCustomerByIdQuery request = new GetCustomerByIdQuery(id);
                var result = await sender.Send(request);
                return Results.Ok(result);
            });

            //Update information for a customer
            endpoints.MapPut("{id}",
            async (
                Ulid id,
                [FromForm] IFormFile? image,
                [FromForm] string? FirstName,
                [FromForm] string? LastName,
                [FromForm] string? PhoneNumber,
                [FromForm] string? Gender,
                ISender sender,
                HttpContext httpContext,
                IJwtProvider jwtProvider) =>
            {
                //Xử lý ảnh
                DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
                Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
                cloudinary.Api.Secure = true;

                var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(image.FileName, memoryStream),
                    UploadPreset = "iiwd8tcu"
                };

                var resultUpload = await cloudinary.UploadAsync(uploadParams);
                Console.WriteLine(resultUpload.JsonObj);

                var token = jwtProvider.GetTokenFromHeader(httpContext);


                var updateCustomerCommand = new UpdateCustomerInformationCommand(
                    id,
                    FirstName,
                    LastName,
                    PhoneNumber,
                    resultUpload.SecureUrl.ToString(),
                    Gender,
                    token
                );

                var result = await sender.Send(updateCustomerCommand);
                if (result.IsSuccess)
                {
                    return Results.Ok("Customer updated successfully!");
                }
                return Results.BadRequest(result.Errors);

            }).RequireAuthorization();
        }
    }

}
