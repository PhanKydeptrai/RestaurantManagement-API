using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public record RestoreTableCommand(int id, string token) : ICommand;
