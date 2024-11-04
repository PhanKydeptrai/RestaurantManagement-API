using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.TableDto;

namespace RestaurantManagement.Application.Features.TableFeature.Queries.GetTableInfo;

public record GetTableInfoQuery() : IQuery<TableInfo[]>;
