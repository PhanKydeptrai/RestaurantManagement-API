using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;

public record AssignTableToBookedCustomerCommand(string tableId, string token) : ICommand;
