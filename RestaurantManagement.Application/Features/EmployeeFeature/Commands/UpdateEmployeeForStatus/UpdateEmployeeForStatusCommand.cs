using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeForStatus
{
    public class UpdateEmployeeForStatusCommand : ICommand
    {
        public string Id { get; set; }
        public string EmployyeStatus { get; set; }

    }
}
