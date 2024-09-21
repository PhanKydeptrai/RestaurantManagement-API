using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

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

        //Get all user
        //[HttpGet]   
        //public async Task<IActionResult> Customers()
        //{
        //    var result = await _sender.Send(new GetAllCustomerQuery());
            
        //    if (result.IsSuccess)
        //    {
        //        return Ok(result.ResultValue);
        //    }
        //    return NotFound("No customers found");
        //}
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand request)
        {
            var result = await _sender.Send(request);
            if (result.IsSuccess)
            {
                return Ok("Customer created successfully!");
            } 
            string[]? errorMessages = result.Errors.ToArray();
            return BadRequest(errorMessages);
        }

    }
        
}
