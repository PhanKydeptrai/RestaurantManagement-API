using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;
public record CreateCustomerCommand(string FirstName,
                                    string LastName,
                                    string? Password, 
                                    string PhoneNumber, 
                                    string Email,
                                    string Gender) 
                                : IRequest<Result<bool>>;

