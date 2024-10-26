using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Ulid id, string token) : ICommand;
