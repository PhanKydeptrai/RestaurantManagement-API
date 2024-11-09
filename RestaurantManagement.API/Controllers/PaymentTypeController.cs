using System.Runtime.InteropServices;
using MediatR;
using RestaurantManagement.API.Abstractions;
using RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

namespace RestaurantManagement.API.Controllers;

public class PaymentTypeController : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api/paymenttype").WithTags("PaymentType").DisableAntiforgery();
        
        //Thêm loại thanh toán
        endpoints.MapPost("", async (
            CreateCategoryCommand command,
            ISender sender) =>
        {
            var result = await sender.Send(command);
            if(result.IsSuccess)
            {
                
            }
        });
    }
}
