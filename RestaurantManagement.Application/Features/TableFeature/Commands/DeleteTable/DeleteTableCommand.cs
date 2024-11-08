using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;

public record DeleteTableCommand(string id, string token) : ICommand;
