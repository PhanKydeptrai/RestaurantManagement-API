using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToCustomer;

public record UnAssignTableToCustomerCommand(string id) : ICommand;
