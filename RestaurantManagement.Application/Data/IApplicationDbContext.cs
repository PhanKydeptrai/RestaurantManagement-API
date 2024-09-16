using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Employee> Employees { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Notification> Notifications { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<Meal> Meals { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<OrderDetail> OrderDetails { get; set; }
    DbSet<Table> Tables { get; set; }
    DbSet<SystemLog> SystemLogs { get; set; }
    DbSet<Booking> Bookings { get; set; }
    DbSet<BookingDetail> BookingDetails { get; set; }
    
}
