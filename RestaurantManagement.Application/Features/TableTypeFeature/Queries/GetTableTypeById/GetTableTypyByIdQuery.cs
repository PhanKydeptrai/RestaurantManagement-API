using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.TableTypeDto;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Queries.GetTableTypeById;

public record GetTableTypyByIdQuery(string id) : IQuery<TableTypeResponse>;

