using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Features.CustomerFeature.GetAllCustomer;

namespace RestaurantManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ISender _sender;
        public CustomerController(ISender sender)
        {
            _sender = sender;   
        }

        [HttpGet]   
        public async Task<IActionResult> Customers()
        {
            var result = await _sender.Send(new GetAllCustomerQuery());
            
            if (result.IsSuccess)
            {
                return Ok(result.ResultValue);
            }
            return NotFound("No customers found");
        }

    }
}
