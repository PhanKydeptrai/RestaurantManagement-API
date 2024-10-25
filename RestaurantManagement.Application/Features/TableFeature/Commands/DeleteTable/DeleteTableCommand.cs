using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;

public record DeleteTableCommand(Ulid id, string token) : ICommand;
