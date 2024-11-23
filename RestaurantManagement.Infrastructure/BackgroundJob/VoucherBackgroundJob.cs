using Quartz;
using RestaurantManagement.Application.Data;

namespace RestaurantManagement.Infrastructure.BackgroundJob;

public class VoucherBackgroundJob : IJob
{
    private readonly IApplicationDbContext _context;

    public VoucherBackgroundJob(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
