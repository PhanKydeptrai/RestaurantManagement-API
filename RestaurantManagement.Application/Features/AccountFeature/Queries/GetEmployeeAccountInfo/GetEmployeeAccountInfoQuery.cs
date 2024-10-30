using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.GetEmployeeAccountInfo;

public record GetEmployeeAccountInfoQuery(string token): IQuery<EmployeeResponse>;
