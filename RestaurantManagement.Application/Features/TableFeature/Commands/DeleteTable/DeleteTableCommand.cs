using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.DeleteTable;

public record DeleteTableCommand(int id, string token) : ICommand;
