using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.GetTableForCustomer;

public record AssignTableToCustomerCommand(int id) : ICommand;
