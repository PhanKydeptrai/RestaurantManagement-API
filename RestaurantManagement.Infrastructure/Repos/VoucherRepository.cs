using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class VoucherRepository : IVoucherRepository
{
    private readonly RestaurantManagementDbContext _context;

    public VoucherRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsVoucherNameExists(string voucherName)
    {
        return await _context.Vouchers.AsNoTracking().AnyAsync(a => a.VoucherName == voucherName);
    }

    public async Task<Voucher?> GetVoucherById(Ulid id)
    {
        return await _context.Vouchers.FindAsync(id);
    }
    public async Task<bool> IsVoucherIdExists(Ulid voucherId)
    {
        return await _context.Vouchers.AsNoTracking().AnyAsync(a => a.VoucherId == voucherId && a.Status == "Active");
    }
    public async Task<bool> IsVoucherNameExists(string voucherName, Ulid voucherId)
    {
        return await _context.Vouchers.AsNoTracking().AnyAsync(a => a.VoucherName == voucherName && a.VoucherId != voucherId);
    }

    public async Task DeleteVoucher(Ulid voucherId)
    {
        await _context.Vouchers.Where(a => a.VoucherId == voucherId)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.Status, "Deleted"));
    }
}
