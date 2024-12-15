using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.VoucherDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetAllVoucher;

public class GetAllVoucherQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAllVoucherQuery, PagedList<VoucherDto>>
{
    public async Task<Result<PagedList<VoucherDto>>> Handle(GetAllVoucherQuery request, CancellationToken cancellationToken)
    {
        var voucherQuery = context.Vouchers.AsQueryable();
        
        //Search
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            voucherQuery = voucherQuery.Where(x => x.VoucherName.Contains(request.searchTerm));
        }

        //Filter
        if(!string.IsNullOrEmpty(request.filterStatus))
        {
            voucherQuery = voucherQuery.Where(x => x.Status == request.filterStatus);
        }

        if(!string.IsNullOrEmpty(request.filterType))
        {
            voucherQuery = voucherQuery.Where(x => x.VoucherType == request.filterType);
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
            voucherQuery = voucherQuery.OrderByDescending(keySelector);
        }
        else
        {
            voucherQuery = voucherQuery.OrderBy(keySelector);
        }


        //paged
        var vouchers = voucherQuery.Select(a => new VoucherDto(
            a.VoucherId, 
            a.VoucherName, 
            a.VoucherCode,
            a.VoucherType,
            a.PercentageDiscount,
            a.MaximumDiscountAmount,
            a.MinimumOrderAmount,
            a.VoucherConditions,
            a.StartDate.ToString("dd/MM/yyyy"),
            a.ExpiredDate.ToString("dd/MM/yyyy"),
            a.Description,
            a.Status)).AsQueryable();
        var voucherList = await PagedList<VoucherDto>.CreateAsync(vouchers, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<VoucherDto>>.Success(voucherList);
    }
}
