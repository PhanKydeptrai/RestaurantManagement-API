using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Infrastructure.Persistence;

public class RestaurantManagementDbContext(DbContextOptions<RestaurantManagementDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Bill> Bills { get; set; }
    public DbSet<CustomerVoucher> CustomerVouchers { get; set; }
    // public DbSet<PaymentType> PaymentTypes { get; set; }
    public DbSet<TableType> TableTypes { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingDetail> BookingDetails { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<BillLog> BillLogs { get; set; }
    public DbSet<TableLog> TableLogs { get; set; }
    public DbSet<TableTypeLog> TableTypeLogs { get; set; }
    public DbSet<CategoryLog> CategoryLogs { get; set; }
    public DbSet<MealLog> MealLogs { get; set; }
    public DbSet<OrderLog> OrderLogs { get; set; }
    public DbSet<CustomerLog> CustomerLogs { get; set; }
    public DbSet<EmployeeLog> EmployeeLogs { get; set; }

    //Cấu hình fluent api
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }


    //protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    //{
    //    configurationBuilder
    //        .Properties<Ulid>()
    //        .HaveConversion<UlidToStringConverter>();
    //}
}