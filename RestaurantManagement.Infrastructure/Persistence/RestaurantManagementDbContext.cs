using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Persistence;

public class RestaurantManagementDbContext : DbContext, IApplicationDbContext
{
    public RestaurantManagementDbContext(DbContextOptions<RestaurantManagementDbContext> options) : base(options) { }

    public DbSet<Bill> Bills { get; set; }
    public DbSet<BookingChangeLog> BookingChangeLogs { get; set; }
    public DbSet<CustomerVoucher> CustomerVouchers { get; set; }
    public DbSet<OrderChangeLog> OrderChangeLogs { get; set; }
    public DbSet<PaymentType> PaymentTypes { get; set; }
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
    public DbSet<SystemLog> SystemLogs { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<User> Users { get; set; }

    
    //Cấu hình fluent api
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Bill>(e =>
        {
            e.HasKey(a => a.BillId);
            e.Property(a => a.BillId).IsRequired().HasConversion<UlidToStringConverter>();

            e.Property(a => a.PaymentStatus).IsRequired().HasColumnType("varchar(20)");

            e.Property(a => a.OrderId).IsRequired().HasConversion<UlidToStringConverter>();

            e.Property(a => a.BookId).IsRequired(false).HasConversion<UlidToStringConverter>();

            e.Property(a => a.VoucherId).IsRequired(false).HasConversion<UlidToStringConverter>();

            e.Property(a => a.PaymentTypeId).IsRequired().HasConversion<UlidToStringConverter>();

            e.Property(a => a.Total).IsRequired().HasColumnType("decimal(18,2)");

            //ForeignKey
            //Một bill có một order
            e.HasOne(a => a.Order).WithOne(a => a.Bill).HasForeignKey<Bill>(a => a.OrderId);
            //Một bill có một booking?
            e.HasOne(a => a.Booking).WithOne(a => a.Bill).HasForeignKey<Bill>(a => a.BookId).IsRequired(false);
            //Một voucher có nhiều bill
            e.HasOne(a => a.Voucher).WithMany(a => a.Bills).HasForeignKey(a => a.VoucherId);
            //Một payment type có nhiều bill
            e.HasOne(a => a.PaymentType).WithMany(a => a.Bills).HasForeignKey(a => a.PaymentTypeId);
        });

        modelBuilder.Entity<PaymentType>(e =>
        {
            e.HasKey(a => a.PaymentTypeId);
            e.Property(a => a.PaymentTypeId).IsRequired().HasConversion<UlidToStringConverter>();

            e.Property(a => a.Name).IsRequired().HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.HasKey(a => a.BookId);
            e.Property(a => a.BookId).IsRequired().HasConversion<UlidToStringConverter>();

            e.Property(a => a.Time).IsRequired().HasColumnType("datetime");
            e.Property(a => a.BookingPrice).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.PaymentStatus).IsRequired().HasColumnType("varchar(20)");
            e.Property(a => a.CustomerId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");

            //ForeignKey
            //Một Customer có nhiều booking
            e.HasOne(a => a.Customer).WithMany(a => a.Bookings).HasForeignKey(a => a.CustomerId);
            //Một booking có nhiều bookingchangelog
            e.HasMany(a => a.BookingChangeLogs).WithOne(a => a.Booking).HasForeignKey(a => a.BookId);
            //Một booking có nhiều bookingdetail
            e.HasMany(a => a.BookingDetails).WithOne(a => a.Booking).HasForeignKey(a => a.BookId);
        });

        modelBuilder.Entity<BookingDetail>(e =>
        {
            e.HasKey(a => a.BookingDetailId);
            e.Property(a => a.BookingDetailId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.TableId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.Status).IsRequired().HasColumnType("varchar(20)");
            e.Property(a => a.Quantity).IsRequired().HasColumnType("int");
            e.Property(a => a.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.BookId).IsRequired().HasConversion<UlidToStringConverter>();

            //ForeignKey
            //một table có nhiều bookingdetail
            e.HasOne(a => a.Table).WithMany(a => a.BookingDetails).HasForeignKey(a => a.TableId);
        });

        modelBuilder.Entity<TableType>(e =>
        {
            e.HasKey(a => a.TableTypeId);
            e.Property(a => a.TableTypeId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.TableImage).IsRequired(false).HasColumnType("varbinary(max)");
            e.Property(a => a.TablePrice).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");

            //ForeignKey
            //Một tabletype có nhiều table
            e.HasMany(a => a.Tables).WithOne(a => a.TableType).HasForeignKey(a => a.TableTypeId);
        });

        modelBuilder.Entity<Table>(e =>
        {
            e.HasKey(a => a.TableId);
            e.Property(a => a.TableId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.TableTypeId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.TableStatus).IsRequired().HasColumnType("varchar(20)");

            //ForeignKey
            //Một table có nhiều order
            e.HasMany(a => a.Orders).WithOne(a => a.Table).HasForeignKey(a => a.TableId);

        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(a => a.OrderId);
            e.Property(a => a.OrderId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.PaymentStatus).IsRequired().HasColumnType("varchar(20)");
            e.Property(a => a.Total).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.CustomerId).IsRequired(false).HasConversion<UlidToStringConverter>();
            e.Property(a => a.TableId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.OrderTime).IsRequired().HasColumnType("datetime");
            e.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");

            //ForeignKey
            //Một order có nhiều orderdetail
            e.HasMany(a => a.OrderDetails).WithOne(a => a.Order).HasForeignKey(a => a.OrderId);
            //Một order có nhiều orderchangelog
            e.HasMany(a => a.OrderChangeLogs).WithOne(a => a.Order).HasForeignKey(a => a.OrderId);
            //Một Customer có nhiều order
            e.HasOne(a => a.Customer).WithMany(a => a.Orders).HasForeignKey(a => a.CustomerId).IsRequired(false);
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(a => a.CustomerId);
            e.Property(a => a.CustomerId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.CustomerStatus).IsRequired().HasColumnType("varchar(20)");
            e.Property(a => a.CustomerType).IsRequired().HasColumnType("varchar(20)");

            //ForeignKey
            //Một customer có một user
            e.HasOne(a => a.User).WithOne(a => a.Customer).HasForeignKey<Customer>(a => a.UserId);
            //Một customer có nhiều customerVoucher
            e.HasMany(a => a.CustomerVouchers).WithOne(a => a.Customer).HasForeignKey(a => a.CustomerId);


        });

