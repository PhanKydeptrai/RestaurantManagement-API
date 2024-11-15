using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

//NOTE: Đã lý Unstrusted data
public record CreateTableCommand(object quantity, string tableTypeId, string token) : ICommand;

public record CreateTableRequest(object quantity, string tableTypeId);

#region Stable code
// public record CreateTableCommand(int quantity, string tableTypeId, string token) : ICommand;
// public record CreateTableRequest(int quantity, string tableTypeId);
#endregion