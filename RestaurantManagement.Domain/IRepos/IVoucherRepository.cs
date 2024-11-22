using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IVoucherRepository 
{
    Task<bool> IsVoucherNameExists(string voucherName);
    Task<bool> IsVoucherNameExists(string voucherName, Ulid voucherId);
    Task<bool> IsVoucherIdExists(Ulid voucherId);
    Task<Voucher?> GetVoucherById(Ulid id);
    Task<bool> IsVoucherValid(string voucherName);
    Task DeleteVoucher(Ulid voucherId);
}
