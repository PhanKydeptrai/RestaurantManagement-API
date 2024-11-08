using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.TableDto;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetTableById;

public record GetTableByIdQuery(string id) : IQuery<TableResponse>;

