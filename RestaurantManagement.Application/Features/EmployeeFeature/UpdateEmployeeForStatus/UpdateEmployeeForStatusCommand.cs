using MediatR;
using RestaurantManagement.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Features.EmployeeFeature.UpdateEmployee
{
    public class UpdateEmployeeForStatusCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; }
        public string EmployyeStatus { get; set; }
        
    }
}
