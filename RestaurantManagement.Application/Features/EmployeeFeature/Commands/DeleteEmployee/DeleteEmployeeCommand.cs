using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(string id, string token) : ICommand;
