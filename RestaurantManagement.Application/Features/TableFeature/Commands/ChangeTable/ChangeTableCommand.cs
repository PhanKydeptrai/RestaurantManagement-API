using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.ChangeTable;

public record ChangeTableRequest(
    // string oldtableId, 
    object newTableId,
    string note);
public record ChangeTableCommand(
    string oldtableId, 
    string note,
    object newTableId,
    string token) : ICommand;
