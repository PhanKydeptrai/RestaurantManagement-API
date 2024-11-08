using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public record RestoreTableCommand(string id, string token) : ICommand;
