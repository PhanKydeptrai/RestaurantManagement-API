using System;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class OrderRepository : IOrderRepository
{
    private readonly RestaurantManagementDbContext _context;
    public OrderRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task AddOrder(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public void DeleteOrder(Order order)
    {
        _context.Orders.Remove(order);
    }

    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order?> GetOrderById(Ulid id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(Ulid id)
    {
        return await _context.Orders.Where(c => c.CustomerId == id).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByOrderStatus(string status)
    {
        return await _context.Orders.Where(s => s.PaymentStatus == status).ToListAsync();
    }

    public IQueryable<Order> GetOrdersQueryable()
    {
        return _context.Orders.AsQueryable();
    }

    public Task<bool> IsStatusExist(string status)
    {
        return _context.Orders.AnyAsync(s => s.PaymentStatus == status);
    }

    public void UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
    }
}
