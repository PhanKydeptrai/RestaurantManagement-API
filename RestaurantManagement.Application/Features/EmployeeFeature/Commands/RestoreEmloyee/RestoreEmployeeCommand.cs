using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;

public record RestoreEmployeeCommand(Ulid id, string token) : ICommand;

