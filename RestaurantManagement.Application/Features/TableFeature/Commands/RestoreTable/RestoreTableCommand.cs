using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public record RestoreTableCommand(Ulid id, string token) : ICommand;
