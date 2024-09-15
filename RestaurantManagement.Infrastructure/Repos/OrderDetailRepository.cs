using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class OrderDetailRepository : IOrderDetailRepository
{
    private readonly RestaurantManagementDbContext _context;
    public OrderDetailRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task CreateOrderDetail(OrderDetail orderDetail)
    {
        await _context.OrderDetails.AddAsync(orderDetail);
    }

    public void DeleteOrderDetail(OrderDetail orderDetail)
    {
        _context.OrderDetails.Remove(orderDetail);
    }

    public async Task<OrderDetail?> GetOrderDetailById(Guid id)
    {
        return await _context.OrderDetails.FirstOrDefaultAsync(x => x.OrderDetailId == id);
    }

    public async Task<ICollection<OrderDetail>> GetOrderDetails()
    {
        return await _context.OrderDetails.ToListAsync();
    }

    public async Task<ICollection<OrderDetail>> GetOrderDetailsByOrderId(Guid id)
    {
        return await _context.OrderDetails.Where(x => x.OrderId == id).ToListAsync();
    }

    public IQueryable<OrderDetail> GetQueryableOrderDetails()
    {
        return _context.OrderDetails.AsQueryable();
    }

    public void UpdateOrderDetail(OrderDetail orderDetail)
    {
        _context.OrderDetails.Update(orderDetail);
    }
}
