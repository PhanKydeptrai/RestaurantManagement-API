using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetAllVoucher;

public record GetAllVoucherQuery(
    string? filterStatus,
    string? searchTerm,
    string? sortColumn,
    string? sortOrder,
    int? page,
    int? pageSize) : IQuery<PagedList<Voucher>>;