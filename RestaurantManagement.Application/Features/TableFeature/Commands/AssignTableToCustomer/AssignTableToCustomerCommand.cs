using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;


public record AssignTableToCustomerCommand(int id) : ICommand;
