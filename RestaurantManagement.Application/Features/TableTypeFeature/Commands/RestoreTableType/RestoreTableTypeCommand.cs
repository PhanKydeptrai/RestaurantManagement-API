using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.RestoreTableType;

public record RestoreTableTypeCommand(string id) : ICommand;
