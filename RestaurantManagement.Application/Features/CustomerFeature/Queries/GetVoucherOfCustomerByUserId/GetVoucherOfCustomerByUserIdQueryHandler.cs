using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.CustomerVoucherDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetVoucherOfCustomerByUserId;

public class GetVoucherOfCustomerByUserIdQueryHandler : IQueryHandler<GetVoucherOfCustomerByUserIdQuery, PagedList<CustomerVoucherResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetVoucherOfCustomerByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<CustomerVoucherResponse>>> Handle(GetVoucherOfCustomerByUserIdQuery request, CancellationToken cancellationToken)
    {

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        
        var customerVouchers = _context.Vouchers
            .Include(a => a.CustomerVouchers)
            .ThenInclude(a => a.Customer)
            .Where(a => a.CustomerVouchers.Any(b => b.Customer.UserId == Ulid.Parse(userId)))
            .AsQueryable();

        //Search
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            customerVouchers = customerVouchers.Where(x => x.VoucherName.Contains(request.searchTerm));
        }

        //Filter 
        if(!string.IsNullOrEmpty(request.filterType))
        {
            customerVouchers = customerVouchers.Where(x => x.VoucherType == request.filterType);
        }

        if(!string.IsNullOrEmpty(request.filterStatus))
        {
            customerVouchers = customerVouchers.Where(x => x.Status == request.filterStatus);
        }

        //sort
        Expression<Func<Voucher, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "vouchername" => x => x.VoucherName,
            "voucherid" => x => x.VoucherId,
            _ => x => x.VoucherId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            customerVouchers = customerVouchers.OrderByDescending(keySelector);
        }
        else
        {
            customerVouchers = customerVouchers.OrderBy(keySelector);
        }

        //paged
        var vouchers = customerVouchers
            .Select(a => new CustomerVoucherResponse(
                a.VoucherId,
                a.VoucherName,
                a.VoucherCode,
                a.VoucherType,
                a.PercentageDiscount,
                a.MaximumDiscountAmount,
                a.MinimumOrderAmount,
                a.VoucherConditions,
                a.StartDate,
                a.ExpiredDate,
                a.Description,
                a.Status,
                a.CustomerVouchers.FirstOrDefault(a => a.Customer.UserId == Ulid.Parse(userId)).Quantity)).AsQueryable();
        var vouchersList = await PagedList<CustomerVoucherResponse>.CreateAsync(vouchers, request.page ?? 1, request.pageSize ?? 10);
        return Result<PagedList<CustomerVoucherResponse>>.Success(vouchersList);
    }
}
