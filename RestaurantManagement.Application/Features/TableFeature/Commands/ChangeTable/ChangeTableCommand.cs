using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.ChangeTable;

public record ChangeTableRequest(
    // string oldtableId, 
    object newTableId);
public record ChangeTableCommand(
    string oldtableId, 
    object newTableId,
    string token) : ICommand;
