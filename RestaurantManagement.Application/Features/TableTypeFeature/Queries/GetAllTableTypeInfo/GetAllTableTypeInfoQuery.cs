using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.TableTypeDto;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetAllTableTypeInfo;

public record GetAllTableInfoQuery() : IQuery<List<TableTypeInfo>>;
