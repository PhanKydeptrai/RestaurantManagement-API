using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;

public record DeleteTableTypeCommand(Ulid id, string token) : ICommand;
