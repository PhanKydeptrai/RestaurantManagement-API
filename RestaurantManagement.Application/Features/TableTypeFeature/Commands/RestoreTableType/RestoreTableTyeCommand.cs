using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.RestoreTableType;

public record RestoreTableTyeCommand(Ulid id) : ICommand;
