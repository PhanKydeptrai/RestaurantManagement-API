using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeForStatus
{
    public class UpdateEmployeeForStatusCommand : IRequest<Result>
    {
        public string Id { get; set; }
        public string EmployyeStatus { get; set; }

    }
}
