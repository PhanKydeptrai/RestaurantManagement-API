using Microsoft.EntityFrameworkCore;
using Quartz;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Infrastructure.BackgroundJob;

public class VoucherBackgroundJob : IJob
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public VoucherBackgroundJob(
        IApplicationDbContext context, 
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        //Kiểm tra toàn bộ khách hàng. Nếu có voucher hết hạn thì xóa voucher đó và gửi mail thông báo

        //Kiểm tra voucher hết hạn
        Voucher[]? expiredVouchers = await _context.Vouchers
            .Where(v => v.ExpiredDate < DateTime.Now)
            .ToArrayAsync();

        //Cập nhật voucher hết hạn
        foreach (var voucher in expiredVouchers)
        {
            voucher.Status = "InActive";
        }

        //Cập nhật thay đổi cho voucher

        await _unitOfWork.SaveChangesAsync();

        Console.WriteLine("Voucher background job is running");        
    }
}
