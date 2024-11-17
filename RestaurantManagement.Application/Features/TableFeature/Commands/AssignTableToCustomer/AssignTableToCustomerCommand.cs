using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;


public record AssignTableToCustomerCommand(string id) : ICommand;

#region Stable code
// public record AssignTableToCustomerCommand(string id) : ICommand;
#endregion
