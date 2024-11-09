using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToBookedCustomerCommand;

public record UnAssignTableToBookedCustomerCommand(string id) : ICommand;
