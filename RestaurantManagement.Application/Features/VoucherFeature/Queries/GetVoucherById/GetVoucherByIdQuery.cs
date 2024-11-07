using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetVoucherById;

public record GetVoucherByIdQuery(string id) : IQuery<Voucher>;
