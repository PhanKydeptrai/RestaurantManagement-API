using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

public record CreateTableCommand(string quantity, string tableTypeId, string token) : ICommand;

public record CreateTableRequest(string quantity, string tableTypeId);