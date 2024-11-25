using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Data;

public interface IApplicationDbContext
{
    DbSet<Bill> Bills { get; set; }
    DbSet<Booking> Bookings { get; set; }
    DbSet<BookingDetail> BookingDetails { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<CustomerVoucher> CustomerVouchers { get; set; }
    DbSet<Employee> Employees { get; set; }
    DbSet<Meal> Meals { get; set; }
    DbSet<Notification> Notifications { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderDetail> OrderDetails { get; set; }
    DbSet<Table> Tables { get; set; }
    DbSet<TableType> TableTypes { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<Voucher> Vouchers { get; set; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    DbSet<BillLog> BillLogs { get; set; }
    DbSet<TableLog> TableLogs { get; set; }
    DbSet<TableTypeLog> TableTypeLogs { get; set; }
    DbSet<CategoryLog> CategoryLogs { get; set; }
    DbSet<MealLog> MealLogs { get; set; }
    DbSet<OrderLog> OrderLogs { get; set; }
    DbSet<CustomerLog> CustomerLogs { get; set; }
    DbSet<EmployeeLog> EmployeeLogs { get; set; }
    DbSet<BookingLog> BookingLogs { get; set; }
    DbSet<VoucherLog> VoucherLogs { get; set; }
    DbSet<OrderTransaction> OrderTransactions { get; set; }
}
