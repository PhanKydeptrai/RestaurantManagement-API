using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class OrderDetailRepository(RestaurantManagementDbContext context) : IOrderDetailRepository
{
    public async Task CreateOrderDetail(OrderDetail orderDetail)
    {
        await context.OrderDetails.AddAsync(orderDetail);
    }

    public void DeleteOrderDetail(OrderDetail orderDetail)
    {
        context.OrderDetails.Remove(orderDetail);
    }

    public async Task<OrderDetail?> GetOrderDetailById(Ulid id)
    {
        return await context.OrderDetails.FindAsync(id);
    }

    public async Task<ICollection<OrderDetail>> GetOrderDetails()
    {
        return await context.OrderDetails.ToListAsync();
    }

    public async Task<ICollection<OrderDetail>> GetOrderDetailsByOrderId(Ulid id)
    {
        return await context.OrderDetails.Where(x => x.OrderId == id).ToListAsync();
    }

    public IQueryable<OrderDetail> GetQueryableOrderDetails()
    {
        return context.OrderDetails.AsQueryable();
    }

    public async Task<bool> IsOrderDetailCanDelete(Ulid id)
    {
        return await context.OrderDetails.Include(a => a.Order).AsNoTracking().AnyAsync(a => a.OrderDetailId == id && a.Order.PaymentStatus == "Unpaid");
    }

    public async Task<bool> IsOrderDetailCanUpdate(Ulid id)
    {
        return await context.OrderDetails.Include(a => a.Order).AsNoTracking()
            .AnyAsync(a => a.OrderDetailId == id && a.Order.PaymentStatus == "Unpaid");
    }

    public void UpdateOrderDetail(OrderDetail orderDetail)
    {
        context.OrderDetails.Update(orderDetail);
    }

    
}