        modelBuilder.Entity<CustomerVoucher>(e =>
        {
            e.HasKey(a => a.CustomerVoucherId);
            e.Property(a => a.CustomerVoucherId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.VoucherId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.CustomerId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.Quantity).IsRequired().HasColumnType("int");

            //ForeignKey
            //Một voucher có nhiều customerVoucher
            e.HasOne(a => a.Voucher).WithMany(a => a.CustomerVouchers).HasForeignKey(a => a.VoucherId);

        });


        modelBuilder.Entity<Voucher>(e =>
        {
            e.HasKey(a => a.VoucherId);
            e.Property(a => a.VoucherId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.VoucherName).IsRequired().HasColumnType("varchar(50)");
            e.Property(a => a.MaxDiscount).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.VoucherCondition).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");
        });

        modelBuilder.Entity<OrderDetail>(e =>
        {
            e.HasKey(a => a.OrderDetailId);
            e.Property(a => a.OrderDetailId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.OrderId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.MealId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.Quantity).IsRequired().HasColumnType("int");
            e.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");
            e.Property(a => a.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");

            //ForeignKey
            //Một meal có nhiều orderdetail
            e.HasOne(a => a.Meal).WithMany(a => a.OrderDetails).HasForeignKey(a => a.MealId);

        });

        modelBuilder.Entity<Meal>(e =>
        {
            e.HasKey(a => a.MealId);
            e.Property(a => a.MealId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.MealName).IsRequired().HasColumnType("varchar(100)");
            e.Property(a => a.Price).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.Image).IsRequired(false).HasColumnType("varbinary(max)");
            e.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");
            e.Property(a => a.SellStatus).IsRequired().HasColumnType("varchar(50)");
            e.Property(a => a.MealStatus).IsRequired().HasColumnType("varchar(50)");
            e.Property(a => a.CategoryId).IsRequired().HasConversion<UlidToStringConverter>();
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(a => a.CategoryId);
            e.Property(a => a.CategoryId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.CategoryName).IsRequired().HasColumnType("varchar(100)");
            e.Property(a => a.CategoryImage).IsRequired(false).HasColumnType("varbinary(max)");
            e.Property(a => a.CategoryStatus).IsRequired().HasColumnType("varchar(50)");
        });

        modelBuilder.Entity<OrderChangeLog>(e =>
        {
            e.HasKey(a => a.OrderChangeLogId);
            e.Property(a => a.OrderChangeLogId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.LogMessage).IsRequired().HasColumnType("nvarchar(255)");
            e.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");
            e.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
            e.Property(a => a.OrderId).IsRequired().HasConversion<UlidToStringConverter>();

            //ForeignKey
            //Một user có nhiều orderchangelog
            e.HasOne(a => a.User).WithMany(a => a.OrderChangeLogs).HasForeignKey(a => a.UserId);

        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(a => a.UserId);
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.FirstName).IsRequired().HasColumnType("nvarchar(50)");
            e.Property(a => a.LastName).IsRequired().HasColumnType("nvarchar(50)");
            e.Property(a => a.Password).IsRequired(false).HasColumnType("varchar(64)");
            e.Property(a => a.Phone).IsRequired(false).HasColumnType("varchar(20)");
            e.Property(a => a.Status).IsRequired().HasColumnType("varchar(20)");
            e.Property(a => a.Email).IsRequired(false).HasColumnType("varchar(50)");
            e.Property(a => a.UserImage).IsRequired(false).HasColumnType("varbinary(max)");
            e.Property(a => a.Gender).IsRequired(false).HasColumnType("varchar(10)");

            //ForeignKey
            //Một user có nhiều notification
            e.HasMany(a => a.Notifications).WithOne(a => a.User).HasForeignKey(a => a.UserId);
            //Một user có nhiều systemlog
            e.HasMany(a => a.SystemLogs).WithOne(a => a.User).HasForeignKey(a => a.UserId);
            //Một user có nhiều BookingChangeLog
            e.HasMany(a => a.BookingChangeLogs).WithOne(a => a.User).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.NoAction); ;
            //một user có một employee
            e.HasOne(a => a.Employee).WithOne(a => a.User).HasForeignKey<Employee>(a => a.UserId);
        });

        modelBuilder.Entity<Employee>(e =>
        {
            e.HasKey(a => a.EmployeeId);
            e.Property(a => a.EmployeeId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.EmployeeStatus).IsRequired().HasColumnType("varchar(20)");
            e.Property(a => a.Role).IsRequired().HasColumnType("varchar(20)");
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.HasKey(a => a.NotificationId);
            e.Property(a => a.NotificationId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.Paragraph).IsRequired().HasColumnType("nvarchar(255)");
            e.Property(a => a.Time).IsRequired().HasColumnType("datetime");
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.Status).IsRequired().HasColumnType("varchar(20)");
        });

        modelBuilder.Entity<SystemLog>(e =>
        {
            e.HasKey(a => a.SystemLogId);
            e.Property(a => a.SystemLogId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.LogDetail).IsRequired().HasColumnType("nvarchar(255)");
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");

        });

        modelBuilder.Entity<BookingChangeLog>(e =>
        {
            e.HasKey(a => a.BookingChangeLogId);
            e.Property(a => a.BookingChangeLogId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.LogMessage).IsRequired().HasColumnType("nvarchar(255)");
            e.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");
            e.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
            e.Property(a => a.BookId).IsRequired().HasConversion<UlidToStringConverter>();

        });

        
        modelBuilder.Entity<EmailVerificationToken>(e =>
        {
            e.HasKey(a => a.EmailVerificationTokenId);
            e.Property(a => a.EmailVerificationTokenId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
            e.Property(a => a.CreatedDate).IsRequired().HasColumnType("datetime");
            e.Property(a => a.ExpiredDate).IsRequired().HasColumnType("datetime");

            //ForeignKey
            //Một user có nhiều emailverificationtoken
            e.HasOne(a => a.User).WithMany(a => a.EmailVerificationTokens).HasForeignKey(a => a.UserId);

        });
    }

    //protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    //{
    //    configurationBuilder
    //        .Properties<Ulid>()
    //        .HaveConversion<UlidToStringConverter>();
    //}
}