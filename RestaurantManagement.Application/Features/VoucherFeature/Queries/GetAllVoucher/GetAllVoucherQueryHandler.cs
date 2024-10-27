using System.Linq.Expressions;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Queries.GetAllVoucher;

public class GetAllVoucherQueryHandler : IQueryHandler<GetAllVoucherQuery, PagedList<Voucher>>
{
    private readonly IApplicationDbContext _context;

    public GetAllVoucherQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<Voucher>>> Handle(GetAllVoucherQuery request, CancellationToken cancellationToken)
    {
        var voucherQuery = _context.Vouchers.AsQueryable();
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
        var vouchers = voucherQuery.AsQueryable();
        var voucherList = await PagedList<Voucher>.CreateAsync(vouchers, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<Voucher>>.Success(voucherList);
    }
}
