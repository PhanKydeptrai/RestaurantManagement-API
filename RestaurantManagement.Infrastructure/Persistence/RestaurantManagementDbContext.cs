using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Infrastructure.Persistence;

public class RestaurantManagementDbContext : DbContext, IApplicationDbContext
{
    public RestaurantManagementDbContext(DbContextOptions<RestaurantManagementDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingDetail> BookingDetails { get; set; }



    //Cấu hình fluent api
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Cấu hình bảng User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(k => k.UserId);
            entity.Property(p => p.FirstName).IsRequired().HasColumnType("nvarchar(10)");
            entity.Property(p => p.LastName).IsRequired().HasColumnType("nvarchar(10)");
            entity.Property(p => p.Password).IsRequired(false).HasColumnType("varchar(64)");
            entity.Property(p => p.PhoneNumber).IsRequired(false).HasColumnType("varchar(10)");
            entity.Property(p => p.Gender).IsRequired(false).HasColumnType("varchar(10)");
            entity.Property(p => p.Status).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.Email).IsRequired().HasColumnType("varchar(30)");
            entity.Property(p => p.UserImage).IsRequired(false).HasColumnType("varbinary(max)");

            //Quan hệ 1-1 với bảng Employee
            entity.HasOne(e => e.Employee).WithOne(u => u.User).HasForeignKey<Employee>(e => e.UserId);
            //Quan hệ 1-1 với bảng Customer
            entity.HasOne(e => e.Customer).WithOne(u => u.User).HasForeignKey<Customer>(e => e.UserId);
            //Quan hệ 1-N với bảng Notification
            entity.HasMany(e => e.Notifications).WithOne(u => u.User).HasForeignKey(e => e.UserId);

        });
        #endregion

        #region Cấu hình bảng Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(k => k.EmployeeId);
            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.Role).IsRequired().HasColumnType("varchar(20)");

        });
        #endregion

        #region Cấu hình bảng Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(k => k.CustomerId);
            // entity.Property(p => p.UserId).IsRequired(false);
            entity.Property(p => p.CustomerType).IsRequired().HasColumnType("varchar(20)");
            //Quan hệ 1 - N với bảng Order
            entity.HasMany(e => e.Orders).WithOne(u => u.Customer).HasForeignKey(e => e.CustomerId).IsRequired(false);
            //Quan hệ 1 nhiều với bảng Booking
            entity.HasMany(e => e.Bookings).WithOne(u => u.Customer).HasForeignKey(e => e.CustomerId);
        });
        #endregion

        #region Cấu hình bảng Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(k => k.NotificationId);
            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.Status).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.Pagragraph).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.Time).IsRequired().HasColumnType("datetime");
        });
        #endregion

        #region Cấu hình bảng Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(k => k.OrderId);
            entity.Property(p => p.CustomerId).IsRequired(false);
            entity.Property(p => p.OrderStatus).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.Total).IsRequired().HasColumnType("decimal");
            entity.Property(p => p.OrderTime).IsRequired().HasColumnType("datetime");

            //Quan hệ 1 - N với bảng OrderDetail
            entity.HasMany(e => e.OrderDetails).WithOne(u => u.Order).HasForeignKey(e => e.OrderId);
        });
        #endregion

        #region Cấu hình bảng Meal
        modelBuilder.Entity<Meal>(entity =>
        {
            entity.HasKey(k => k.MealId);
            entity.Property(p => p.CategoryId).IsRequired();
            entity.Property(p => p.MealName).IsRequired().HasColumnType("nvarchar(30)");
            entity.Property(p => p.Price).IsRequired().HasColumnType("decimal");
            entity.Property(p => p.Image).IsRequired(false).HasColumnType("varbinary(max)");
            entity.Property(p => p.Description).IsRequired(false).HasColumnType("nvarchar(100)");

        });

        #endregion

        #region Cấu hình bảng Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(k => k.CategoryId);
            entity.Property(p => p.CategoryStatus).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.CategoryName).IsRequired().HasColumnType("nvarchar(50)");
            entity.Property(p => p.Image).IsRequired(false).HasColumnType("varbinary(max)");
            entity.Property(p => p.Desciption).IsRequired(false).HasColumnType("nvarchar(100)");

            //Quan hệ 1 - N với bảng Meal
            entity.HasMany(e => e.Meals).WithOne(u => u.Category).HasForeignKey(e => e.CategoryId);
        });
        #endregion

        #region Cấu hình bảng OrderDetail
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(k => k.OrderDetailId);
            entity.Property(p => p.OrderId).IsRequired();
            entity.Property(p => p.MealId).IsRequired();
            entity.Property(p => p.Quantity).IsRequired().HasColumnType("int");
            entity.Property(p => p.Note).IsRequired(false).HasColumnType("nvarchar(255)");

            //Quan hệ 1 - N với bảng Meal
            entity.HasOne(e => e.Meal).WithMany(u => u.OrderDetails).HasForeignKey(e => e.MealId);
            

        });
        #endregion

        #region Cấu hình cho bảng Table
        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(k => k.TableId);
            entity.Property(p => p.TableType).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.TableName).IsRequired().HasColumnType("nvarchar(30)");
            entity.Property(p => p.TableStatus).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.Desciption).IsRequired(false).HasColumnType("varchar(255)");
            entity.Property(p => p.TableImage).IsRequired(false).HasColumnType("varbinary(max)");

            //Quan hệ 1 - N với bảng Order
            entity.HasMany(e => e.Orders).WithOne(u => u.Table).HasForeignKey(e => e.TableId);
        });
        #endregion

        #region Cấu hình cho bảng SystemLog
        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(k => k.LogId);
            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.LogDetail).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.LogDate).IsRequired().HasColumnType("datetime");
        });
        #endregion

        #region Cấu hình cho bảng Booking
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(k => k.BookingId);
            entity.Property(p => p.CustomerId).IsRequired();
            entity.Property(p => p.BookingPrice).IsRequired().HasColumnType("decimal");
            entity.Property(p => p.Status).IsRequired().HasColumnType("varchar(20)");
            entity.Property(p => p.Note).IsRequired(false).HasColumnName("nvarchar(255)");
            entity.Property(p => p.Time).IsRequired().HasColumnType("datetime");

            //Quan hệ 1 - N với bảng BookingDetail
            entity.HasMany(e => e.BookingDetails).WithOne(u => u.Booking).HasForeignKey(e => e.BookId);
        });
        #endregion

        #region Cấu hình cho bảng BookingDetail
            modelBuilder.Entity<BookingDetail>(entity =>{
                entity.HasKey(k => k.BookingDetailId);
                entity.Property(p => p.TableId).IsRequired();
                entity.Property(p => p.BookId).IsRequired();
                entity.Property(p => p.Status).IsRequired().HasColumnType("varchar(20)");
                //Quan hệ 1 - N với bảng Table
                entity.HasOne(e => e.Table).WithMany(u => u.BookingDetails).HasForeignKey(e => e.TableId);
            });
        #endregion
                
    }
}
