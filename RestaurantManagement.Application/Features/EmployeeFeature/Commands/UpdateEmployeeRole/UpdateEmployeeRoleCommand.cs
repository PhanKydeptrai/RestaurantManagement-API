using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeRole;

public record UpdateEmployeeRoleCommand(string id, string role, string token) : ICommand;
