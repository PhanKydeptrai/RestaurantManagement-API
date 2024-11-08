using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;

public record RestoreEmployeeCommand(string id, string token) : ICommand;

