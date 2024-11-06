using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class OrderRepository(RestaurantManagementDbContext context) : IOrderRepository
{
    public async Task AddOrder(Order order)
    {
        await context.Orders.AddAsync(order);
    }

    public void DeleteOrder(Order order)
    {
        context.Orders.Remove(order);
    }

    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        return await context.Orders.ToListAsync();
    }

    public async Task<Order?> GetOrderById(Ulid id)
    {
        return await context.Orders.FindAsync(id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(Ulid id)
    {
        return await context.Orders.Where(c => c.CustomerId == id).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByOrderStatus(string status)
    {
        return await context.Orders.Where(s => s.PaymentStatus == status).ToListAsync();
    }

    public IQueryable<Order> GetOrdersQueryable()
    {
        return context.Orders.AsQueryable();
    }

    public Task<bool> IsStatusExist(string status)
    {
        return context.Orders.AnyAsync(s => s.PaymentStatus == status);
    }

    public void UpdateOrder(Order order)
    {
        context.Orders.Update(order);
    }
}
