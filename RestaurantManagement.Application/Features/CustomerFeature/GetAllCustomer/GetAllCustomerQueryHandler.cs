using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CustomerFeature.GetAllCustomer;

// public class GetAllCustomerQueryHandler : IRequestHandler<GetAllCustomerQuery, Result<List<CustomerInformation>>>
// {
//     private readonly ICustomerRepository _customerRepository;
//     private readonly IApplicationDbContext _context;
//     public GetAllCustomerQueryHandler(ICustomerRepository customerRepository, IApplicationDbContext context)
//     {
//         _customerRepository = customerRepository;
//         _context = context;
//     }
//     Task<Result<List<CustomerInformation>>> IRequestHandler<GetAllCustomerQuery, Result<List<CustomerInformation>>>.Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
//     {
//         Result<List<CustomerInformation>> result = new Result<List<CustomerInformation>>
//         {
//             ResultValue = new List<CustomerInformation>(),
//             IsSuccess = false,
//             Errors = new List<string>()
//         };

//         IQueryable query = _context.Customers
//                                 .Include(x => x.User)
//                                 .Select(p => p.User.Customer)
//                                 .AsQueryable();
        
//         return result;
//     }
// }
