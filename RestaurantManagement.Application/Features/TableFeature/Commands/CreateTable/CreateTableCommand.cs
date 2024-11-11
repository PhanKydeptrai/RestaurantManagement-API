using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

public record CreateTableCommand(int quantity, string tableTypeId, string token) : ICommand;

public record CreateTableRequest(int quantity, string tableTypeId);