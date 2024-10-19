using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Data;

public interface IApplicationDbContext
{
    DbSet<Bill> Bills { get; set; }
    DbSet<Booking> Bookings { get; set; }
    DbSet<BookingChangeLog> BookingChangeLogs { get; set; }
    DbSet<BookingDetail> BookingDetails { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<CustomerVoucher> CustomerVouchers { get; set; }
    DbSet<Employee> Employees { get; set; }
    DbSet<Meal> Meals { get; set; }
    DbSet<Notification> Notifications { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderChangeLog> OrderChangeLogs { get; set; }
    DbSet<OrderDetail> OrderDetails { get; set; }
    DbSet<PaymentType> PaymentTypes { get; set; }
    DbSet<SystemLog> SystemLogs { get; set; }
    DbSet<Table> Tables { get; set; }
    DbSet<TableType> TableTypes { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Voucher> Vouchers { get; set; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
}
