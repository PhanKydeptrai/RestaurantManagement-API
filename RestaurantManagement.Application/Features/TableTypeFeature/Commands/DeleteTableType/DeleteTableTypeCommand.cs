using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.DeleteTableType;

public record DeleteTableTypeCommand(string id, string token) : ICommand;
