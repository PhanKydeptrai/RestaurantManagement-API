using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeRole;

public record UpdateEmployeeRoleCommand(Ulid id, string role, string token) : ICommand;
