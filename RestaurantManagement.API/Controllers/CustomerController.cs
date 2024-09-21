using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.GetAllCustomer;
using RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

namespace RestaurantManagement.API.Controllers
{
    [Route("api/customers")]
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
        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            GetAllCustomerQuery request = new GetAllCustomerQuery();
            var result = await _sender.Send(request);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("Not Found");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> CustomerById(Guid id)
        {
            GetCustomerByIdQuery request = new GetCustomerByIdQuery(id);
            var result = await _sender.Send(request);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest("Not Found!");
        }
        // [HttpGet("{id}")]
        // public async Task<IActionResult>
        [HttpGet("CustomerSearch")]
        public async Task<IActionResult> Customers(
            [FromQuery] string? searchTerm, 
            [FromQuery] int page, 
            [FromQuery] int pageSize)
        {
            var query = new CustomerFilterQuery(searchTerm, page, pageSize);
            var response = await _sender.Send(query);
            return Ok(response);
        }
        
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
